resource "helm_release" "grafana_loki" {
  name             = "loki"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "loki"
  namespace        = "loki"
  create_namespace = true

  set {
    name  = "loki.storage.type"
    value = "filesystem"
  }

  set {
    name  = "loki.auth.enabled"
    value = false
  }
}
