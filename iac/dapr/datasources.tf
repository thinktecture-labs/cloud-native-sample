data "kubernetes_namespace" "app" {
  metadata {
    name = var.app_namespace
  }
}

data "kubernetes_service_v1" "zipkin" {
  metadata {
    name      = var.zipkin_config.name
    namespace = var.zipkin_config.namespace
  }
}
