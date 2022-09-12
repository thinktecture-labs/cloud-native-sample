resource "azurerm_container_registry" "main" {
  name                   = var.acr_name
  resource_group_name    = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
  admin_enabled          = false
  anonymous_pull_enabled = false
  sku                    = "Premium"
  tags                   = local.tags
}

data "azurerm_container_registry_scope_map" "map_push" {
  container_registry_name = azurerm_container_registry.main.name
  resource_group_name     = azurerm_resource_group.main.name
  name                    = "_repositories_push"
  depends_on = [
    azurerm_container_registry.main
  ]
}

resource "azurerm_container_registry_token" "gh" {
  container_registry_name = azurerm_container_registry.main.name
  resource_group_name     = azurerm_resource_group.main.name
  scope_map_id            = data.azurerm_container_registry_scope_map.map_push.id
  name                    = var.acr_token_name
  enabled                 = true
  depends_on = [
    data.azurerm_container_registry_scope_map.map_push
  ]
}

resource "azurerm_monitor_diagnostic_setting" "diag_acr" {

  name                       = "diag-acr-${azurerm_container_registry.main.name}-${terraform.workspace}"
  log_analytics_workspace_id = azurerm_log_analytics_workspace.main.id
  target_resource_id         = azurerm_container_registry.main.id

  log {
    category = "ContainerRegistryLoginEvents"
    enabled  = true
    retention_policy {
      days    = 0
      enabled = false
    }
  }

  log {
    category = "ContainerRegistryRepositoryEvents"
    enabled  = true
    retention_policy {
      days    = 90
      enabled = true
    }
  }

  metric {
    category = "AllMetrics"
    enabled  = true
    retention_policy {
      days    = 90
      enabled = true
    }
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
