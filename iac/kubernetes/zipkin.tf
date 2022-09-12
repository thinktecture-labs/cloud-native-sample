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

