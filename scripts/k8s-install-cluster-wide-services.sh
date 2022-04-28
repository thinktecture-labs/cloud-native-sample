#!/bin/bash

set -e

echo "Adding Helm repos"
helm repo add dapr https://dapr.github.io/helm-charts/
helm repo add grafana https://grafana.github.io/helm-charts
helm repo add zipkin-helm https://financial-times.github.io/zipkin-helm/docs
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo add ygqygq2 https://ygqygq2.github.io/charts/

echo "Updating helm repos"
helm repo update

echo "installing zipkin"
k create ns zipkin
k apply -f ./zipkin.yml

echo "installing DAPR"
helm upgrade --install dapr dapr/dapr --version=1.7 -n dapr-system --create-namespace -f ./values/dapr.yml

echo "installing rabbitmq"
helm install rabbitmq -n rabbit --create-namespace bitnami/rabbitmq -f ./values/rabbit.yml

echo "installing prometheus"
helm install prometheus -n prometheus --create-namespace prometheus-community/prometheus -f ./values/prometheus.yml

echo "installing loki"
helm install loki -n loki --create-namespace grafana/loki -f ./values/loki.yml

echo "installing grafana"
helm install grafana -n grafana --create-namespace grafana/grafana -f ./values/grafana.yml

echo "installing promtail"


echo "create sample namespace"
kubectl create ns cloud-native-sample
kubectl create secret generic rabbit --from-literal connectionstring="amqp://guest:guest@rabbitmq.rabbit.svc.cluster.local:5672"

kubect apply -f ./dapr-configuration.yml
kubect apply -f ./dapr-components.yml
