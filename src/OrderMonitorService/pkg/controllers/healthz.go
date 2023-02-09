package controllers

import "github.com/gin-gonic/gin"

const (
	healthz_prefix = "/healthz"
)

func RegisterHealthEndpoints(e *gin.Engine) {
	g := e.Group(healthz_prefix)
	g.GET("/readiness", func(ctx *gin.Context) {
		ctx.JSON(200, gin.H{
			"ready": true,
		})
	})

	g.GET("/liveness", func(ctx *gin.Context) {
		ctx.JSON(200, gin.H{
			"alive": true,
		})
	})
}
