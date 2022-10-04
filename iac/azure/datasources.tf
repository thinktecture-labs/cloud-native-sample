data "azurerm_client_config" "current" {}

data "azuread_client_config" "current" {}

data "azuread_user" "cw" {
  user_principal_name = "christian.weyer@thinktecture.com"
}

data "azuread_user" "thh" {
  user_principal_name = "thorsten.hans@thinktecture.com"
}

data "azurerm_kubernetes_service_versions" "latest" {
  location        = var.location
  include_preview = false
}
