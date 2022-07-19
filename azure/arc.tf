resource "azurerm_container_registry" "main" {
  name                   = "ttcnsample"
  resource_group_name    = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
  admin_enabled          = false
  anonymous_pull_enabled = false
  sku                    = "Premium"
  tags                   = local.tags
}

resource "azurerm_monitor_diagnostic_setting" "diag_acr" {

  name                           = "diag-acr-${azurerm_container_registry.main.name}-${terraform.workspace}"
  log_analytics_workspace_id     = azurerm_log_analytics_workspace.main.id
  log_analytics_destination_type = "Dedicated"
  target_resource_id             = azurerm_container_registry.main.id

  log {
    category = "ContainerRegistryLoginEvents"
    enabled  = true
  }

  log {
    category = "ContainerRegistryRepositoryEvents"
    enabled  = true
  }

  metric {
    category = "AllMetrics"
    enabled  = true
  }
}


resource "azurerm_role_assignment" "push_thh" {
  scope                = azurerm_container_registry.main.id
  role_definition_name = "AcrPush"
  principal_id         = data.azuread_user.thh.object_id
}

resource "azurerm_role_assignment" "push_cw" {
  scope                = azurerm_container_registry.main.id
  role_definition_name = "AcrPush"
  principal_id         = data.azuread_user.cw.object_id
}

resource "azurerm_role_assignment" "pull_aks" {
  scope                = azurerm_container_registry.main.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_kubernetes_cluster.main.kubelet_identity[0].object_id
}
