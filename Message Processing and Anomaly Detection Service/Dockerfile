FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Message Processing and Anomaly Detection Service/Message Processing and Anomaly Detection Service.csproj", "Message Processing and Anomaly Detection Service/"]
RUN dotnet restore "Message Processing and Anomaly Detection Service/Message Processing and Anomaly Detection Service.csproj"
COPY . .
WORKDIR "/src/Message Processing and Anomaly Detection Service"
RUN dotnet build "./Message Processing and Anomaly Detection Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Message Processing and Anomaly Detection Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Message Processing and Anomaly Detection Service.dll"]
