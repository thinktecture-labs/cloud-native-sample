package dapr

import (
	"github.com/gin-gonic/gin"
	"github.com/thinktecture-labs/cloud-native-sample/shipping-service/pkg/shipping"
)

type subscription struct {
	PubsubName string            `json:"pubsubname"`
	Topic      string            `json:"topic"`
	Metadata   map[string]string `json:"metadata,omitempty"`
	Routes     routes            `json:"routes"`
}

type routes struct {
	Rules   []rule `json:"rules,omitempty"`
	Default string `json:"default,omitempty"`
}

type rule struct {
	Match string `json:"match"`
	Path  string `json:"path"`
}

func GetSubscriptionHandler(cfg *shipping.Configuration) gin.HandlerFunc {

	return func(c *gin.Context) {
		t := []subscription{
			{
				PubsubName: cfg.SourcePubSubName,
				Topic:      cfg.SourceTopicName,
				Routes: routes{
					Rules: []rule{
						{
							Match: `event.type == "com.thinktecture/new-order"`,
							Path:  "/orders",
						},
					},
					Default: "/orders",
				},
			},
		}
		c.JSON(200, t)
	}
}
