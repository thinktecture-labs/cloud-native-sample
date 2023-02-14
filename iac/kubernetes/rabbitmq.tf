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

  set {
    name  = "ingress.enabled"
    value = "true"
  }

  set {
    name  = "ingress.ingressClassName"
    value = "nginx"
  }

  set {
    name  = "ingress.hostname"
    value = "cn-rabbitmq.thinktecture-demos.com"
  }

  set {
    name  = "ingress.tls"
    value = "true"
  }

  set {
    name  = "ingress.annotations.cert-manager\\.io/cluster-issuer"
    value = "letsencrypt-prod"
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
