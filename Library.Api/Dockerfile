FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5100
EXPOSE 7070

ENV ConnectionStrings:SqlServerConnection="Server=db; Database=LibraryDb; User=sa; Password=DB_Password; TrustServerCertificate=true;"

# Set environment variables for Redis connection and cache settings
ENV CACHE_REDIS="library.cache:6379"
ENV CACHE_BOOK_IMAGE_KEY_PREFIX="book-img-"
ENV CACHE_MIME_KEY_PREFIX="mime-img-"
ENV CACHE_LIFETIME_MINUTES=60


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
ENTRYPOINT ["dotnet", "Library.Api.dll"]
