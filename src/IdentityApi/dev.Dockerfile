FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine
WORKDIR /app
COPY ["src/eShop.ServiceDefaults", "src/eShop.ServiceDefaults"]
COPY ["src/IdentityApi", "src/IdentityApi"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]

# For this project
WORKDIR /app/src/IdentityApi

# Build app for devs
EXPOSE 5241
ENTRYPOINT ["dotnet", "watch"]