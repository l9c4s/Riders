
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Esta fase � usada para compilar o projeto de servi�o
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY ["Riders/Riders.csproj", "Riders/"]
COPY ["Aplication/Aplication.csproj", "Aplication/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infraestructure/Infraestructure.csproj", "Infraestructure/"]
COPY ["Test_Riders/Test_Riders.csproj", "Test_Riders/"]


# Restaure as depend�ncias
RUN dotnet restore "Riders/Riders.csproj"

COPY . .
  
# Build do projeto
WORKDIR "/src/Riders"
RUN dotnet build "Riders.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase � usada para publicar o projeto de servi�o a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Riders.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase � usada na produ��o ou quando executada no VS no modo normal (padr�o quando n�o est� usando a configura��o de Depura��o)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Riders.dll"]


