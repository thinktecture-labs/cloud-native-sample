resource "azurerm_dns_a_record" "cn" {
  count               = var.dns_zone.deploy_dns ? 1 : 0
  name                = "cn"
  zone_name           = var.dns_zone.name
  resource_group_name = var.dns_zone.resource_group_name
  ttl                 = 300
  records             = [data.kubernetes_service.ing.status.0.load_balancer.0.ingress.0.ip]
}

resource "azurerm_dns_a_record" "grafana" {
  count               = var.dns_zone.deploy_dns ? 1 : 0
  name                = "cn-grafana"
  zone_name           = var.dns_zone.name
  resource_group_name = var.dns_zone.resource_group_name
  ttl                 = 300
  records             = [data.kubernetes_service.ing.status.0.load_balancer.0.ingress.0.ip]
}

resource "azurerm_dns_a_record" "zipkin" {
  count               = var.dns_zone.deploy_dns ? 1 : 0
  name                = "cn-zipkin"
  zone_name           = var.dns_zone.name
  resource_group_name = var.dns_zone.resource_group_name
  ttl                 = 300
  records             = [data.kubernetes_service.ing.status.0.load_balancer.0.ingress.0.ip]
}

resource "azurerm_dns_a_record" "dapr_dashboard" {
  count               = var.dns_zone.deploy_dns ? 1 : 0
  name                = "cn-dapr"
  zone_name           = var.dns_zone.name
  resource_group_name = var.dns_zone.resource_group_name
  ttl                 = 300
  records             = [data.kubernetes_service.ing.status.0.load_balancer.0.ingress.0.ip]
}

resource "azurerm_dns_a_record" "fakesmtp" {
  count               = var.dns_zone.deploy_dns ? 1 : 0
  name                = "cn-fakesmtp"
  zone_name           = var.dns_zone.name
  resource_group_name = var.dns_zone.resource_group_name
  ttl                 = 300
  records             = [data.kubernetes_service.ing.status.0.load_balancer.0.ingress.0.ip]
}

resource "azurerm_dns_a_record" "rabbitmq_management" {
  count               = var.dns_zone.deploy_dns ? 1 : 0
  name                = "cn-rabbitmq"
  zone_name           = var.dns_zone.name
  resource_group_name = var.dns_zone.resource_group_name
  ttl                 = 300
  records             = [data.kubernetes_service.ing.status.0.load_balancer.0.ingress.0.ip]
}
