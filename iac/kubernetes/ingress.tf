resource "helm_release" "ingress" {
  name             = "cn-ingress"
  repository       = "https://kubernetes.github.io/ingress-nginx"
  chart            = "ingress-nginx"
  timeout          = 600
  namespace        = "ingress"
  create_namespace = true

  set {
    name  = "controller.service.annotations.service\\.beta\\.kubernetes\\.io/azure-load-balancer-health-probe-request-path"
    value = "/healthz"
  }
}

data "kubernetes_service" "ing" {
  metadata {
    name      = "${helm_release.ingress.name}-ingress-nginx-controller"
    namespace = helm_release.ingress.namespace
  }
  depends_on = [
    helm_release.ingress
  ]
}
