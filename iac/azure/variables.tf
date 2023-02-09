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

variable "acr_name" {
  type        = string
  description = "Desired ACR instance name"
}

variable "acr_token_name" {
  type        = string
  description = "Desired ACR token name"
}

variable "sql_server_admin_user" {
  type        = string
  default     = "cnadmin"
  description = "Username for the SQL Server admin user"
}

variable "sql_server_admin_password" {
  type        = string
  sensitive   = true
  description = "Password for the SQL Server admin user"
}
