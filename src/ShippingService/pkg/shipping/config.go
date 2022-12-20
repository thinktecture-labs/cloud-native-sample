package shipping

import (
	"encoding/json"
	"io/ioutil"
	"os"
	"strconv"
)

type Configuration struct {
	Port              int    `json:"port"`
	SourcePubSubName  string `json:"sourcePubSubName"`
	SourceTopicName   string `json:"sourceTopicName"`
	TargetPubSubName  string `json:"targetPubSubName"`
	TargetTopicName   string `json:"targetTopicName"`
	Environment       string `json:"environment"`
	ZipkinEndpoint    string `json:"zipkinEndpoint"`
	DisableConsoleLog bool   `json:"disableConsoleLog"`
}

func (c *Configuration) IsProduction() bool {
	return c.Environment == environmentProduction
}

const (
	defaultPort           = 5000
	envPort               = "PORT"
	environmentProduction = "Production"
	configFilePath        = "./config.json"
	envSourcePubSubName   = "ShippingService__SourcePubSubName"
	envSourceTopicName    = "ShippingService__SourceTopicName"
	envTargetPubSubName   = "ShippingService__TargetPubSubName"
	envTargetTopicName    = "ShippingService_TargetTopicName"
	envEnvironmentName    = "ShippingService__Environment"
	envZipkinEndpoint     = "ShippingService__ZipkinEndpoint"
	envDisableConsoleLog  = "ShippingService__DisableConsoleLog"
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
	if v, ok := os.LookupEnv(envPort); ok {
		if len(v) == 0 {
			cfg.Port = defaultPort
		}
		port, err := strconv.Atoi(v)
		if err != nil {
			cfg.Port = defaultPort
		}
		cfg.Port = port
	}
	if v, ok := os.LookupEnv(envDisableConsoleLog); ok {
		if len(v) == 0 {
			cfg.DisableConsoleLog = false
		}
		disable, err := strconv.ParseBool(v)
		if err != nil {
			cfg.DisableConsoleLog = false
		}
		cfg.DisableConsoleLog = disable
	}
	if v, ok := os.LookupEnv(envSourcePubSubName); ok {
		cfg.SourcePubSubName = v
	}
	if v, ok := os.LookupEnv(envSourceTopicName); ok {
		cfg.SourceTopicName = v
	}
	if v, ok := os.LookupEnv(envTargetPubSubName); ok {
		cfg.TargetPubSubName = v
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
