#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Shortify.NET.API/Shortify.NET.API.csproj", "Shortify.NET.API/"]
COPY ["Shortify.NET.Application/Shortify.NET.Application.csproj", "Shortify.NET.Application/"]
COPY ["Shortify.NET.Core/Shortify.NET.Core.csproj", "Shortify.NET.Core/"]
COPY ["Shortify.NET.Common/Shortify.NET.Common.csproj", "Shortify.NET.Common/"]
COPY ["Shortify.NET.Infrastructure/Shortify.NET.Infrastructure.csproj", "Shortify.NET.Infrastructure/"]
COPY ["Shortify.NET.Persistence/Shortify.NET.Persistence.csproj", "Shortify.NET.Persistence/"]
RUN dotnet restore "./Shortify.NET.API/Shortify.NET.API.csproj"
COPY . .
WORKDIR "/src/Shortify.NET.API"
RUN dotnet build "./Shortify.NET.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Shortify.NET.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shortify.NET.API.dll"]