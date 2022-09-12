// helm release for zipkin 
resource "helm_release" "zipkin" {
  name             = "cn-zipkin"
  repository       = "https://financial-times.github.io/zipkin-helm/docs"
  chart            = "zipkin"
  timeout          = 600
  namespace        = "zipkin"
  create_namespace = true
}

// helm release for grafana
resource "helm_release" "grafana" {
  name             = "cn-grafana"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "grafana"
  timeout          = 600
  namespace        = "grafana"
  create_namespace = true

  set {
    name  = "ingrss.enabled"
    value = "true"
  }

}

// helm release for prometheus
resource "helm_release" "prometheus" {
  name             = "prometheus"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "prometheus"
  namespace        = "prometheus"
  create_namespace = true
}

// helm release grafana loki
resource "helm_release" "grafana_loki" {
  name             = "loki"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "loki"
  namespace        = "loki"
  create_namespace = true
}

// helm release grafana promtail
resource "helm_release" "grafana_promtail" {
  name             = "promtail"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "promtail"
  namespace        = "promtail"
  create_namespace = true

  set {
    name  = "config.lokiAddress"
    value = "xxx"
  }
}
