package dapr

import (
	"fmt"
	"os"

	"github.com/gin-gonic/gin"
	"github.com/sirupsen/logrus"
)

const (
	apiTokenEnvVarName = "DAPR_API_TOKEN"
	apiTokenHeaderName = "dapr-api-token"
)

type DaprResponse struct{}

func ValidateApiToken(log *logrus.Logger) gin.HandlerFunc {
	return func(ctx *gin.Context) {
		e, err := getDaprApiToken()
		if err != nil {
			log.Errorf("Dapr token not specified: %s", err)
			ctx.AbortWithStatus(500)
			return
		}
		a := ctx.GetHeader(apiTokenHeaderName)
		if a != e {
			log.Warnf("%s received from daprd", a)
			ctx.AbortWithStatus(401)
			return
		}
		// keep this log for showing it in a live-demo
		// otherwise you should not log the token
		log.Debugf("received valid token from daprd: %s", a)
		ctx.Next()
	}
}

func getDaprApiToken() (string, error) {
	// read from DAPR_API_TOKEN env var
	t, ok := os.LookupEnv(apiTokenEnvVarName)
	if !ok || len(t) == 0 {
		return "", fmt.Errorf("the %s not set or value empty", apiTokenEnvVarName)
	}
	return t, nil
}
