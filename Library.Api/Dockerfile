FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
ARG APP_UID=1000
WORKDIR /app
EXPOSE 5100
EXPOSE 8080

# Set environment variables for MSSQL connection
ENV ConnectionStrings:SqlServerConnection="Server=db; Database=LibraryDb; User=sa; Password=DB_Password; TrustServerCertificate=true;"

# Set environment variables for Redis connection and cache settings
ENV Cache:Redis="library.cache:6379"
ENV Cache:BookImageKeyPrefix="book-img-"
ENV Cache:MimeKeyPrefix="mime-img-"
ENV Cache:CacheLifeTimeMinutes=60

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Library.Api/Library.Api.csproj", "Library.Api/"]
COPY ["Library.Application/Library.Application.csproj", "Library.Application/"]
COPY ["Library.Contracts/Library.Contracts.csproj", "Library.Contracts/"]
COPY ["Library.Data/Library.Data.csproj", "Library.Data/"]
RUN dotnet restore "Library.Api/Library.Api.csproj"
COPY . .
WORKDIR "/src/Library.Api"
RUN dotnet build "./Library.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Library.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


WORKDIR /app
COPY --from=publish /app/publish .

# Ensure images folder exists and is writable
RUN mkdir -p /app/wwwroot/images \
    && chmod -R 777 /app/wwwroot/images

ENTRYPOINT ["dotnet", "Library.Api.dll"]
