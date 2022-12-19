package shipping

import (
	"encoding/json"
	"io/ioutil"
	"os"
)

type Configuration struct {
	PubSubName      string
	SourceTopicName string
	TargetTopicName string
	Environment     string
	ZipkinEndpoint  string
}

func (c *Configuration) IsProduction() bool {
	return c.Environment == environmentProduction
}

const (
	environmentProduction = "Production"
	configFilePath        = "./config.json"
	envPubSubName         = "ShippingService__PubSubName"
	envSourceTopicName    = "ShippingService__SourceTopicName"
	envTargetTopicName    = "ShippingService_TargetTopicName"
	envEnvironmentName    = "ShippingService__Environment"
	envZipkinEndpoint     = "ShippingService__ZipkinEndpoint"
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
	if v, ok := os.LookupEnv(envSourceTopicName); ok {
		cfg.SourceTopicName = v
	}
	if v, ok := os.LookupEnv(envTargetTopicName); ok {
		cfg.TargetTopicName = v
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
