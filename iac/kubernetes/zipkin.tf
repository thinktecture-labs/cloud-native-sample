resource "kubernetes_namespace" "zipkin" {
  metadata {
    name = "zipkin"
  }
}

resource "kubernetes_deployment" "zipkin" {
  metadata {
    name      = "zipkin"
    namespace = kubernetes_namespace.zipkin.metadata[0].name
  }
  spec {
    replicas = 1
    selector {
      match_labels = {
        app = "zipkin"
      }
    }
    template {
      metadata {
        labels = {
          app = "zipkin"
        }
      }
      spec {
        container {
          name  = "zipkin"
          image = "openzipkin/zipkin"
          port {
            container_port = 9411
          }
        }
      }
    }
  }
}



resource "kubernetes_service" "zipkin" {
  metadata {
    name      = "zipkin"
    namespace = kubernetes_namespace.zipkin.metadata[0].name
  }
  spec {
    selector = {
      app = "zipkin"
    }
    port {
      port        = 9411
      target_port = 9411
    }
    type = "ClusterIP"
  }
}

resource "kubernetes_config_map_v1" "name" {
  metadata {
    name      = "zipkin"
    namespace = kubernetes_namespace.app.metadata[0].name
  }
  data = {
    "endpoint" = "http://${kubernetes_service.zipkin.metadata[0].name}.${kubernetes_service.zipkin.metadata[0].namespace}.svc.cluster.local:9411/api/v2/spans"
  }
}

locals {
  zipkin_endpoint = "http://${kubernetes_service.zipkin.metadata[0].name}.${kubernetes_service.zipkin.metadata[0].namespace}.svc.cluster.local:9411/"
}

resource "kubernetes_ingress_v1" "zipkin" {
  metadata {
    name      = "zipkin"
    namespace = kubernetes_namespace.zipkin.metadata[0].name
    annotations = {
      "cert-manager.io/cluster-issuer" = "letsencrypt-prod"
    }
  }
  spec {
    ingress_class_name = "nginx"
    tls {
      hosts       = ["cn-zipkin.thinktecture-demos.com"]
      secret_name = "zipkin-tls"
    }
    rule {
      host = "cn-zipkin.thinktecture-demos.com"
      http {
        path {
          path      = "/"
          path_type = "Prefix"
          backend {
            service {
              name = kubernetes_service.zipkin.metadata[0].name
              port {
                number = kubernetes_service.zipkin.spec[0].port[0].port
              }
            }
          }
        }
      }
    }
  }
}
