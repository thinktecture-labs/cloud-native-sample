package shipping

import (
	"context"
	"fmt"
	"time"

	dapr "github.com/dapr/go-sdk/client"
)

type Shipping struct {
	cfg *ShippingServiceConfiguration
}

func NewShipping(cfg *ShippingServiceConfiguration) *Shipping {
	return &Shipping{
		cfg: cfg,
	}
}

type ShippingServiceConfiguration struct {
	PubSubName string
	TopicName  string
	Mode       string
}

type ShippingProcessed struct {
	UserId       string
	CustomerName string
	OrderId      string
}

type Order struct {
	Id           string
	UserId       string
	CustomerName string
	SubmittedAt  time.Time
	Positions    []OrderPosition
}

type OrderPosition struct {
	ProductId   string
	ProductName string
	Quantity    int
}

func (s *Shipping) ProcessOrder(o *Order) error {
	c, err := dapr.NewClient()
	if err != nil {
		return fmt.Errorf("Error while creating dapr client %s", err)
	}

	m := &ShippingProcessed{
		CustomerName: o.CustomerName,
		OrderId:      o.Id,
		UserId:       o.UserId,
	}

	if err = c.PublishEvent(context.Background(), s.cfg.PubSubName, s.cfg.TopicName, m); err != nil {
		return fmt.Errorf("Will fail because publishing order-processed event failed: %s", err)
	}
	return nil
}
