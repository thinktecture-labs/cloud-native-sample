variable "location" {
  type        = string
  default     = "germanywestcentral"
  description = "Azure Region used to deploy all services to"
}

variable "custom_tags" {
  type        = map(string)
  default     = {}
  description = "Custom Azure Tags assigned to Azure resources"
}
