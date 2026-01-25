FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["JustiSafe.Web/JustiSafe.Web.csproj", "JustiSafe.Web/"]
COPY ["JustiSafe.Core/JustiSafe.Core.csproj", "JustiSafe.Core/"]
COPY ["JustiSafe.Data/JustiSafe.Data.csproj", "JustiSafe.Data/"]
RUN dotnet restore "JustiSafe.Web/JustiSafe.Web.csproj"
COPY . .
WORKDIR "/src/JustiSafe.Web"
RUN dotnet build "JustiSafe.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JustiSafe.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JustiSafe.Web.dll"]
