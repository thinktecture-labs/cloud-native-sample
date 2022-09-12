// helm release for zipkin 

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

// helm release for grafana
resource "helm_release" "grafana" {
  name             = "cn-grafana"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "grafana"
  timeout          = 600
  namespace        = "grafana"
  create_namespace = true

  set {
    name  = "ingrss.enabled"
    value = "true"
  }

}

// helm release for prometheus
resource "helm_release" "prometheus" {
  name             = "prometheus"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "prometheus"
  namespace        = "prometheus"
  create_namespace = true
}

// helm release grafana loki
resource "helm_release" "grafana_loki" {
  name             = "loki"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "loki"
  namespace        = "loki"
  create_namespace = true

  set {
    name  = "loki.storage.type"
    value = "filesystem"
  }
}

// helm release grafana promtail
resource "helm_release" "grafana_promtail" {
  name             = "promtail"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "promtail"
  namespace        = "promtail"
  create_namespace = true

  set {
    name  = "config.lokiAddress"
    value = "xxx"
  }
}
