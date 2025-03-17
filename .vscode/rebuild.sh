#!/bin/bash

# Clean the solution
dotnet clean
rm -rf IDC.Template/bin/*
rm -rf IDC.Utilities/bin/*

# Build the solution
dotnet build

# Create dependencies directory and copy files
mkdir -p IDC.Template/wwwroot/dependencies
cp -R IDC.Utilities/bin/Debug/net8.0/* IDC.Template/wwwroot/dependencies/

# Remove deps.json files
find IDC.Template/wwwroot/dependencies \
     {IDC.Template,IDC.Utilities}/bin/{Debug,Release}/net8.0 \
     -name "*.deps.json" -type f -delete 2>/dev/null || true

