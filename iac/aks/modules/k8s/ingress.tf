resource "helm_release" "ingress" {
  name             = "cn-ingress-release"
  repository       = "https://kubernetes.github.io/ingress-nginx"
  chart            = "ingress-nginx"
  namespace        = "ingress"
  create_namespace = true
}
