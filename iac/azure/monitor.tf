resource "azurerm_log_analytics_workspace" "main" {
  name                = "law-cloud-native-sample"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  daily_quota_gb    = 2
  retention_in_days = 90
  sku               = "PerGB2018"
  tags              = local.tags
}

resource "azurerm_application_insights" "main" {
  name                = "appinsights-cloud-native-sample"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  workspace_id        = azurerm_log_analytics_workspace.main.id
  application_type    = "web"
  tags                = local.tags
}
