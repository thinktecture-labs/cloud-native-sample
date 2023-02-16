package monitor

import (
	"context"
	"errors"
	"fmt"
	"io/ioutil"
	"net/http"
	"sync"
	"time"

	"github.com/thinktecture-labs/cloud-native-sample/ordermonitorservice/pkg/utils"
)

const (
	methodOrders   = "orders"
	methodProducts = "products"
)

func GetOrderMonitorData(ctx context.Context, timeout int) ([]OrderListModel, error) {
	// we need to get the data from the orders and products backend
	// so we want to call them in parallel using go-routines
	resChan := make(chan backendResult, 2)
	var wg sync.WaitGroup
	wg.Add(2)

	go getData(ctx, &wg, resChan, kindOrders, methodOrders, timeout)
	go getData(ctx, &wg, resChan, kindProducts, methodProducts, timeout)

	// wait for the slowest request to finish
	wg.Wait()

	data1 := <-resChan
	data2 := <-resChan
	// check if one of the backen requests failed

	if data1.err != nil || data2.err != nil && (errors.Is(data1.err, UnauthorizedError{}) || errors.Is(data2.err, UnauthorizedError{})) {
		return nil, UnauthorizedError{}
	}

	if data1.err != nil {

		return nil, fmt.Errorf("error getting data from %s: %w", data1.kind, data1.err)
	}

	if data2.err != nil {
		return nil, fmt.Errorf("error getting data from %s: %w", data2.kind, data2.err)
	}
	o, err := getOrders(data1, data2)
	if err != nil {
		return nil, err
	}
	p, err := getProducts(data1, data2)
	if err != nil {
		return nil, err
	}

	return buildResult(o, p)

}

func buildResult(orders []Order, products []Product) ([]OrderListModel, error) {

	result := make([]OrderListModel, 0)

	for _, order := range orders {
		model := OrderListModel{
			Id:        order.Id,
			UserId:    order.UserId,
			Positions: make([]PositionListModel, len(order.Positions)),
		}
		for i, position := range order.Positions {
			product := getProductById(position.ProductId, products)
			if product == nil {
				return nil, fmt.Errorf("product %s not found", position.ProductId)
			}
			model.Positions[i] = PositionListModel{
				ProductId:          position.ProductId,
				ProductName:        product.Name,
				ProductPrice:       product.Price,
				ProductDescription: product.Description,
				Quantity:           position.Quantity,
			}

		}
		result = append(result, model)
	}
	return result, nil
}

func getData(ctx context.Context, wg *sync.WaitGroup, r chan backendResult, service string, action string, timeout int) {
	defer wg.Done()
	u, err := buildBackendUrl(service, action)
	if err != nil {
		r <- newErrorBackendResult(err, service)
		return
	}
	// we want to wait a maximum of 2 secs for the request to finish
	client := http.Client{
		Timeout: time.Duration(timeout) * time.Second,
	}

	req, _ := http.NewRequest(http.MethodGet, u, nil)
	req.Header.Set("Accept", "application/json")
	req.Header.Set("Content-Type", "application/json")
	req.Header.Set("Authorization", ctx.Value(utils.AuthKey).(string))

	res, err := client.Do(req)

	if err != nil {
		r <- newErrorBackendResult(err, service)
		return
	}

	// check if status code is 401 and return custom error
	if res.StatusCode == http.StatusUnauthorized {
		r <- newErrorBackendResult(UnauthorizedError{}, service)
		return
	}

	defer res.Body.Close()

	data, err := ioutil.ReadAll(res.Body)
	if err != nil {
		r <- newErrorBackendResult(err, service)
		return
	}
	r <- newBackendResult(data, service)
}
