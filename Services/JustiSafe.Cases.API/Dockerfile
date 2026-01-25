FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/JustiSafe.Cases.API/JustiSafe.Cases.API.csproj", "Services/JustiSafe.Cases.API/"]
RUN dotnet restore "Services/JustiSafe.Cases.API/JustiSafe.Cases.API.csproj"
COPY . .
WORKDIR "/src/Services/JustiSafe.Cases.API"
RUN dotnet build "JustiSafe.Cases.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JustiSafe.Cases.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JustiSafe.Cases.API.dll"]
