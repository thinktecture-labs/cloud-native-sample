package main

import (
	"github.com/gin-gonic/gin"
	"github.com/penglongli/gin-metrics/ginmetrics"
)

func configureMetrics(e *gin.Engine) {
	m := ginmetrics.GetMonitor()
	m.SetMetricPath("/metrics")
	m.Use(e)
}
