# Clean the solution
dotnet clean

# Remove bin directories
Remove-Item -Path "IDC.Template/bin/*" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "IDC.Utilities/bin/*" -Recurse -Force -ErrorAction SilentlyContinue