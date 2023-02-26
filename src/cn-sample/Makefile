.PHONY: build

build:
	@echo "Building..."
	@mage -goos windows -goarch amd64 -compile ../../tools/win_amd64/cn-sample.exe
	@mage -goos windows -goarch arm64 -compile ../../tools/win_arm64/cn-sample.exe
	@mage -goos darwin -goarch amd64 -compile ../../tools/darwin_amd64/cn-sample
	@mage -goos darwin -goarch arm64 -compile ../../tools/darwin_arm64/cn-sample
	@mage -goos linux -goarch amd64 -compile ../../tools/linux_amd64/cn-sample
