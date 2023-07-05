FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY "SignalRChat/SignalRChat.csproj" ./

RUN dotnet restore "SignalRChat.csproj"

COPY . ./


RUN dotnet publish "SignalRChat/SignalRChat.csproj" -c Release -o out

FROM base as final
WORKDIR /app
COPY --from=build /app/out .

# Configure ASP.NET Core
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production


ENTRYPOINT ["dotnet", "SignalRChat.dll"]
