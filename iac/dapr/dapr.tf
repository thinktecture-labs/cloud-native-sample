
resource "helm_release" "dapr" {
  name             = "dapr"
  repository       = "https://dapr.github.io/helm-charts/"
  chart            = "dapr"
  timeout          = 600
  namespace        = "dapr-system"
  create_namespace = true
}

resource "kubernetes_manifest" "darp_configuration" {
  manifest = {
    "apiVersion" = "dapr.io/v1alpha1"
    "kind"       = "Configuration"
    "metadata" = {
      "name"      = "cloud-native-sample"
      "namespace" = data.kubernetes_namespace.app.metadata[0].name
    }
    "spec" = {
      "tracing" = {
        "samplingRate" = "${var.dapr_config_sample_rate}"
        "zipkin" = {
          "endpointAddress" = "http://${data.kubernetes_service_v1.zipkin.metadata[0].name}.${data.kubernetes_service_v1.zipkin.metadata[0].namespace}.svc.cluster.local:9411/api/v2/spans"
        }
      }
    }
  }
}

resource "kubernetes_manifest" "dapr_component_orders_rabbit" {
  manifest = {
    "apiVersion" = "dapr.io/v1alpha1"
    "kind"       = "Component"
    "metadata" = {
      "name"      = "orders"
      "namespace" = data.kubernetes_namespace.app.metadata[0].name
    }
    "spec" = {
      "type"    = "bindings.rabbitmq"
      "version" = "v1"
      "metadata" = [
        {
          "name"  = "queueName"
          "value" = "orders"
        },
        {
          "name" = "host"
          "secretKeyRef" = {
            "name" = "rabbitmq"
            "key"  = "connectionString"

          }
        },
        {
          "name"  = "enableDeadLetter"
          "value" = "true"
        },
        {
          "name"  = "exchangeKind"
          "value" = "topic"
        },
        {
          "name"  = "deletedWhenUnused"
          "value" = "false"
        },
        {
          "name"  = "deliveryMode"
          "value" = "2"
        }
      ]
    }
  }
}

resource "kubernetes_manifest" "dapr_subscription_new_orders" {
  manifest = {
    "apiVersion" = "dapr.io/v1alpha1"
    "kind"       = "Subscription"
    "metadata" = {
      "name"      = "sub-new-orders"
      "namespace" = data.kubernetes_namespace.app.metadata[0].name
    }
    "spec" = {
      "pubsubname" = "orders"
      "topic"      = "new_orders"
      "route"      = "/orders"
    }
    "scopes" : ["shipping"]
  }
}

resource "kubernetes_manifest" "dapr_subscription_processed_orders" {
  manifest = {
    "apiVersion" = "dapr.io/v1alpha1"
    "kind"       = "Subscription"
    "metadata" = {
      "name"      = "sub-processed-orders"
      "namespace" = data.kubernetes_namespace.app.metadata[0].name
    }
    "spec" = {
      "pubsubname" = "orders"
      "topic"      = "processed_orders"
      "route"      = "/orders"
    }
    "scopes" : ["notification"]
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
