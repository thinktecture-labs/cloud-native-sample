package main

import (
	"io/ioutil"
	"os"

	"github.com/gin-gonic/gin"
	"github.com/sirupsen/logrus"
	"github.com/thinktecture-labs/cloud-native-sample/shipping-service/pkg/shipping"
	ginlogrus "github.com/toorop/gin-logrus"
)

func configureLogging(r *gin.Engine, cfg *shipping.Configuration) *logrus.Logger {
	log := logrus.New()
	log.SetOutput(ioutil.Discard)
	log.SetLevel(logrus.DebugLevel)

	if !cfg.DisableConsoleLog {
		log.SetOutput(os.Stdout)
		r.Use(ginlogrus.Logger(log))
	}
	if cfg.IsProduction() {
		log.Infoln("Will run shipping service in release mode.")
		gin.SetMode(gin.ReleaseMode)
	}
	return log
}
