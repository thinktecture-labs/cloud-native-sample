package main

import (
	"fmt"
	"log"
	"os"
	"strconv"

	"github.com/gin-gonic/gin"
	"github.com/penglongli/gin-metrics/ginmetrics"
	"github.com/sirupsen/logrus"
	"github.com/thinktecture-labs/cloud-native-sample/shipping-service/pkg/shipping"
	ginlogrus "github.com/toorop/gin-logrus"
	"go.opentelemetry.io/otel"
	"go.opentelemetry.io/otel/exporters/zipkin"
	sdktrace "go.opentelemetry.io/otel/sdk/trace"
)

type cloudEvent struct {
	Id   string
	Data shipping.Order
}

const (
	defaultPort = 5000
)

func main() {
	cfg, err := shipping.LoadConfiguration()
	if err != nil {
		log.Fatalf("Error while reading configuration: %s", err)
	}
	log := logrus.New()
	log.SetOutput(os.Stdout)
	log.SetLevel(logrus.DebugLevel)

	if cfg.IsProduction() {
		log.Infoln("Will run shipping service in release mode.")
		gin.SetMode(gin.ReleaseMode)
	}

	r := gin.New()
	//todo: extract to dedicated func
	m := ginmetrics.GetMonitor()
	m.SetMetricPath("/metrics")
	m.Use(r)
	r.Use(ginlogrus.Logger(log), gin.Recovery())
	//todo: rename route to ship or process
	r.POST("/orders", func(ctx *gin.Context) {
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
