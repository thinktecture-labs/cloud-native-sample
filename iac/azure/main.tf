resource "azurerm_resource_group" "main" {
  name     = "rg-cloud-native-sample"
  location = var.location

  tags = local.tags
}
