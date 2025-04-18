# Clean the solution
dotnet clean
Remove-Item -Path "IDC.Template/bin/*" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "IDC.Utilities/bin/*" -Recurse -Force -ErrorAction SilentlyContinue

# Build the solution
dotnet build

# Create dependencies directory and copy files
New-Item -Path "IDC.Template/wwwroot/dependencies" -ItemType Directory -Force
Copy-Item -Path "IDC.Utilities/bin/Debug/net8.0/*" -Destination "IDC.Template/wwwroot/dependencies/" -Recurse -Force

# Remove deps.json files
Get-ChildItem -Path @(
    "IDC.Template/wwwroot/dependencies",
    "IDC.Template/bin/Debug/net8.0",
    "IDC.Template/bin/Release/net8.0",
    "IDC.Utilities/bin/Debug/net8.0",
    "IDC.Utilities/bin/Release/net8.0"
) -Filter "*.deps.json" -Recurse | Remove-Item -Force -ErrorAction SilentlyContinue