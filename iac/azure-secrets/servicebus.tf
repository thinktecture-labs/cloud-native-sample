resource "kubernetes_secret" "servicebus" {
  metadata {
    name      = "az-servicebus"
    namespace = "cloud-native-sample"
  }
  data = {
    "connectionString" = var.azure_servicebus_connection_string
  }
}
