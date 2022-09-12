resource "helm_release" "grafana" {
  name             = "cn-grafana"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "grafana"
  timeout          = 600
  namespace        = "grafana"
  create_namespace = true

  set {
    name  = "ingress.enabled"
    value = "true"
  }

  set {
    name  = "ingress.hosts[0].host"
    value = "cn-grafana.thinktecture-demos.com"
  }

  set {
    name  = "ingress.tls[0].hosts[0]"
    value = "cn-grafana.thinktecture-demos.com"
  }

  set {
    name  = "ingres.tls[0].secretName"
    value = "grafanatls"
  }

  set {
    name  = "ingress.annotations.kubernetes.io/ingress.class"
    value = "nginx"
  }

  set {
    name  = "ingress.annotations.cert-manager.io/cluster-issuer"
    value = "letsencrypt-prod"
  }
}
