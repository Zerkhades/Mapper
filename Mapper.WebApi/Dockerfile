#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
###################################################################

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj-files for all projects
COPY ["Mapper.WebApi/Mapper.WebApi.csproj", "Mapper.WebApi/"]
COPY ["Mapper.Application/Mapper.Application.csproj", "Mapper.Application/"]
COPY ["Mapper.Domain/Mapper.Domain.csproj", "Mapper.Domain/"]
COPY ["Mapper.Persistence/Mapper.Persistence.csproj", "Mapper.Persistence/"]

# Restoring dependencies
RUN dotnet restore "Mapper.WebApi/Mapper.WebApi.csproj"

# Copy all files
COPY . .

# Сборка проекта
WORKDIR "/src/Mapper.WebApi"
RUN dotnet build "Mapper.WebApi.csproj" -c Release -o /app/build

# Project publishing
FROM build AS publish
RUN dotnet publish "Mapper.WebApi.csproj" -c Release -o /app/publish

# Final deploy
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mapper.WebApi.dll"]