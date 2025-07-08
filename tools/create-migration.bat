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
set DataProjectPath=%~dp0..\src\Infrastructure\ArturRios.UserManagement.Data
set MigrationsPath=%~dp0..\src\Infrastructure\ArturRios.UserManagement.Data\Migrations
set ScriptsPath=%~dp0..\migrations

:: Navigate to the Data project directory
cd /d "%DataProjectPath%"
if errorlevel 1 (
    echo Failed to navigate to the Data project directory.
    exit /b 1
)

:: Create the migration
echo Creating migration "%MigrationName%"...
dotnet ef migrations add %MigrationName% --output-dir %MigrationsPath%
if errorlevel 1 (
    echo Failed to create migration.
    exit /b 1
)

:: Get the full migration name (including timestamp)
for /f "tokens=1 delims= " %%i in ('dotnet ef migrations list ^| findstr /r /c:"%MigrationName%"') do set FullMigrationName=%%i

if "%FullMigrationName%"=="" (
    echo Failed to retrieve the full migration name.
    exit /b 1
)

:: Get the last migration
for /f "delims=" %%i in ('dotnet ef migrations list ^| findstr /r /v "No migrations were found" ^| tail -n 1') do set LastMigration=%%i

if "%LastMigration%"=="" (
    set LastMigration=0
)

:: Generate SQL script for the migration
echo Generating SQL script for migration...
dotnet ef migrations script %LastMigration% %MigrationName% --output %ScriptsPath%\%FullMigrationName%.sql
if errorlevel 1 (
    echo Failed to generate SQL script.
    exit /b 1
)

:: Generate rollback SQL script
echo Generating rollback SQL script...
dotnet ef migrations script %MigrationName% %LastMigration% --output %ScriptsPath%\%FullMigrationName%_Rollback.sql
if errorlevel 1 (
    echo Failed to generate rollback SQL script.
    exit /b 1
)

echo Migration and scripts created successfully.
exit /b 0