#!/bin/bash

set -e

echo "Adding Helm repos"
helm repo add dapr https://dapr.github.io/helm-charts/
helm repo add grafana https://grafana.github.io/helm-charts
helm repo add zipkin-helm https://financial-times.github.io/zipkin-helm/docs

echo "Updating helm repos"
helm repo update

echo "installing DAPR"
helm upgrade --install dapr dapr/dapr --version=1.7 -n dapr-system --create-namespace --wait

echo "installing rabbitmq"
helm install rabbitmq -n rabbit --create-namespace bitnami/rabbitmq

echo "installing grafana"
helm install grafana -n grafana --create-namespace grafana/grafana -f ./values/grafana.yml

echo "installing prometheus"

echo "installing zipkin"

echo "installing loki"
helm install loki -n loki --create-namespace grafana/loki -f ./values/loki.yml

echo "installing promtail"
