CREATE EXTENSION IF NOT EXISTS postgis;

-- Incident
CREATE TYPE incident_type AS ENUM (
    'feu_industriel', 'feu_urbain', 'feu_parc', 'feu_foret',
    'accident_route', 'accident_ferroviaire',
    'maladie_malaise', 'inondation', 'fuite_gaz', 'secours_personne'
);

CREATE TYPE incident_severity AS ENUM ('faible', 'moyen', 'eleve');
CREATE TYPE incident_statut AS ENUM ('declare', 'en_cours', 'resolu', 'annule');
CREATE TYPE incident_source AS ENUM ('telephone', 'sur_place');

-- Intervention
CREATE TYPE intervention_statut AS ENUM (
    'prise_en_charge', 'en_cours', 'terminee', 'refusee_annulee', 'besoin_aide'
);

-- Truck
CREATE TYPE truck_type AS ENUM ('camion_citerne', 'vsav', 'echelle');
CREATE TYPE truck_disponibility AS ENUM ('disponible', 'en_intervention', 'indisponible');
CREATE TYPE truck_indisponibility_reason AS ENUM (
    'panne_essence', 'crevaison', 'bouchons', 'malaise_personnel', 
    'maintenance', 'autre'
);

CREATE TABLE users (
    id UUID PRIMARY KEY,
    email TEXT UNIQUE NOT NULL,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);


CREATE TABLE incidents (
    id UUID PRIMARY KEY,
    type incident_type NOT NULL,
    location GEOGRAPHY(POINT, 4326),
    severity incident_severity NOT NULL,
    priority INT,
    statut incident_statut NOT NULL,
    source incident_source NOT NULL,
    description TEXT,
    created_by UUID REFERENCES users(id),
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE interventions (
    id UUID PRIMARY KEY,
    incident_id UUID REFERENCES incidents(id) ON DELETE CASCADE,
    statut intervention_statut NOT NULL,
    "begin" TIMESTAMP,
    "end" TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE trucks (
    id UUID PRIMARY KEY,
    type truck_type NOT NULL,
    last_location GEOGRAPHY(POINT, 4326),
    disponibility truck_disponibility NOT NULL,
    indisponibility_reason truck_indisponibility_reason,
    fuel FLOAT,
    consumable JSONB,
    passenger_count INT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE assignees (
    id UUID PRIMARY KEY,
    intervention_id UUID REFERENCES interventions(id) ON DELETE CASCADE,
    truck_id UUID REFERENCES trucks(id),
    itinerary JSONB,
    "begin" TIMESTAMP,
    "end" TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE telemetry_logs (
    id UUID PRIMARY KEY,
    truck_id UUID REFERENCES trucks(id) ON DELETE CASCADE,
    captors_values JSONB NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);
