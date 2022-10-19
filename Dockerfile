FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["goHomeAssistant.Service/goHomeAssistant.Service.csproj", "goHomeAssistant.Service/"]
RUN dotnet restore "goHomeAssistant.Service/goHomeAssistant.Service.csproj"
COPY . .
WORKDIR "/src/goHomeAssistant.Service"
RUN dotnet build "goHomeAssistant.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "goHomeAssistant.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "goHomeAssistant.Service.dll"]
