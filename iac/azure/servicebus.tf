resource "azurerm_servicebus_namespace" "main" {
  name                = "sbn-cloud-native-sample"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "Standard"
  tags                = local.tags
}

resource "azurerm_servicebus_queue" "main" {
  name         = "queue-orders"
  namespace_id = azurerm_servicebus_namespace.main.id
}
