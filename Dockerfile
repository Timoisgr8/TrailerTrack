# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore
COPY TrailerTrack.Domain/TrailerTrack.Domain.csproj TrailerTrack.Domain/
COPY TrailerTrack.Application/TrailerTrack.Application.csproj TrailerTrack.Application/
COPY TrailerTrack.Infrastructure/TrailerTrack.Infrastructure.csproj TrailerTrack.Infrastructure/
COPY TrailerTrack.Web/TrailerTrack.Web.csproj TrailerTrack.Web/

RUN dotnet restore TrailerTrack.Web/TrailerTrack.Web.csproj

# Copy everything and build
COPY . .
RUN dotnet publish TrailerTrack.Web/TrailerTrack.Web.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TrailerTrack.Web.dll"]