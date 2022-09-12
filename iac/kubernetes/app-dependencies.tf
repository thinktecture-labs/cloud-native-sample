resource "helm_release" "rabbitmq" {
  name       = "rabbit"
  repository = "https://charts.bitnami.com/bitnami"
  chart      = "rabbitmq"
  timeout    = 600
  // this must be deployed to the same namespace as the the dapr components and
  // the workloads that use that particular dapr component
  namespace        = "rabbitmq"
  create_namespace = true
}

resource "helm_release" "dapr" {
  name             = "dapr"
  repository       = "https://dapr.github.io/helm-charts/"
  chart            = "dapr"
  timeout          = 600
  namespace        = "dapr-system"
  create_namespace = true
}

resource "kubernetes_ingress_v1" "dapr_dashboard" {
  depends_on = [
    helm_release.dapr
  ]

  metadata {
    name      = "dapr-dashboard"
    namespace = "dapr-system"
    annotations = {
      "cert-manager.io/cluster-issuer" = "letsencrypt-prod"
    }
  }
  spec {
    ingress_class_name = "nginx"
    tls {
      secret_name = "dapr-dashboard-tls"
      hosts       = ["cn-dapr.thinktecture-demos.com"]
    }
    rule {
      host = "cn-dapr.thinktecture-demos.com"
      http {
        path {
          path      = "/"
          path_type = "Prefix"
          backend {
            service {
              name = "dapr-dashboard"
              port {
                number = 8080
              }
            }
          }
        }
      }
    }
  }

}
