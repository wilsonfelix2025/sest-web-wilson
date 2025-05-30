# Etapa base
FROM mcr.microsoft.com/dotnet/aspnet:2.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:2.1 AS build
WORKDIR /source

COPY src/SestWeb.Api/SestWeb.Api.csproj src/SestWeb.Api/
COPY src/SestWeb.Application/SestWeb.Application.csproj src/SestWeb.Application/
COPY src/SestWeb.Domain/SestWeb.Domain.csproj src/SestWeb.Domain/
COPY src/SestWeb.Infra/SestWeb.Infra.csproj src/SestWeb.Infra/

RUN dotnet restore src/SestWeb.Api/SestWeb.Api.csproj

COPY src/ src/

WORKDIR /source/src/SestWeb.Api
RUN dotnet build SestWeb.Api.csproj -c Release -o /app/build

# Etapa de publicação corrigida
FROM build AS publish
WORKDIR /source/src/SestWeb.Api
RUN dotnet publish SestWeb.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SestWeb.Api.dll"]
