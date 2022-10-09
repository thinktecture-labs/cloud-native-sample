variable "dapr_config_sample_rate" {
  type    = number
  default = 1
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
