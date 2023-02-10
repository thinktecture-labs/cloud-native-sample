package controllers

import (
	"context"
	"errors"

	"github.com/gin-gonic/gin"
	"github.com/thinktecture-labs/cloud-native-sample/ordermonitorservice/pkg/monitor"
	"github.com/thinktecture-labs/cloud-native-sample/ordermonitorservice/pkg/utils"
)

const (
	ordermonitorservice_prefix = "/orders"
)

func RegisterOrderMonitorServiceEndpoints(e *gin.Engine, cfg *monitor.Configuration) {
	g := e.Group(ordermonitorservice_prefix)
	g.GET("/monitor", func(ctx *gin.Context) {
		authValue := ctx.Request.Header.Get("Authorization")
		if len(authValue) == 0 {
			ctx.JSON(401, gin.H{"error": "missing authorization header"})
			return
		}
		// let the backend services do the actual authn check
		// we just need to forward the authn header

		c := context.WithValue(context.Background(), utils.AuthKey, authValue)

		res, err := monitor.GetOrderMonitorData(c, cfg.BackendTimeout)
		if err != nil && errors.Is(err, monitor.UnauthorizedError{}) {
			ctx.JSON(401, gin.H{"error": err.Error()})
			return
		}
		if err != nil {
			ctx.JSON(500, gin.H{"error": err.Error()})
			return
		}
		ctx.JSON(200, res)
	})
}
