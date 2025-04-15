@echo off
setlocal

:: Check if a migration name is provided
if "%~1"=="" (
    echo Error: Migration name is required.
    echo Usage: %~nx0 [MigrationName]
    exit /b 1
)

:: Set variables
set MigrationName=%~1
set DataProjectPath=%~dp0..\src\Infrastructure\TechCraftsmen.Management.User.Data
set MigrationsPath=%~dp0..\migrations

:: Navigate to the Data project directory
cd /d "%DataProjectPath%"
if errorlevel 1 (
    echo Failed to navigate to the Data project directory.
    exit /b 1
)

:: Get the last migration
for /f "delims=" %%i in ('dotnet ef migrations list ^| findstr /r /v "No migrations were found" ^| tail -n 1') do set LastMigration=%%i

if "%LastMigration%"=="" (
    set LastMigration=0
)

:: Generate SQL script for the migration
echo Generating SQL script for migration "%MigrationName%"...
dotnet ef migrations script %LastMigration% %MigrationName% --output %MigrationsPath%\%MigrationName%.sql
if errorlevel 1 (
    echo Failed to generate SQL script.
    exit /b 1
)

:: Generate rollback SQL script
echo Generating rollback SQL script for migration "%MigrationName%"...
dotnet ef migrations script %MigrationName% %LastMigration% --output %MigrationsPath%\%MigrationName%_Rollback.sql
if errorlevel 1 (
    echo Failed to generate rollback SQL script.
    exit /b 1
)

echo Scripts generated successfully.
exit /b 0