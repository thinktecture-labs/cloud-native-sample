//go:build mage

// Cloud-Native Sample Developer CLI
package main

import (
	"os"

	"github.com/magefile/mage/sh"
)

// 📦 Initialize your local machine to run the cloud-native sample
func Init() {
	sh.Run("docker", "plugin", "install", "grafana/loki-docker-driver:latest --alias loki --grant-all-permissions")
}

// 🚀 Start the cloud-native sample
func Start() {
	sh.Run("docker-compose up --build -d")
}

// ⚡️ Quickstart the cloud-native sample
func Quickstart() {
	sh.Run("docker-compose up -d")
}

// 🛑 Stop the cloud-native sample
func Stop() {
	sh.Run("docker-compose down")
}

// 📝 View the logs of the cloud-native sample
func Logs() {
	sh.Exec(make(map[string]string), os.Stdout, os.Stderr, "docker-compose", "logs", "-f")
}

// 🧹 Clean-up your loacl machine
func CleanUp() {
	sh.Run("docker-compose down --rmi all --volumes --remove-orphans")
}
