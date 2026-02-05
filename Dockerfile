# Use .NET 6 SDK for build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy backend project files
COPY backend/*.csproj ./backend/
WORKDIR /app/backend
RUN dotnet restore

# Copy the rest of the backend code
WORKDIR /app
COPY backend/ ./backend/
WORKDIR /app/backend
RUN dotnet publish -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/backend/out .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
ENTRYPOINT ["dotnet", "InvitacionesAPI.dll"]
