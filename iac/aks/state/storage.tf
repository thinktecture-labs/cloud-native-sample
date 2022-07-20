
resource "azurerm_storage_account" "iac" {
  name                = "sattcnsample2022"
  resource_group_name = azurerm_resource_group.iac.name
  location            = azurerm_resource_group.iac.location

  account_kind             = "StorageV2"
  account_replication_type = "LRS"
  account_tier             = "Standard"

  is_hns_enabled = false
  blob_properties {
    change_feed_enabled = false
    versioning_enabled  = true
  }

}

resource "azurerm_storage_container" "iac" {
  name                  = "iac"
  storage_account_name  = azurerm_storage_account.iac.name
  container_access_type = "private"
}

resource "azurerm_role_assignment" "storage_contrib" {
  scope                = azurerm_storage_account.iac.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = data.azurerm_client_config.current.object_id
}
