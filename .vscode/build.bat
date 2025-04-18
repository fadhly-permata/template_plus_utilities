@echo off

REM Build the solution
dotnet build

REM Create dependencies directory if not exists
if not exist "IDC.Template\wwwroot\dependencies" mkdir "IDC.Template\wwwroot\dependencies"

REM Copy utilities to dependencies folder
xcopy /Y /E /I "IDC.Utilities\bin\Debug\net8.0\*" "IDC.Template\wwwroot\dependencies\"

REM Remove deps.json files
for %%G in (
    "IDC.Template\wwwroot\dependencies"
    "IDC.Template\bin\Debug\net8.0"
    "IDC.Template\bin\Release\net8.0"
    "IDC.Utilities\bin\Debug\net8.0"
    "IDC.Utilities\bin\Release\net8.0"
) do (
    if exist "%%~G" del /F /S /Q "%%~G\*.deps.json" 2>nul
)