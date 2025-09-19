# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Products.Web/Products.Web.csproj", "src/Products.Web/"]
COPY ["src/Products.Application/Products.Application.csproj", "src/Products.Application/"]
COPY ["src/Products.Application.Contracts/Products.Application.Contracts.csproj", "src/Products.Application.Contracts/"]
COPY ["src/Products.Domain/Products.Domain.csproj", "src/Products.Domain/"]
COPY ["src/Products.Domain.Shared/Products.Domain.Shared.csproj", "src/Products.Domain.Shared/"]
COPY ["src/Products.EntityFrameworkCore/Products.EntityFrameworkCore.csproj", "src/Products.EntityFrameworkCore/"]
COPY ["src/Products.HttpApi/Products.HttpApi.csproj", "src/Products.HttpApi/"]
COPY ["src/Products.HttpApi.Client/Products.HttpApi.Client.csproj", "src/Products.HttpApi.Client/"]
COPY ["common.props", "."]
COPY ["NuGet.Config", "."]

# Restore dependencies
RUN dotnet restore "src/Products.Web/Products.Web.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/Products.Web"
RUN dotnet build "Products.Web.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Products.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 9.0 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install necessary packages
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Expose the port the app runs on
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "Products.Web.dll"]