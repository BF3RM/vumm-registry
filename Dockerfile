# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /source

# copy csproj and restore as distinct layers
COPY VUModManagerRegistry/*.csproj ./
RUN dotnet restore

FROM builder AS build
# copy everything else and build app
WORKDIR /source
COPY VUModManagerRegistry/. ./
RUN dotnet publish -c Release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "VUModManagerRegistry.dll"]