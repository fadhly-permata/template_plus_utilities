#!/bin/bash

# Build the solution
dotnet build

# Create dependencies directory if not exists
mkdir -p IDC.Template/wwwroot/dependencies

# Copy utilities to dependencies folder
cp -R IDC.Utilities/bin/Debug/net8.0/* IDC.Template/wwwroot/dependencies/

# Remove deps.json files
find IDC.Template/wwwroot/dependencies \
     {IDC.Template,IDC.Utilities}/bin/{Debug,Release}/net8.0 \
     -name "*.deps.json" -type f -delete 2>/dev/null || true
