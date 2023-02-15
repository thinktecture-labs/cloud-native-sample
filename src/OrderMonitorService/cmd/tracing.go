package main

import (
	"github.com/thinktecture-labs/cloud-native-sample/ordermonitorservice/pkg/monitor"
	"go.opentelemetry.io/otel"
	"go.opentelemetry.io/otel/exporters/zipkin"
	"go.opentelemetry.io/otel/propagation"
	"go.opentelemetry.io/otel/sdk/resource"
	sdktrace "go.opentelemetry.io/otel/sdk/trace"
	semconv "go.opentelemetry.io/otel/semconv/v1.17.0"
)

func configureTracing(cfg *monitor.Configuration) (tp *sdktrace.TracerProvider, err error) {
	if len(cfg.ZipkinEndpoint) == 0 {
		return nil, nil
	}
	z, err := zipkin.New(cfg.ZipkinEndpoint)
	if err != nil {
		return nil, err
	}

	tp = sdktrace.NewTracerProvider(
		sdktrace.WithSampler(sdktrace.AlwaysSample()),
		sdktrace.WithBatcher(z),
		sdktrace.WithResource(resource.NewWithAttributes(
			semconv.SchemaURL,
			semconv.ServiceName(serviceName),
		)),
	)

	otel.SetTracerProvider(tp)
	otel.SetTextMapPropagator(propagation.NewCompositeTextMapPropagator(propagation.TraceContext{}, propagation.Baggage{}))
	return tp, nil
}
