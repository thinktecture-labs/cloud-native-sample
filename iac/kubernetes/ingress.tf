resource "helm_release" "ingress" {
  name             = "cn-ingress"
  repository       = "https://kubernetes.github.io/ingress-nginx"
  chart            = "ingress-nginx"
  timeout          = 600
  namespace        = "ingress"
  create_namespace = true

  values = [
    file("${path.module}/values/ingress.yml"),
  ]
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
