@echo off
:: Windows batch wrapper for gototag PowerShell script
:: This script is designed to be placed in your Unity project's root folder.

:: Pass all arguments to the PowerShell script
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0gototag.ps1" %*


