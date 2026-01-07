# Consultez https://aka.ms/customizecontainer pour savoir comment personnaliser votre conteneur de débogage et comment Visual Studio utilise ce Dockerfile pour générer vos images afin d’accélérer le débogage.

# Cet index est utilisé lors de l’exécution à partir de VS en mode rapide (par défaut pour la configuration de débogage)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Cette phase est utilisée pour générer le projet de service
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SDMISAppQG/SDMISAppQG.csproj", "SDMISAppQG/"]
RUN dotnet restore "./SDMISAppQG/SDMISAppQG.csproj"
COPY . .
WORKDIR "/src/SDMISAppQG"
RUN dotnet build "./SDMISAppQG.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Install dotnet-ef tool for migrations
RUN dotnet tool install --global dotnet-ef --version 10.0.1
ENV PATH="${PATH}:/root/.dotnet/tools"

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SDMISAppQG.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l'exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n'est pas utilisée)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/SDMISAppQG /src/SDMISAppQG
COPY --from=build /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copy entrypoint script
COPY SDMISAppQG/entrypoint.sh .
RUN chmod +x entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]