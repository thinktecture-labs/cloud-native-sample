#! /bin/bash

# simple script to validate all terraform sources
echo "Work, Work! 🚧"

echo " - 🔎 Validating top level module"
cd azure
terraform validate
cd ..

cd state
echo " - 🔎 Validating state module"
terraform validate
cd ..

cd kubernetes
echo " - 🔎 Validating Kubernetes module"
terraform validate
cd ..

echo "All done! ✅"


