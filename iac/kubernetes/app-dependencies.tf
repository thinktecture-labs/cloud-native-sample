// helm release for rabbitmq
resource "helm_release" "rabbitmq" {
  name       = "rabbit"
  repository = "https://charts.bitnami.com/bitnami"
  chart      = "rabbitmq"
  // this must be deployed to the same namespace as the the dapr components and
  // the workloads that use that particular dapr component
  namespace        = "rabbitmq"
  create_namespace = true
}

//helm release for dapr
resource "helm_release" "dapr" {
  name             = "dapr"
  repository       = "https://dapr.github.io/helm-charts/"
  chart            = "dapr"
  namespace        = "dapr-system"
  create_namespace = true
}


// dapr components must have explicit dependency on dapr helm release
// dapr helm release installs CRDs in the cluster