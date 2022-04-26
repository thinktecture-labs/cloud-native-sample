# cloud-native-sample

## Start Services

### OrdersService

```bash
cd src/OrdersService
dapr run --app-id orders --app-port 5002 --dapr-http-port 9002 --dapr-grpc-port 10002 --components-path ../dapr/components --config ../dapr/config.yaml --log-level debug -- dotnet run
```

### NotificationService

```bash
cd src/NotificationService
dapr run --app-id notification --app-port 5004 --dapr-http-port 9004 --dapr-grpc-port 10004 --components-path ../dapr/components --config ../dapr/config.yaml -- dotnet run
```

### ShippingService

```bash
cd src/ShippingService
dapr run --app-id shipping --app-port 5003 --dapr-http-port 9003 --dapr-grpc-port 10003 --components-path ../dapr/components --config ../dapr/config.yaml --log-level debug -- go run ./cmd/api.go
```

### ProductsService

```bash
cd src/ProductsService
dapr run --app-id products --app-port 5001 --dapr-http-port 9001 --dapr-grpc-port 10001 --config ../dapr/config.yaml -- dotnet run
```

### Gateway

```bash
cd src/Gateway
dotnet run
```

## Load Testing

```bash
hey -c 1 -n 1000 http://localhost:5000/products
```
