terraform {
  required_version = "~>1.3.1"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.21.0"
    }
    helm = {
      source  = "hashicorp/helm"
      version = ">= 2.6.0"
    }

    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = ">= 2.12.1"
    }
  }
  backend "azurerm" {
    storage_account_name = "sattcnsample2022"
    container_name       = "iac"
    key                  = "dapr.terraform.tfstate"
    use_azuread_auth     = true
  }
}

provider "azurerm" {
  features {}
}

provider "helm" {
  kubernetes {
  }
}

provider "kubernetes" {
}
