package monitor

import (
	"encoding/json"
	"io/ioutil"
	"os"
	"strconv"
)

type Configuration struct {
	Port              int    `json:"port"`
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
	envEnvironmentName    = "OrderMonitorService__Environment"
	envZipkinEndpoint     = "OrderMonitorService__ZipkinEndpoint"
	envDisableConsoleLog  = "OrderMonitorService__DisableConsoleLog"
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
