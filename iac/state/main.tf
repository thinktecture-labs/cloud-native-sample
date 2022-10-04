terraform {
  required_version = "~>1.2.8"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.21.0"
    }
  }
}
provider "azurerm" {
  features {

  }
}
data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "iac" {
  name     = "rg-cloud-native-sample-iac"
  location = var.location

  tags = local.tags
}

