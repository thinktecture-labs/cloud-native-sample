resource "helm_release" "grafana" {
  name             = "cn-grafana"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "grafana"
  timeout          = 600
  namespace        = "grafana"
  create_namespace = true

  values = [
    templatefile("${path.module}/values/grafana.yml", {
      prometheus_endpoint = local.prometheus_endpoint
      loki_endpoint       = local.loki_query_endpoint
      zipkin_endpoint     = local.zipkin_endpoint
    }),
  ]
}
