resource "helm_release" "certmanager" {
  name             = "cn-cert-manager"
  repository       = "https://charts.jetstack.io"
  chart            = "cert-manager"
  timeout          = 600
  namespace        = "cert-manager"
  create_namespace = true

  set {
    name  = "installCRDs"
    value = "true"
  }
}
