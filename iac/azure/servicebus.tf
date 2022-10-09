resource "azurerm_servicebus_namespace" "main" {
  name                = "sbn-cloud-native-sample"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "Standard"
  tags                = local.tags
}


resource "azurerm_servicebus_topic" "new_orders" {
  name         = "new_orders"
  namespace_id = azurerm_servicebus_namespace.main.id
}

resource "azurerm_servicebus_topic" "processed_orders" {
  name         = "processed_orders"
  namespace_id = azurerm_servicebus_namespace.main.id
}

resource "azurerm_servicebus_topic" "notifications" {
  name         = "notifications"
  namespace_id = azurerm_servicebus_namespace.main.id
}
