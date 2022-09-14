

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

variable "dapr_config_sample_rate" {
  type    = number
  default = 1
}

variable "rabbitmq_user" {
  type    = string
  default = "cn"
}

variable "rabbitmq_password" {
  type      = string
  default   = "SomeSecretPassword"
  sensitive = true
}
