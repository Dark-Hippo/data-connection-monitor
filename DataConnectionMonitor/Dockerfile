# Use the official Microsoft .NET Core runtime base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the image to '/app'
WORKDIR /app

# Copy the project file into the image
COPY *.csproj ./

# Restore the application dependencies
RUN dotnet restore

# Copy the rest of the application files into the image
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Start a new stage with the runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0

# Set the working directory in the image to '/app'
WORKDIR /app

# Copy the build output from the previous stage into this stage
COPY --from=build /app/out .

# Set the command that will be run when the container is started
ENTRYPOINT ["dotnet", "DataConnectionMonitor.dll"]