package main

import (
	"io/ioutil"
	"os"

	"github.com/gin-gonic/gin"
	"github.com/sirupsen/logrus"
	"github.com/thinktecture-labs/cloud-native-sample/ordermonitorservice/pkg/monitor"
	ginlogrus "github.com/toorop/gin-logrus"
)

func configureLogging(r *gin.Engine, cfg *monitor.Configuration) *logrus.Logger {
	log := logrus.New()
	log.SetOutput(ioutil.Discard)
	log.SetLevel(logrus.DebugLevel)

	if !cfg.DisableConsoleLog {
		log.SetOutput(os.Stdout)
		r.Use(ginlogrus.Logger(log))
	}
	if cfg.IsProduction() {
		log.Infoln("Will run OrderMonitorService in release mode.")
		gin.SetMode(gin.ReleaseMode)
	}
	return log
}
