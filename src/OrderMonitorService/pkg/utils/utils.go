package utils

type ContextKey string

const (
	AuthKey        = ContextKey("Authorization")
	BackendTimeOut = ContextKey("BackendTimeOut")
)
