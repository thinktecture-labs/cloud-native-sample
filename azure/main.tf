terraform {
  required_version = "~>1.2.5"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.14.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 2.15.0"
    }
  }
  backend "azurerm" {
    storage_account_name = "sattcnsample2022"
    container_name       = "iac"
    key                  = "cnsample.terraform.tfstate"
    use_azuread_auth     = true 
  }
}
provider "azurerm" {
  features {

  }
}

provider "azuread" { 
}

resource "azurerm_resource_group" "main" {
  name     = "rg-cloud-native-sample"
  location = var.location

  tags = local.tags
}

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
}

data "azurerm_client_config" "current" {}
data "azuread_client_config" "current" {}


resource "azuread_group" "k8s_admins" {
  display_name     = "Kubernetes Administrators (Cloud-Native Sample)"
  mail_enabled     = false
  mail_nickname    = "Kubernetes-Administrators"
  security_enabled = true
  types            = ["Unified"]

  owners = [
    data.azuread_client_config.current.object_id,
    data.azuread_user.cw.object_id,
    data.azuread_user.thh.object_id,
  ]

  behaviors = ["HideGroupInOutlook"]

  theme = "Teal"
}

data "azuread_user" "cw" {
  user_principal_name = "christian.weyer@thinktecture.com"
}

data "azuread_user" "thh" {
  user_principal_name = "thorsten.hans@thinktecture.com"
}
