# Hot reload enabled Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 80

# Copy project files
COPY . .

# Restore dependencies
RUN dotnet restore

# Set environment variables for hot reloading
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# Run the application with hot reloading
ENTRYPOINT ["dotnet", "watch", "run", "--no-launch-profile"]