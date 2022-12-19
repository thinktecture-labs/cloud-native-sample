package shipping

import (
	"encoding/json"
	"io/ioutil"
	"os"
)

const (
	configFilePath     = "./config.json"
	envPubSubName      = "ShippingService__PubSubName"
	envTopicName       = "ShippingService__TopicName"
	envEnvironmentName = "ShippingService__Environment"
	envZipkinEndpoint  = "ShippingService__ZipkinEndpoint"
)

func LoadConfiguration() (*Configuration, error) {
	cfg, err := loadConfigurationFromConfigFile()
	if err != nil {
		return nil, err
	}
	mergeEnvironmentVariables(cfg)
	return cfg, nil
}

func mergeEnvironmentVariables(cfg *Configuration) {
	if v, ok := os.LookupEnv(envPubSubName); ok {
		cfg.PubSubName = v
	}
	if v, ok := os.LookupEnv(envTopicName); ok {
		cfg.TopicName = v
	}
	if v, ok := os.LookupEnv(envEnvironmentName); ok {
		cfg.Environment = v
	}
	if v, ok := os.LookupEnv(envZipkinEndpoint); ok {
		cfg.ZipkinEndpoint = v
	}
}

func loadConfigurationFromConfigFile() (*Configuration, error) {
	c, err := ioutil.ReadFile(configFilePath)
	if err != nil {
		return nil, err
	}
	var cfg Configuration
	if err = json.Unmarshal(c, &cfg); err != nil {
		return nil, err
	}
	return &cfg, err
}
