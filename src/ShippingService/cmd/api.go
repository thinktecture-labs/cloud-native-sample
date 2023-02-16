package main

import (
	"context"
	"fmt"
	"log"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/thinktecture-labs/cloud-native-sample/shipping-service/pkg/cloudevents"
	"github.com/thinktecture-labs/cloud-native-sample/shipping-service/pkg/dapr"
	"github.com/thinktecture-labs/cloud-native-sample/shipping-service/pkg/shipping"

	"go.opentelemetry.io/contrib/instrumentation/github.com/gin-gonic/gin/otelgin"
	"go.opentelemetry.io/otel"
)

const (
	serviceName = "ShippingService"
)

var tracer = otel.Tracer(serviceName)

func main() {
	cfg := getConfig()

	r := gin.New()
	log := configureLogging(r, cfg)

	configureMetrics(r)
	tp, err := configureTracing(cfg)
	if err != nil {
		log.Fatalf("Error while configuring tracing: %s", err)
	}
	if tp != nil {
		defer func() {
			if err := tp.Shutdown(context.Background()); err != nil {
				log.Printf("Error shutting down tracer provider: %v", err)
			}
		}()
	}

	dontTrace := otelgin.WithFilter(func(r *http.Request) bool {
		urls := []string{"/healthz/readiness", "/healthz/liveness", "/metrics"}
		// return false if request url is in the list
		for _, url := range urls {
			if r.URL.Path == url {
				return false
			}
		}
		return true
	})

	r.Use(otelgin.Middleware(serviceName, dontTrace))
	r.Use(gin.Recovery())
	r.GET("/dapr/subscribe", dapr.GetSubscriptionHandler(cfg))

	r.POST("/orders", dapr.ValidateApiToken(log), func(ctx *gin.Context) {
		_, span := tracer.Start(ctx.Request.Context(), "process_order")
		defer span.End()
		var envelope cloudevents.CloudEvent
		if err := ctx.BindJSON(&envelope); err != nil {
			log.Warnf("BadRequest - Error while binding JSON to CloudEvent %s", err)
			ctx.AbortWithStatus(400)
			return
		}
		log.Infof("Processing CloudEvent of type %s (id %s)", envelope.Type, envelope.Id)
		s := shipping.NewShipping(cfg, log)
		traceId := span.SpanContext().TraceID().String()
		if err = s.ProcessOrder(&envelope.Data, traceId); err != nil {
			ctx.AbortWithStatus(500)
			return
		}
		// we must send at least! an empty JSON object, otherwise dapr component will treat response incorrectly and log
		// skipping status check due to error parsing result from pub/sub event
		// https://github.com/dapr/dapr/issues/2235
		ctx.JSON(200, dapr.DaprResponse{})
	})
	healthz := r.Group("/healthz")
	healthz.GET("/readiness", ok)
	healthz.GET("/liveness", ok)

	log.Infof("Starting shipping service on port %d", cfg.Port)
	if err := r.Run(fmt.Sprintf(":%d", cfg.Port)); err != nil {
		log.Fatalf("Error while running gin: %s", err)
	}
}

func ok(c *gin.Context) {
	c.Status(200)
}

func getConfig() *shipping.Configuration {
	cfg, err := shipping.LoadConfiguration()
	if err != nil {
		log.Fatalf("Error while reading configuration: %s", err)
	}
	return cfg
}
