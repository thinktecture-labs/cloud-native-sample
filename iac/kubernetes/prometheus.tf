resource "helm_release" "prometheus" {
  name             = "prometheus"
  repository       = "https://prometheus-community.github.io/helm-charts"
  chart            = "prometheus"
  namespace        = "prometheus"
  create_namespace = true

  set {
    name  = "serverFiles.alerting_rules.yml"
    value = file("${path.module}/manifests/alerting_rules.yml")
  }
}

