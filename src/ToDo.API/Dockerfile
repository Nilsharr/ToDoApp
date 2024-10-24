FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
COPY ["Directory.Build.props", "/"]
COPY ["Directory.Packages.props", "/"]
WORKDIR /src
COPY ["src/ToDo.Domain/ToDo.Domain.csproj", "ToDo.Domain/"]
COPY ["src/ToDo.Infrastructure/ToDo.Infrastructure.csproj", "ToDo.Infrastructure/"]
COPY ["src/ToDo.API/ToDo.API.csproj", "ToDo.API/"]
RUN dotnet restore "ToDo.API/ToDo.API.csproj"
COPY /src .
WORKDIR "/src/ToDo.API"
RUN dotnet build "ToDo.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ToDo.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDo.API.dll"]
