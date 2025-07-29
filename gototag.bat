@echo off
:: Windows batch wrapper for gototag PowerShell script
:: This script is designed to be placed in your Unity project's root folder.

:: Get the tag name from the first argument
set TAG=%1

:: Check if tag name was provided
if "%TAG%"=="" (
    echo Error: No tag name provided.
    echo Usage: gototag ^<tag-name^>
    exit /b 1
)

:: Call PowerShell script with ExecutionPolicy Bypass
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0gototag.ps1" %TAG%

