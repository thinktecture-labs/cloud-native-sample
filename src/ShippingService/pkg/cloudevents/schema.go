package cloudevents

import "github.com/thinktecture-labs/cloud-native-sample/shipping-service/pkg/shipping"

type CloudEvent struct {
	Id   string         `json:"id"`
	Type string         `json:"type"`
	Data shipping.Order `json:"data"`
}
