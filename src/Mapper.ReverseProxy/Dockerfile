# See https://aka.ms/customizecontainer to learn how to customize the debug container and how Visual Studio uses this Dockerfile.

# This stage is used when running from Visual Studio in fast mode.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081

# This stage is used to build the service project.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["src/Mapper.ReverseProxy/Mapper.ReverseProxy.csproj", "."]
RUN dotnet restore "./Mapper.ReverseProxy.csproj"
COPY ["src/Mapper.ReverseProxy/", "."]
RUN dotnet build "./Mapper.ReverseProxy.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Mapper.ReverseProxy.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when Visual Studio fast mode is disabled.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mapper.ReverseProxy.dll"]
