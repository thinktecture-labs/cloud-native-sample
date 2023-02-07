//go:build mage

// Cloud-Native Sample Developer CLI
package main

import (
	"fmt"
	"os"

	"github.com/magefile/mage/mg"
	"github.com/magefile/mage/sh"
)

// 📦 Initialize your local machine to run the cloud-native sample
func Init() {
	sh.Run("docker", "plugin", "install", "grafana/loki-docker-driver:latest --alias loki --grant-all-permissions")
}

// 🚀 Start the cloud-native sample
func Start() {
	mg.Deps(guard)
	sh.Run("docker-compose up -d")
}

// ⚡️ Quickstart the cloud-native sample
func Quickstart() {
	mg.Deps(guard)
	sh.Run("docker-compose up -d")
}

// 🛑 Stop the cloud-native sample
func Stop() {
	mg.Deps(guard)
	sh.Run("docker-compose down")
}

// 📝 View the logs of the cloud-native sample
func Logs() {
	mg.Deps(guard)
	sh.Exec(make(map[string]string), os.Stdout, os.Stderr, "docker-compose logs -f")
}

// 🧹 Clean-up your loacl machine
func CleanUp() {
	mg.Deps(guard)
	sh.Run("docker-compose down --rmi all --volumes --remove-orphans")
}

func guard() {
	// check if docker-compose.yaml exists
	_, err := os.Stat("docker-compose.yml")

	if os.IsNotExist(err) {
		fmt.Println("docker-compose.yml not found.")
		fmt.Println("Run cn-sample CLI in project root directory.")
		os.Exit(1)
	} else if err != nil {
		fmt.Println("Error: ", err)
		os.Exit(1)
	}
}
