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

resource "kubernetes_manifest" "clusterissuer" {
  manifest = {
    "apiVersion" = "cert-manager.io/v1"
    "kind"       = "ClusterIssuer"
    "metadata" = {
      "name" = "letsencrypt-prod"
    }
    "spec" = {
      "acme" = {
        "email" = "thorsten.hans@thinktecture.com"
        "privateKeySecretRef" = {
          "name" = "letsencrypt-prod"
        }
        "server" = "https://acme-v02.api.letsencrypt.org/directory"
        "solvers" = [
          {
            "http01" = {
              "ingress" = {
                "class" = "nginx"
              }
            }
          },
        ]
      }
    }
  }

  depends_on = [
    helm_release.certmanager
  ]

}
