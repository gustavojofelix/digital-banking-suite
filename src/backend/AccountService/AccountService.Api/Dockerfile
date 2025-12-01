# ===========================
# 1. Build stage
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["AccountService.Api.csproj", "./"]
RUN dotnet restore "AccountService.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build the project
RUN dotnet build "AccountService.Api.csproj" -c Release -o /app/build

# ===========================
# 2. Publish stage
# ===========================
FROM build AS publish
RUN dotnet publish "AccountService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===========================
# 3. Runtime stage
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Set a non-root user if desired (good practice for production)
# USER app

COPY --from=publish /app/publish .
EXPOSE 8080

# ASP.NET Core defaults to port 8080 in some container scenarios;
# adjust ASPNETCORE_URLS if needed.
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "AccountService.Api.dll"]