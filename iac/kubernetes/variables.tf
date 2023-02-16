

variable "dns_zone" {
  type = object({
    deploy_dns          = bool
    name                = string
    resource_group_name = string
  })

  default = {
    deploy_dns          = true
    name                = "thinktecture-demos.com"
    resource_group_name = "rg-research"
  }
}

variable "rabbitmq_user" {
  type    = string
  default = "cloudnativesample"
}

variable "rabbitmq_password" {
  type        = string
  default     = "Some_Secret_Password$"
  description = "Overwrite this default value using a GitHub Secret"
  sensitive   = true
}

variable "loki_storage_account_name" {
  type    = string
  default = "sattcnsampleloki"
}

variable "loki_storage_account_key" {
  type      = string
  sensitive = true
}

