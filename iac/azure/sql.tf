resource "azurerm_mssql_server" "main" {
  name                         = "sql-cloud-native-sample-${terraform.workspace}"
  resource_group_name          = azurerm_resource_group.main.name
  location                     = var.location
  version                      = "12.0"
  administrator_login          = var.sql_server_admin_user
  administrator_login_password = var.sql_server_admin_password
  minimum_tls_version          = "1.2"
  tags                         = local.tags
}

resource "azurerm_mssql_database" "products" {
  name           = "cnproducts"
  server_id      = azurerm_mssql_server.main.id
  sku_name       = "S0"
  zone_redundant = false

  tags = local.tags
}
