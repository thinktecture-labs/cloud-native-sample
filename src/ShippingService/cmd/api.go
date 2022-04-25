package main

import (
	"fmt"
	"log"
	"os"
	"strconv"

	"github.com/gin-gonic/gin"
	"github.com/sirupsen/logrus"
	"github.com/thinktecture-labs/cloud-native-sample/shipping-service/pkg/shipping"
	ginlogrus "github.com/toorop/gin-logrus"
)

type cloudEvent struct {
	Data shipping.Order
}

const (
	defaultPort = 5003
)

func main() {
	cfg, err := shipping.LoadConfiguration()
	if err != nil {
		log.Fatalf("Error while reading configuration: %s", err)
	}
	log := logrus.New()
	log.SetOutput(os.Stdout)
	log.SetLevel(logrus.DebugLevel)
	if cfg.Mode == "Release" {
		log.Infoln("Will run shipping service in release mode.")
		gin.SetMode(gin.ReleaseMode)
	}
	r := gin.New()

	r.Use(ginlogrus.Logger(log), gin.Recovery())
	r.POST("/orders", func(ctx *gin.Context) {
		var orderEnvelope cloudEvent
		if err := ctx.BindJSON(&orderEnvelope); err != nil {
			ctx.AbortWithStatus(400)
			return
		}
		s := shipping.NewShipping(cfg)
		if err = s.ProcessOrder(&orderEnvelope.Data); err != nil {
			ctx.AbortWithStatus(500)
			return
		}
		ctx.Status(200)

	})
	p := getPort()
	r.Run(fmt.Sprintf(":%d", p))
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
