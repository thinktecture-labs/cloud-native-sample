resource "kubernetes_namespace" "app" {

  metadata {
    name = "cloud-native-sample"
  }
}
