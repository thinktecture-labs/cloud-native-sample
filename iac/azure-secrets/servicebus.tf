resource "kubernetes_secret" "servicebus" {
  metadata {
    name      = "az-servicebus"
    namespace = "cloud-native-sample"
  }
  data = {
    "coconnectionString" = var.azure_servicebus_connection_string
  }
}
