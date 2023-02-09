variable "sql_server_fqdn" {
  type        = string
  description = "The FQDN of the SQL Server"
}

variable "sql_username" {
  type        = string
  description = "The username for the SQL Server"
}

variable "sql_password" {
  type        = string
  sensitive   = true
  description = "The password for the SQL Server"
}

variable "azure_servicebus_connection_string" {
  type        = string
  sensitive   = true
  description = "The connection string for the Azure Service Bus"
}
