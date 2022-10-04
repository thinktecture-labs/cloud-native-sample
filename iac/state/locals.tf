locals {
  default_tags = {
    "com.thinktecture.project" = "Cloud-Native Sample"
    "com.thinktecture.owner"   = "thorsten.hans@thinktecture.com"
  }
  tags = merge(local.default_tags, var.custom_tags)
}
