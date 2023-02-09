terraform {
  required_version = "~>1.3.1"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.21.0"
    }

    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = ">= 2.12.1"
    }
  }
  backend "azurerm" {
    storage_account_name = "sattcnsample2022"
    container_name       = "iac"
    key                  = "az-secrets.terraform.tfstate"
    use_azuread_auth     = true
  }
}

provider "azurerm" {
  features {}
}

provider "kubernetes" {
}
