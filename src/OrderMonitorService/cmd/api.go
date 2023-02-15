package main

import (
	"context"
	"fmt"
	"log"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/thinktecture-labs/cloud-native-sample/ordermonitorservice/pkg/controllers"
	"github.com/thinktecture-labs/cloud-native-sample/ordermonitorservice/pkg/monitor"
	"go.opentelemetry.io/contrib/instrumentation/github.com/gin-gonic/gin/otelgin"
	"go.opentelemetry.io/otel"
)

const (
	serviceName = "OrderMonitorService"
)

var tracer = otel.Tracer(serviceName)

func main() {
	log.Printf("Starting %s", serviceName)
	cfg := getConfig()
	e := gin.New()
	log := configureLogging(e, cfg)

	configureMetrics(e)
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

	e.Use(otelgin.Middleware(serviceName, dontTrace))
	e.Use(gin.Recovery())

	controllers.RegisterHealthEndpoints(e)
	controllers.RegisterOrderMonitorServiceEndpoints(e, cfg)

	log.Infof("Starting %s on port %d", serviceName, cfg.Port)
	if err := e.Run(fmt.Sprintf(":%d", cfg.Port)); err != nil {
		log.Fatalf("Error while running %s: %s", serviceName, err)
	}

}

func getConfig() *monitor.Configuration {
	cfg, err := monitor.LoadConfiguration()
	if err != nil {
		log.Fatalf("Error while reading configuration: %s", err)
	}
	return cfg
}
