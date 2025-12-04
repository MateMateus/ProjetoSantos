# Usa a imagem base do .NET 9 para rodar a aplicação (mais leve)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Usa a imagem do SDK do .NET 9 para construir a aplicação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# --- ATENÇÃO: Substitua 'NomeDoSeuProjeto' pelo nome real do seu arquivo .csproj ---
COPY ["SantosApi.csproj", "."]
RUN dotnet restore "./SantosApi.csproj"

COPY . .
WORKDIR "/src/."
RUN dotnet build "./ProjetoSantos.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProjetoSantos.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjetoSantos.dll"]