
resource "helm_release" "dapr" {
  name             = "dapr"
  repository       = "https://dapr.github.io/helm-charts/"
  chart            = "dapr"
  timeout          = 600
  namespace        = "dapr-system"
  create_namespace = true
}

resource "kubernetes_manifest" "dapr_configuration" {
  manifest = {
    "apiVersion" = "dapr.io/v1alpha1"
    "kind"       = "Configuration"
    "metadata" = {
      "name"      = "cloud-native-sample"
      "namespace" = data.kubernetes_namespace.app.metadata[0].name
    }
    "spec" = {
      "metric" = {
        "enabled" = true
      }
      "tracing" = {
        "samplingRate" = "${var.dapr_config_sample_rate}"
        "zipkin" = {
          "endpointAddress" = "http://${data.kubernetes_service_v1.zipkin.metadata[0].name}.${data.kubernetes_service_v1.zipkin.metadata[0].namespace}.svc.cluster.local:9411/api/v2/spans"
        }
      }
    }
  }
}

resource "kubernetes_manifest" "dapr_component_orders" {
  manifest = yamldecode(file("${path.module}/manifests/orders_${var.message_broker}.yml"))
}

resource "kubernetes_manifest" "dapr_component_pricedrops" {
  manifest = yamldecode(file("${path.module}/manifests/pricedrops_${var.message_broker}.yml"))
}

resource "kubernetes_manifest" "dapr_component_email" {
  manifest = yamldecode(file("${path.module}/manifests/email.yml"))
}

resource "kubernetes_secret_v1" "dapr_auth_token" {
  metadata {
    name      = "dapr-api-auth"
    namespace = data.kubernetes_namespace.app.metadata[0].name
  }
  // todo: set this to a value provided via variable
  data = {
    token = var.dapr_api_auth_token
  }
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
