resource "helm_release" "prometheus" {
  name             = "prometheus"
  repository       = "https://prometheus-community.github.io/helm-charts"
  chart            = "prometheus"
  namespace        = "prometheus"
  create_namespace = true

  values = [file("${path.module}/values/prometheus.yml")]
}

locals {
  prometheus_endpoint = "http://${helm_release.prometheus.name}-server.${helm_release.prometheus.namespace}.svc.cluster.local:80"
}
