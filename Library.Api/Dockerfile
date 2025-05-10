FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ConnectionStrings:SqlServerConnection="Server=db; Database=LibraryDb; User=sa; Password=DB_Password; TrustServerCertificate=true;"
ENV ASPNETCORE_URLS=http://+:80

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
