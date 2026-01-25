FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/JustiSafe.Gateway/JustiSafe.Gateway.csproj", "Services/JustiSafe.Gateway/"]
RUN dotnet restore "Services/JustiSafe.Gateway/JustiSafe.Gateway.csproj"
COPY . .
WORKDIR "/src/Services/JustiSafe.Gateway"
RUN dotnet build "JustiSafe.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JustiSafe.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JustiSafe.Gateway.dll"]
