resource "helm_release" "rabbitmq" {
  name       = "rabbit"
  repository = "https://charts.bitnami.com/bitnami"
  chart      = "rabbitmq"
  timeout    = 600
  // this must be deployed to the same namespace as the the dapr components and
  // the workloads that use that particular dapr component
  namespace        = "rabbitmq"
  create_namespace = true

  set {
    name  = "auth.username"
    value = var.rabbitmq_user
  }

  set {
    name  = "auth.password"
    value = var.rabbitmq_password
  }

  lifecycle {
    ignore_changes = [
      set
    ]
  }
}

resource "kubernetes_secret" "rabbitmq" {
  metadata {
    name      = "rabbitmq"
    namespace = kubernetes_namespace.app.metadata[0].name
  }

  data = {
    connectionString = "amqp://${var.rabbitmq_user}:${var.rabbitmq_password}@${helm_release.rabbitmq.name}-${helm_release.rabbitmq.chart}.${helm_release.rabbitmq.metadata[0].namespace}.svc.cluster.local:5672"
  }
}
