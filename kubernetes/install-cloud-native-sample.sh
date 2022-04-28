#!/bin/bash
set -e
echo "Creating cloud-native-sample namespace..."

kubectl apply -f ./cloud-native-sample/namespace.yml

echo "Creating secret in cloud-native-sample..."
kubectl create secret generic rabbit --from-literal connectionstring="amqp://guest:guest@rabbitmq.rabbit.svc.cluster.local:5672"

echo "Deploying Dapr Configuration & Components..."
kubect apply -f ./cloud-native-sample/dapr

echo "Installing Gateway ..."
helm upgrade --install gateway -n cloud-native-sample ./../charts/gateway
echo "Installing Orders ..."
helm upgrade --install orders -n cloud-native-sample ./../charts/orders
echo "Installing Products ..."
helm upgrade --install products -n cloud-native-sample ./../charts/products
echo "Installing Shipping ..."
helm upgrade --install shipping -n cloud-native-sample ./../charts/shipping
echo "Installing Notification ..."
helm upgrade --install notification -n cloud-native-sample ./../charts/notification
echo "Installing Notification ..."
helm upgrade --install order-monitor-client -n cloud-native-sample ./../charts/order-monitor-client
