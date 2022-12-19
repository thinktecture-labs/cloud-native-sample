package main

import (
	"os"

	"github.com/gin-gonic/gin"
	"github.com/sirupsen/logrus"
	"github.com/thinktecture-labs/cloud-native-sample/shipping-service/pkg/shipping"
)

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
