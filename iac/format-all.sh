#! /bin/bash

# simple script to re-format all terraform sources
echo "Work, Work! 🚧"

echo " - 💄 Formatting top level module"
cd azure
terraform fmt
cd ..

cd state
echo " - 💄 Formatting state module"
terraform fmt
cd ..

cd kubernetes
echo " - 💄 Formatting Kubernetes module"
terraform fmt
cd ..

echo "All done! ✅"


