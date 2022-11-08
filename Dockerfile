FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HomeAssistant.Service/HomeAssistant.Service.csproj", "HomeAssistant.Service/"]
RUN dotnet restore "HomeAssistant.Service/HomeAssistant.Service.csproj"
COPY . .
WORKDIR "/src/HomeAssistant.Service"
RUN dotnet build "HomeAssistant.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeAssistant.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeAssistant.Service.dll"]
