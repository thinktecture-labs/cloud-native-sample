terraform {
  required_version = "~>1.2.5"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.14.0"
    }
  }
}

resource "azurerm_dns_a_record" "dns" {
  name                = "cn"
  zone_name           = var.dns_zone_name
  resource_group_name = var.dns_zone_resource_group_name
  ttl                 = 300
  records             = [var.ingress_ip]
}
