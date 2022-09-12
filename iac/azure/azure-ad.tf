resource "azuread_group" "k8s_admins" {
  display_name     = "Kubernetes Administrators (Cloud-Native Sample)"
  mail_enabled     = true
  mail_nickname    = "Kubernetes-Administrators-CNSample"
  security_enabled = true
  types            = ["Unified"]

  owners = [
    data.azuread_client_config.current.object_id,
    data.azuread_user.cw.object_id,
    data.azuread_user.thh.object_id,
  ]

  behaviors = ["HideGroupInOutlook"]
  theme     = "Teal"
  lifecycle {
    ignore_changes = [
      owners, members
    ]
  }
}
