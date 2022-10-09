resource "helm_release" "grafana_promtail" {
  name             = "promtail"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "promtail"
  namespace        = "promtail"
  create_namespace = true

  values = [
    "${file("promtail.values.yml")}"
  ]
  set {
    name  = "config.clients[0].url"
    value = "http://${helm_release.grafana_loki.name}.${helm_release.grafana_loki.namespace}.svc.cluster.local:3100/loki/api/v1/push"
  }
}
