

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
