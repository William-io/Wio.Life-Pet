FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Source/Wio.Life-Pet.Web/Wio.Life-Pet.Web.csproj", "Source/Wio.Life-Pet.Web/"]
RUN dotnet restore "Source/Wio.Life-Pet.Web/Wio.Life-Pet.Web.csproj"
COPY . .
WORKDIR "/src/Source/Wio.Life-Pet.Web"
RUN dotnet build "./Wio.Life-Pet.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Wio.Life-Pet.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wio.Life-Pet.Web.dll"]
