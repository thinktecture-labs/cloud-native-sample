#! /bin/bash

# simple script to re-format all terraform sources
echo "Work, Work! 🚧"

echo " - 💄 Formatting top level module"
terraform fmt

cd state
echo " - 💄 Formatting state module"
terraform fmt
cd ..
cd modules

cd k8s
echo " - 💄 Formatting Kubernetes module"
terraform fmt
cd ..

cd dns
echo " - 💄 Formatting DNS module"
terraform fmt
cd ..
cd ..

echo "All done! ✅"


