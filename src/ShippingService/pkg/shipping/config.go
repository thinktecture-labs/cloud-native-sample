package shipping

import (
	"encoding/json"
	"io/ioutil"
)

func LoadConfiguration() (*ShippingServiceConfiguration, error) {
	c, err := ioutil.ReadFile("./config.json")
	if err != nil {
		return nil, err
	}
	var cfg ShippingServiceConfiguration
	if err = json.Unmarshal(c, &cfg); err != nil {
		return nil, err
	}
	return &cfg, err
}
