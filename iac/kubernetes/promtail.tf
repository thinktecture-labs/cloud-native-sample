resource "helm_release" "grafana_promtail" {
  name             = "promtail"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "promtail"
  namespace        = "promtail"
  create_namespace = true

  values = [
    file("${path.module}/values/promtail.yml")
  ]
  set {
    name  = "config.clients[0].url"
    value = local.loki_distributor_endpoint
  }
}
