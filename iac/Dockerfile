FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /

COPY ../src src
COPY ../common common

WORKDIR /src/Application/ArturRios.UserManagement.WebApi
RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ArturRios.UserManagement.WebApi.dll"]
