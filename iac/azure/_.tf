terraform {
  required_version = "~>1.3.1"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.21.0"
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
