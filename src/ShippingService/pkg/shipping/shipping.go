package shipping

import (
	"context"
	"fmt"
	"math/rand"

	"time"

	dapr "github.com/dapr/go-sdk/client"
	"github.com/sirupsen/logrus"
)

type Shipping struct {
	cfg *Configuration
	log *logrus.Logger
}

func NewShipping(cfg *Configuration, log *logrus.Logger) *Shipping {
	return &Shipping{
		cfg: cfg,
		log: log,
	}
}

type ShippingProcessed struct {
	UserId       string `json:"userId"`
	CustomerName string `json:"userName"`
	OrderId      string `json:"orderId"`
}

type Order struct {
	Id           string          `json:"id"`
	UserId       string          `json:"userId"`
	CustomerName string          `json:"userName"`
	SubmittedAt  time.Time       `json:"submittedAt"`
	Positions    []OrderPosition `json:"positions"`
}

type OrderPosition struct {
	Id        string `json:"id"`
	ProductId string `json:"productId"`
	Quantity  int    `json:"quantity"`
}

func (s *Shipping) ProcessOrder(o *Order, traceId string) error {
	c, err := dapr.NewClient()
	if err != nil {
		return fmt.Errorf("error while creating dapr client %s", err)
	}

	// invoke business logic ;-)
	s.log.Infof("Applying shipping business logic on message %s", o.Id)

	rand.Seed(time.Now().UnixNano())
	min := 2
	max := 4
	randomNumber := rand.Intn(max-min+1) + min

	s.log.Infof("Shipping business logic takes %d seconds", randomNumber)

	time.Sleep(time.Second * time.Duration(randomNumber))

	m := &ShippingProcessed{
		CustomerName: o.CustomerName,
		OrderId:      o.Id,
		UserId:       o.UserId,
	}
	s.log.Infof("Publishing event to %s %s", s.cfg.TargetPubSubName, s.cfg.TargetTopicName)
	c.WithTraceID(context.TODO(), traceId)
	if err = c.PublishEvent(context.Background(), s.cfg.TargetPubSubName, s.cfg.TargetTopicName, m); err != nil {
		return fmt.Errorf("will fail because publishing order-processed event failed: %s", err)
	}
	return nil
}
