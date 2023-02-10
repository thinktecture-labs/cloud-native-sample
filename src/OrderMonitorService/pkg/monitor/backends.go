package monitor

import (
	"encoding/json"
	"fmt"
	"os"
)

const (
	kindOrders   = "orders"
	kindProducts = "products"
)

type UnauthorizedError struct {
}

func (e UnauthorizedError) Error() string {
	return "unauthorized"
}

type backendResult struct {
	err  error
	kind string
	data []byte
}

func newErrorBackendResult(err error, kind string) backendResult {
	return backendResult{
		err:  err,
		kind: kind,
	}
}

func newBackendResult(data []byte, kind string) backendResult {
	return backendResult{
		data: data,
		kind: kind,
	}
}

func (r backendResult) isOrders() bool {
	return r.kind == kindOrders
}

func (r backendResult) asOrders() ([]Order, error) {
	if r.err != nil {
		return nil, r.err
	}
	if r.kind != kindOrders {
		return nil, fmt.Errorf("invalid kind %s", r.kind)
	}
	var orders []Order
	err := json.Unmarshal(r.data, &orders)
	if err != nil {
		fmt.Println("error unmarshalling orders")
		fmt.Println(string(r.data))
	}
	return orders, err
}

func (r backendResult) asProducts() ([]Product, error) {
	if r.err != nil {
		return nil, r.err
	}
	if r.kind != kindProducts {
		return nil, fmt.Errorf("invalid kind %s", r.kind)
	}
	var products []Product
	err := json.Unmarshal(r.data, &products)
	if err != nil {
		fmt.Println("error unmarshalling products")
		fmt.Println(string(r.data))
	}
	return products, err
}

func (r backendResult) isProducts() bool {
	return r.kind == kindProducts
}

type Order struct {
	Id        string     `json:"id"`
	UserId    string     `json:"userId"`
	Positions []Position `json:"positions"`
}

type Position struct {
	ProductId string `json:"productId"`
	Quantity  int    `json:"quantity"`
}

type Product struct {
	Id          string  `json:"id"`
	Name        string  `json:"name"`
	Description string  `json:"description"`
	Price       float64 `json:"price"`
}

func buildBackendUrl(service string, action string) (string, error) {
	daprHttpPort := os.Getenv("DAPR_HTTP_PORT")
	if len(daprHttpPort) == 0 {
		return "", fmt.Errorf("DAPR_HTTP_PORT not set")
	}
	return fmt.Sprintf("http://localhost:%s/v1.0/invoke/%s/method/%s", daprHttpPort, service, action), nil
}

func getOrders(r1 backendResult, r2 backendResult) ([]Order, error) {
	if r1.isOrders() {
		return r1.asOrders()
	}
	if r2.isOrders() {
		return r2.asOrders()
	}
	return nil, fmt.Errorf("no orders found")
}

func getProducts(r1 backendResult, r2 backendResult) ([]Product, error) {
	if r1.isProducts() {
		return r1.asProducts()
	}
	if r2.isProducts() {
		return r2.asProducts()
	}
	return nil, fmt.Errorf("no products found")
}

func getProductById(productId string, products []Product) *Product {
	for _, product := range products {
		if product.Id == productId {
			return &product
		}
	}
	return nil
}
