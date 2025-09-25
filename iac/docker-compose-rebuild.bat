@echo off

echo Removing database volume...
docker volume rm user_management_postgres_data

echo Removing Docker images...
docker image rm postgres
docker image rm user_management_webapi

echo Rebuilding Docker images and putting the online...
docker compose up -d

echo Done
