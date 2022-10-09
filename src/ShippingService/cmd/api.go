package main

import (
	"context"
	"fmt"
	"log"
	"os"
	"strconv"

	"github.com/gin-gonic/gin"
	"github.com/penglongli/gin-metrics/ginmetrics"
	"github.com/sirupsen/logrus"
	"github.com/thinktecture-labs/cloud-native-sample/shipping-service/pkg/shipping"
	ginlogrus "github.com/toorop/gin-logrus"
	"go.opentelemetry.io/contrib/instrumentation/github.com/gin-gonic/gin/otelgin"
	"go.opentelemetry.io/otel"
	stdout "go.opentelemetry.io/otel/exporters/stdout/stdouttrace"
	"go.opentelemetry.io/otel/exporters/zipkin"
	"go.opentelemetry.io/otel/propagation"
	sdktrace "go.opentelemetry.io/otel/sdk/trace"
)

type cloudEvent struct {
	Id   string
	Data shipping.Order
}

const (
	serviceName = "ShippingService"
	defaultPort = 5000
)

var tracer = otel.Tracer(serviceName)

func configureMetrics(e *gin.Engine) {
	m := ginmetrics.GetMonitor()
	m.SetMetricPath("/metrics")
	m.Use(e)
}

func configureTracing(cfg *shipping.Configuration) (tp *sdktrace.TracerProvider, err error) {
	c, err := stdout.New(stdout.WithPrettyPrint())
	z, err := zipkin.New(cfg.ZipkinEndpoint)
	if err != nil {
		return nil, err
	}

	tp = sdktrace.NewTracerProvider(
		sdktrace.WithSampler(sdktrace.AlwaysSample()),
		sdktrace.WithBatcher(z),
		sdktrace.WithBatcher(c),
	)

	otel.SetTracerProvider(tp)
	otel.SetTextMapPropagator(propagation.NewCompositeTextMapPropagator(propagation.TraceContext{}, propagation.Baggage{}))
	return tp, nil
}

func getConfig() *shipping.Configuration {
	cfg, err := shipping.LoadConfiguration()
	if err != nil {
		log.Fatalf("Error while reading configuration: %s", err)
	}
	return cfg
}

func configureLogging(cfg *shipping.Configuration) *logrus.Logger {
	log := logrus.New()
	log.SetOutput(os.Stdout)
	log.SetLevel(logrus.DebugLevel)

	if cfg.IsProduction() {
		log.Infoln("Will run shipping service in release mode.")
		gin.SetMode(gin.ReleaseMode)
	}
	return log
}

func main() {
	cfg := getConfig()
	log := configureLogging(cfg)

	r := gin.New()

	configureMetrics(r)
	tp, err := configureTracing(cfg)
	if err != nil {
		log.Fatalf("Error while configuring tracing: %s", err)
	}
	defer func() {
		if err := tp.Shutdown(context.Background()); err != nil {
			log.Printf("Error shutting down tracer provider: %v", err)
		}
	}()

	r.Use(otelgin.Middleware(serviceName))
	r.Use(ginlogrus.Logger(log), gin.Recovery())

	r.POST("/orders", func(ctx *gin.Context) {
		_, span := tracer.Start(ctx.Request.Context(), "process_order")
		defer span.End()
		var orderEnvelope cloudEvent
		if err := ctx.BindJSON(&orderEnvelope); err != nil {
			ctx.AbortWithStatus(400)
			return
		}
		log.Infof("Processing CloudEvent with id %s", orderEnvelope.Id)
		s := shipping.NewShipping(cfg, log)
		if err = s.ProcessOrder(&orderEnvelope.Data); err != nil {
			ctx.AbortWithStatus(500)
			return
		}

		// we must send at least! an empty JSON object, otherwise dapr component will treat response incorrectly and log
		// skipping status check due to error parsing result from pub/sub event
		// https://github.com/dapr/dapr/issues/2235
		ctx.JSON(200, daprResponse{})

	})

	r.GET("/healthz/readiness", func(ctx *gin.Context) {
		ctx.Status(200)
	})

	r.GET("/healthz/liveness", func(ctx *gin.Context) {
		ctx.Status(200)
	})

	p := getPort()
	log.Infof("Starting shipping service on port %d", p)
	if err := r.Run(fmt.Sprintf(":%d", p)); err != nil {
		log.Fatalf("Error while running gin: %s", err)
	}
}

func getTraceProvider() *sdktrace.TracerProvider {
	exporter, err := zipkin.New(os.Getenv("ZIPKIN_ENDPOINT"))
	if err != nil {
		fmt.Errorf("Could not create zipkin exporter %s", err)
		os.Exit(1)
	}
	tp := sdktrace.NewTracerProvider(
		sdktrace.WithSampler(sdktrace.AlwaysSample()),
		sdktrace.WithBatcher(exporter),
	)
	otel.SetTracerProvider(tp)
	return tp
}

type daprResponse struct {
}

func getPort() int {
	p := os.Getenv("PORT")
	if len(p) == 0 {
		return defaultPort
	}
	port, err := strconv.Atoi(p)
	if err != nil {
		return defaultPort
	}
	return port
}
