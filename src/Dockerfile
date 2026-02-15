FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY SignalRChat/*.csproj ./SignalRChat/
RUN dotnet restore SignalRChat/SignalRChat.csproj -r linux-musl-x64
COPY SignalRChat/. ./SignalRChat/

RUN dotnet publish SignalRChat/SignalRChat.csproj \
    -c Release \
    -r linux-musl-x64 \
    --self-contained true \
    --no-restore \
    -o /app/publish \
    /p:PublishTrimmed=true \
    /p:PublishSingleFile=false \
    /p:TrimMode=full \
    /p:EnableCompressionInSingleFile=true \
    /p:DebugType=none \
    /p:DebugSymbols=false

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine AS runtime

RUN addgroup -S appgroup && adduser -S appuser -G appgroup

WORKDIR /app
COPY --from=build /app/publish .
USER appuser
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "SignalRChat.dll"]
