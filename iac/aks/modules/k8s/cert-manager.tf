resource "helm_release" "certmanager" {
  name             = "cn-cert-manager"
  repository       = "https://charts.jetstack.io"
  chart            = "cert-manager"
  namespace        = "cert-manager"
  create_namespace = true

  set {
    name  = "installCRDs"
    value = "true"
  }
}

resource "helm_release" "cluster_issuer" {
  name             = "letsencrypt-cluster-issuer"
  chart            = "${path.root}/../../charts/cluster-issuer"
  namespace        = "cloud-native-sample"
  create_namespace = true
}
