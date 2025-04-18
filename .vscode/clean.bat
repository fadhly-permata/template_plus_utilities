@echo off

REM Clean the solution
dotnet clean

REM Remove bin directories
rd /s /q "IDC.Template\bin" 2>nul
rd /s /q "IDC.Utilities\bin" 2>nul