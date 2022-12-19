package main

import (
	"github.com/thinktecture-labs/cloud-native-sample/shipping-service/pkg/shipping"
	"go.opentelemetry.io/otel"
	"go.opentelemetry.io/otel/exporters/zipkin"
	"go.opentelemetry.io/otel/propagation"
	sdktrace "go.opentelemetry.io/otel/sdk/trace"
)

func configureTracing(cfg *shipping.Configuration) (tp *sdktrace.TracerProvider, err error) {
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
		sdktrace.WithBatcher(c),
	)

	otel.SetTracerProvider(tp)
	otel.SetTextMapPropagator(propagation.NewCompositeTextMapPropagator(propagation.TraceContext{}, propagation.Baggage{}))
	return tp, nil
}
