# SDMIS AppQG - Backend

> Système de gestion des incidents et interventions pour les services de secours

## Table des matières

- [Vue d'ensemble](#vue-densemble)
- [Modules principaux](#modules-principaux)
- [Prérequis](#prérequis)
- [Installation et démarrage](#installation-et-démarrage)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Tests](#tests)
- [Technologies utilisées](#technologies-utilisées)

---

## Vue d'ensemble

SDMISAppQG-Backend est une application backend ASP.NET Core (.NET 10) conçue pour gérer les incidents, interventions, et véhicules des services de secours. Le système intègre trois composantes principales : une API REST, des WebSockets pour la télémétrie en temps réel, et RabbitMQ pour la communication inter-services.

## Modules principaux

### 1. Module API REST

L'API REST expose plusieurs endpoints pour la gestion des ressources :

#### Controllers disponibles

- **IncidentsController** : Gestion des incidents (feu, accident, secours personne, etc.)
  - CRUD complet avec types d'incidents (feu_industriel, feu_urbain, accident_route, etc.)
  - Statuts : déclaré, en_cours, résolu, annulé
  - Géolocalisation avec PostGIS (latitude/longitude)

- **InterventionsController** : Gestion des interventions des équipes
  - Liaison incidents ↔ véhicules
  - Statuts : prise_en_charge, en_cours, terminée, refusée_annulée, besoin_aide

- **VehiclesController** : Gestion de la flotte de véhicules
  - Types : camion_citerne, VSAV, échelle
  - Disponibilité temps réel
  - Raisons d'indisponibilité (panne, maintenance, etc.)

- **UsersController** : Gestion des utilisateurs
- **AssigneesController** : Affectation des personnels aux interventions
- **PassengersController** : Gestion des passagers dans les véhicules
- **TelemetryLogsController** : Historique des données de télémétrie
- **MessagingController** : Communication avec les services externes via RabbitMQ

#### Authentification

- **Keycloak JWT** : Toutes les routes sont protégées par authentification JWT
- Authority : `https://keycloak.crouscam.fr/realms/sdmis`
- Les tokens sont validés à chaque requête

#### Documentation API

- **Scalar UI** : Interface interactive disponible à `/scalar/v1` en mode développement
- **OpenAPI** : Spécification disponible à `/openapi/v1.json`

### 2. Module WebSocket (SignalR)

Le système utilise SignalR pour les communications temps réel avec deux hubs principaux :

#### **TelemetryHub** (`/hubs/telemetry`)

- **Usage** : Réception des données de télémétrie depuis la passerelle Python
- **Méthode** : `ReceiveTelemetry(string data)`
- **Fonctionnalités** :
  - Réception des données de capteurs véhicules en temps réel
  - Traitement et stockage automatique en base de données
  - Broadcast vers les clients React connectés
  - Logging détaillé des connexions et données reçues

#### **GpsHub** (`/hubs/gps`)

- **Usage** : Diffusion en temps réel des positions GPS des véhicules
- **Interface typée** : Utilise `IGpsClient` pour garantir la cohérence des messages
- **Fonctionnalités** :
  - Suivi en temps réel de la position des véhicules
  - Mise à jour automatique sur les cartes côté client

#### Configuration CORS

```csharp
// Permet les connexions depuis React et Python
policy.SetIsOriginAllowed(_ => true)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials(); // Requis pour SignalR
```

### 3. Module RabbitMQ

RabbitMQ assure la communication asynchrone avec les services externes (notamment Java).

#### Configuration

- **Exchange** : `sdmis-exchange` (type: Direct, durable)
- **Queues** :
  - `dotnet-to-java-queue` : Messages envoyés vers Java
  - `java-to-dotnet-queue` : Messages reçus depuis Java

#### Service RabbitMQService

```csharp
// Publication vers Java
PublishToJava<T>(MessageEnvelope<T> envelope)

// Consommation depuis Java
ConsumeFromJava<T>(Func<T, Task> handler)

// Health check
IsHealthyAsync() : bool
```

#### Messages types

- **VehicleLocationUpdate** : Mise à jour de position véhicule
- **IncidentNotification** : Notification de nouvel incident
- **GenericMessage** : Messages génériques avec payload JSON

#### Points d'accès

Le `MessagingController` expose des endpoints REST pour déclencher l'envoi de messages :
- `POST /api/Messaging/vehicle-location`
- `POST /api/Messaging/incident-notification`
- `POST /api/Messaging/send`
- `GET /api/Messaging/health` : Vérification connexion RabbitMQ

---

## Prérequis

- **Docker** et **Docker Compose**
- **.NET 10 SDK** (pour développement local)
- **PostgreSQL** avec extension **PostGIS** (fourni via Docker)
- **RabbitMQ** (fourni via Docker)

---

## Installation et démarrage

### 1. Cloner le repository

```bash
git clone <url-du-repo>
cd SDMISAppQG-Backend
```

### 2. Configurer les variables d'environnement

Créez un fichier `.env` à la racine du projet :

```env
# PostgreSQL
POSTGRES_DB=appqg-db
POSTGRES_USER=sdmid
POSTGRES_PASSWORD=votreMotDePasse

# RabbitMQ
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
```

### 3. Lancer les conteneurs

```bash
docker-compose up --build
```

#### ⚠️ IMPORTANT : Temps de démarrage

Le démarrage complet prend plusieurs minutes :

1. **PostgreSQL** démarre en premier (~10-20 secondes)
   - Le healthcheck vérifie que la base est prête

2. **RabbitMQ** démarre ensuite (~15-30 secondes)
   - Le healthcheck vérifie la disponibilité

3. **Backend .NET** démarre après les healthchecks
   - Restauration des dépendances NuGet
   - **Application des migrations Entity Framework** (~30-60 secondes)
   - **Chargement des fixtures** via l'entrypoint.sh
   - Démarrage de l'application

**Total estimé : 1-2 minutes**

#### Vérifier que tout est prêt

```bash
# Logs du backend
docker logs -f sdmisappqg-backend

# Vous devriez voir :
# ✓ "Migrations applied successfully!"
# ✓ "Starting application..."
# ✓ "Now listening on: http://[::]:8080"
```

### 4. Accès aux services

Une fois démarré :

| Service | URL | Credentials |
|---------|-----|-------------|
| **Backend API** | http://localhost:5078 | JWT Token |
| **Scalar API Docs** | http://localhost:5078/scalar/v1 | - |
| **PostgreSQL** | localhost:5432 | Voir `.env` |
| **RabbitMQ Management** | http://localhost:15672 | guest/guest |
| **SignalR Telemetry** | ws://localhost:5078/hubs/telemetry | - |
| **SignalR GPS** | ws://localhost:5078/hubs/gps | - |

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db;Port=5432;Database=appqg-db;..."
  },
  "Keycloak": {
    "Authority": "https://keycloak.crouscam.fr/realms/sdmis",
    "Audience": "account"
  },
  "RabbitMQ": {
    "HostName": "rabbitmq",
    "ExchangeName": "sdmis-exchange",
    "QueueNames": {
      "ToJava": "dotnet-to-java-queue",
      "FromJava": "java-to-dotnet-queue"
    }
  }
}
```

### Structure de la base de données

Le fichier `init/init.sql` contient :
- Extension PostGIS pour la géolocalisation
- Types ENUM PostgreSQL pour les statuts
- Tables principales : users, incidents, interventions, vehicles, etc.
- Relations et contraintes de clés étrangères

Les migrations Entity Framework sont appliquées automatiquement au démarrage du conteneur.

---

## 📚 API Documentation

### Utiliser Bruno (recommandé)

Le projet inclut une collection complète Bruno dans `brunoRoutes/` :

```
brunoRoutes/
├── collection.bru           # Configuration collection
├── getToken.bru            # Obtenir un JWT
├── Incidents/              # 9 routes
├── Interventions/          # 8 routes
├── Vehicles/               # Routes véhicules
├── Users/                  # Routes utilisateurs
├── Messaging/              # 5 routes (RabbitMQ + health)
└── environments/
    └── local.bru           # Environnement local
```

1. Installer [Bruno](https://www.usebruno.com/)
2. Ouvrir la collection `brunoRoutes/`
3. Sélectionner l'environnement `local`
4. Exécuter `getToken.bru` pour obtenir un JWT
5. Tester les endpoints

### Endpoints clés

```http
# Incidents
GET    /api/Incidents
POST   /api/Incidents
GET    /api/Incidents/{id}
PUT    /api/Incidents/{id}
DELETE /api/Incidents/{id}

# Interventions
GET    /api/Interventions
POST   /api/Interventions
GET    /api/Interventions/incident/{incidentId}

# Vehicles
GET    /api/Vehicles
POST   /api/Vehicles
GET    /api/Vehicles/{id}

# Messaging (RabbitMQ)
POST   /api/Messaging/vehicle-location
POST   /api/Messaging/incident-notification
GET    /api/Messaging/health
```

---

## 🧪 Tests

Le projet inclut une suite de tests dans `SDMISTests/` :

```
SDMISTests/
├── Unit/          # Tests unitaires
├── Integration/   # Tests d'intégration
└── Fixtures/      # Données de test
```

### Exécuter les tests

```bash
# En local
dotnet test

# Dans Docker (TODO: Ajouter tests au CI/CD)
```

---

## 🔧 Technologies utilisées

### Backend

| Technologie | Version | Usage |
|-------------|---------|-------|
| **.NET** | 10.0 | Framework principal |
| **ASP.NET Core** | 10.0 | API REST |
| **Entity Framework Core** | 10.0 | ORM |
| **SignalR** | 10.0 | WebSockets temps réel |
| **RabbitMQ.Client** | 7.2.0 | Messagerie asynchrone |

### Base de données

| Technologie | Usage |
|-------------|-------|
| **PostgreSQL** | Base de données relationnelle |
| **PostGIS** | Extension géospatiale (lat/long) |
| **Npgsql** | Driver .NET pour PostgreSQL |
| **NetTopologySuite** | Manipulation géométries (GeoJSON) |

### Authentification

| Technologie | Usage |
|-------------|-------|
| **Keycloak** | Serveur d'authentification OAuth2/OIDC |
| **JWT Bearer** | Tokens d'authentification |

### Infrastructure

| Technologie | Usage |
|-------------|-------|
| **Docker** | Conteneurisation |
| **Docker Compose** | Orchestration multi-conteneurs |
| **RabbitMQ** | Message broker |

### Documentation

| Outil | URL |
|-------|-----|
| **Scalar** | `/scalar/v1` - UI interactive |
| **OpenAPI** | `/openapi/v1.json` - Spécification |
| **Bruno** | `brunoRoutes/` - Collection de tests |


