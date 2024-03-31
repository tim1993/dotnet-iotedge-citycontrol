FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore -r linux-arm

COPY . ./
RUN dotnet publish -c Release -o out -r linux-arm --self-contained true

FROM mcr.microsoft.com/dotnet/runtime:8.0.3-bookworm-slim-arm32v7
WORKDIR /app
COPY --from=build-env /app/out ./


ENTRYPOINT ["dotnet", "NET.CityControl.IoTEdge.dll"]
