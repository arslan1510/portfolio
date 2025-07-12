# Use .NET 9 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set working directory
WORKDIR /app

# Copy project file and restore dependencies
COPY backend/project.fsproj ./
RUN dotnet restore project.fsproj

# Copy source code
COPY backend/ ./

# Build the application
RUN dotnet build project.fsproj -c Release -o /app/build

# Publish the application
RUN dotnet publish project.fsproj -c Release -o /app/publish

# Use .NET 9 runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set working directory
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Copy frontend files for static serving
COPY frontend/ ./frontend/

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Run the application
ENTRYPOINT ["dotnet", "Portfolio.Backend.dll"] 