# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /src/out .
ENTRYPOINT ["dotnet", "Az.dll"]  # Replace YourApiProjectName.dll with the name of your API's assembly
