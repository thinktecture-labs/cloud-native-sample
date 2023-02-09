resource "azurerm_redis_cache" "redis" {
  name                = "redis-cloud-native-sample-${terraform.workspace}"
  resource_group_name = azurerm_resource_group.main.name
  location            = var.location
  capacity            = 1
  family              = "C"
  sku_name            = "Basic"
  tags                = local.tags
}
