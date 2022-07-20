terraform {
  required_version = "~>1.2.5"
  required_providers {
    helm = {
      source  = "hashicorp/helm"
      version = ">= 2.6.0"
    }
  }
}

provider "helm" {
  kubernetes {
    host                   = var.kube_config.host
    client_certificate     = base64decode(var.kube_config.client_certificate)
    client_key             = base64decode(var.kube_config.client_key)
    cluster_ca_certificate = base64decode(var.kube_config.cluster_ca_certificate)
  }
}
