resource "azurerm_resource_group" "main" {
  name     = "rg-cloud-native-sample-${terraform.workspace}"
  location = var.location
  tags     = local.tags
}
