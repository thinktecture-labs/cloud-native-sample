resource "helm_release" "ingress" {
  name             = "cn-ingress-release"
  repository       = "https://kubernetes.github.io/ingress-nginx"
  chart            = "ingress-nginx"
  namespace        = "ingress"
  create_namespace = true
}


resource "helm_release" "certmanager" {
  name             = "cn-cert-manager"
  repository       = "https://charts.jetstack.io"
  chart            = "cert-manager"
  namespace        = "cert-manager"
  create_namespace = true
}
