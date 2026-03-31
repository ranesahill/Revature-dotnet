# ── Build Stage ──────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore
COPY IMS.Core/IMS.Core.csproj IMS.Core/
COPY IMS.App/IMS.App.csproj IMS.App/
COPY IMS.Infrastructure/IMS.Infrastructure.csproj IMS.Infrastructure/
COPY IMS.API/IMS.API.csproj IMS.API/
RUN dotnet restore IMS.API/IMS.API.csproj

# Copy source and build
COPY . .
RUN dotnet publish IMS.API/IMS.API.csproj -c Release -o /app/publish --no-restore

# ── Runtime Stage ────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "IMS.API.dll"]
