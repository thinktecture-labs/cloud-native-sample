variable "dapr_config_sample_rate" {
  type    = number
  default = 1
}

variable "dapr_api_auth_token" {
  type      = string
  sensitive = true
}
variable "app_namespace" {
  type    = string
  default = "cloud-native-sample"
}


variable "zipkin_config" {
  type = object({
    name      = string
    namespace = string
  })
  default = {
    name      = "zipkin"
    namespace = "zipkin"
  }
}

variable "message_broker" {
  type    = string
  default = "azservicebus"
  validation {
    condition     = contains(["azservicebus", "rabbitmq"], var.message_broker)
    error_message = "Message broker must be either azservicebus or rabbitmq"
  }
}
