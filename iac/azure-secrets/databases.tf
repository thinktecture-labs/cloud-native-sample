resource "kubernetes_secret" "database" {
  metadata {
    name      = "databases"
    namespace = "cloud-native-sample"
  }

  data = {
    products = "Server=tcp:${var.sql_server_fqdn},1433;Database=cnproducts;User ID=${var.sql_username};Password=${var.sql_password};Trusted_Connection=False;Encrypt=True;MultipleActiveResultSets=True;"
  }

}
