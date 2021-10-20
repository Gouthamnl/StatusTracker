FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

COPY TrackingService/TrackingService.csproj TrackingService/
COPY TrackingService.DAL/TrackingService.DAL.csproj TrackingService.DAL/
COPY Logger/Logger.csproj Logger/
COPY TrackingServiceRunner/TrackingServiceRunner.csproj TrackingServiceRunner/
RUN dotnet restore TrackingServiceRunner/TrackingServiceRunner.csproj
COPY . .
WORKDIR /src/TrackingServiceRunner
RUN dotnet build TrackingServiceRunner.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TrackingServiceRunner.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TrackingServiceRunner.dll"]
