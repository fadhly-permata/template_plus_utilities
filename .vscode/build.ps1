# Build the solution
dotnet build

# Create dependencies directory if not exists
New-Item -Path "IDC.Template/wwwroot/dependencies" -ItemType Directory -Force

# Copy utilities to dependencies folder
Copy-Item -Path "IDC.Utilities/bin/Debug/net8.0/*" -Destination "IDC.Template/wwwroot/dependencies/" -Recurse -Force

# Remove deps.json files
Get-ChildItem -Path @(
    "IDC.Template/wwwroot/dependencies",
    "IDC.Template/bin/Debug/net8.0",
    "IDC.Template/bin/Release/net8.0",
    "IDC.Utilities/bin/Debug/net8.0",
    "IDC.Utilities/bin/Release/net8.0"
) -Filter "*.deps.json" -Recurse | Remove-Item -Force -ErrorAction SilentlyContinue