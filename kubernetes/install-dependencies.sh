#!/bin/bash

set -e

echo "Adding Helm repositories ..."
helm repo add dapr https://dapr.github.io/helm-charts/
helm repo add grafana https://grafana.github.io/helm-charts
helm repo add zipkin-helm https://financial-times.github.io/zipkin-helm/docs
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts

echo "Updating helm repositories ..."
helm repo update

echo "Installing Zipkin ..."
kubectl apply -f ./zipkin/namespace.yml
kubectl apply -f ./zipkin/app

echo "installing dapr ..."
helm upgrade --install dapr dapr/dapr --version=1.7 -n dapr-system --create-namespace -f ./helm-values/dapr.yml

echo "Installing RabbitMQ ..."
helm install rabbitmq -n rabbit --create-namespace bitnami/rabbitmq -f ./helm-values/rabbit.yml

echo "Installing Prometheus ..."
helm install prometheus -n prometheus --create-namespace prometheus-community/prometheus -f ./helm-values/prometheus.yml

echo "Installing Grafana Loki ..."
helm install loki -n loki --create-namespace grafana/loki -f ./helm-values/loki.yml

echo "Installing Grafana ..."
helm install grafana -n grafana --create-namespace grafana/grafana -f ./helm-values/grafana.yml

echo "Installing Grafana Promtail ..."
helm install promtail -n promtail --create-namespace grafana/promtail -f ./helm-values/promtail.yml
