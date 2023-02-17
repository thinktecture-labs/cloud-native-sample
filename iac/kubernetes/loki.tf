resource "helm_release" "grafana_loki" {
  name             = "loki"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "loki-distributed"
  namespace        = "loki"
  create_namespace = true

  values = [
    templatefile("${path.module}/values/loki.yml", {
      storage_account_name = var.loki_storage_account_name
      storage_account_key  = var.loki_storage_account_key
    }),
  ]
}

locals {
  loki_distributor_endpoint = "http://${helm_release.grafana_loki.name}-loki-distributed-distributor.${helm_release.grafana_loki.namespace}.svc.cluster.local:3100/loki/api/v1/push"
  loki_query_endpoint       = "http://${helm_release.grafana_loki.name}loki-distributed-query-frontend.${helm_release.grafana_loki.namespace}.svc.cluster.local:3100"

}
