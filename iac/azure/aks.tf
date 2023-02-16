resource "azurerm_kubernetes_cluster" "main" {
  name                = "aks-cloud-native-sample"
  resource_group_name = azurerm_resource_group.main.name
  node_resource_group = "${azurerm_resource_group.main.name}-nrg"
  location            = azurerm_resource_group.main.location
  dns_prefix          = "cnsample"
  kubernetes_version  = "1.24.6"
  sku_tier            = "Free"

  identity {
    type = "SystemAssigned"
  }

  default_node_pool {
    name                = "default"
    enable_auto_scaling = true
    min_count           = 2
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
    log_analytics_workspace_id = azurerm_log_analytics_workspace.main.id
  }

  tags = local.tags
}

resource "azurerm_role_assignment" "aks_cluster_admin" {
  scope                = azurerm_kubernetes_cluster.main.id
  role_definition_name = "Azure Kubernetes Service RBAC Cluster Admin"
  principal_id         = data.azurerm_client_config.current.object_id
}


locals {
  namespaces = ["default", "kube-system", "cloud-native-sample", "ingress"]
}

resource "azurerm_role_assignment" "aks_reader" {
  for_each             = toset(local.namespaces)
  scope                = "${azurerm_kubernetes_cluster.main.id}/namespaces/${each.key}"
  role_definition_name = "Azure Kubernetes Service RBAC Reader"
  principal_id         = azuread_group.k8s_admins.object_id
}

resource "azurerm_storage_account" "loki" {
  name                     = "sattcnsampleloki"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "StorageV2"
  tags                     = local.tags
}

resource "azurerm_storage_container" "loki" {
  name                  = "loki"
  storage_account_name  = azurerm_storage_account.loki.name
  container_access_type = "private"
}
