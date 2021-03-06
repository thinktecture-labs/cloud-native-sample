resource "helm_release" "ingress" {
  name             = "cn-ingress"
  repository       = "https://kubernetes.github.io/ingress-nginx"
  chart            = "ingress-nginx"
  namespace        = "ingress"
  create_namespace = true
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

output "ingress_ip" {
  value = data.kubernetes_service.ing.status.0.load_balancer.0.ingress.0.ip
}

