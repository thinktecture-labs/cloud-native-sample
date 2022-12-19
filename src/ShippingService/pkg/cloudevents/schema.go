package cloudevents

import "github.com/thinktecture-labs/cloud-native-sample/shipping-service/pkg/shipping"

type CloudEvent struct {
	Id   string
	Data shipping.Order
}
