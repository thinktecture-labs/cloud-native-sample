resource "azurerm_kubernetes_cluster" "main" {
  name                = "aks-cloud-native-sample"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  dns_prefix          = "cnsample"

  identity {
    type = "SystemAssigned"
  }

  default_node_pool {
    name                = "default"
    enable_auto_scaling = true
    min_count           = 1
    max_count           = 3
    vm_size             = "Standard_DS4_v2"
  }

  azure_active_directory_role_based_access_control {
    managed                = true
    azure_rbac_enabled     = true
    tenant_id              = data.azurerm_client_config.current.tenant_id
    admin_group_object_ids = [azuread_group.k8s_admins.object_id]
  }

  oms_agent {
    log_analytics_workspace_id = azurerm_log_analytics_workspace.main.workspace_id
  }

  kubernetes_version = "1.23.8"
  sku_tier           = "Free"

  tags = local.tags
}
