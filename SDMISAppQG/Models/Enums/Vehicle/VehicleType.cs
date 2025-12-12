using System.ComponentModel;

namespace SDMISAppQG.Models.Enums.Vehicle;

public enum VehicleType {
   [Description("Non défini")]
   Inconnu = 0,

   // --- Secours à Personne (SAP) ---
   [Description("Véhicule de Secours et d'Assistance aux Victimes")]
   VSAV = 10,

   [Description("Véhicule Léger Infirmier")]
   VLI = 11,

   [Description("Véhicule Radio-Médicalisé")]
   VRM = 12,

   [Description("Véhicule de Premier Secours")]
   VPS = 13,

   // --- Incendie Urbain ---
   [Description("Fourgon Pompe Tonne")]
   FPT = 20,

   [Description("Fourgon Pompe Tonne Léger")]
   FPTL = 21,

   [Description("Fourgon Pompe Tonne Secours Routier")]
   FPTSR = 22,

   [Description("Fourgon Mousse Grande Puissance")]
   FMOGP = 23,

   // --- Incendie Espaces Naturels (Feux de Forêt) ---
   [Description("Camion Citerne Feux de Forêt")]
   CCF = 30,

   [Description("Camion Citerne Grande Capacité")]
   CCGC = 31,

   [Description("Véhicule Léger Hors Route")]
   VLHR = 32,

   // --- Moyens Élévateurs (Aériens) ---
   [Description("Échelle Pivotante Automatique")]
   EPA = 40,

   [Description("Bras Élévateur Aérien")]
   BEA = 41,

   // --- Secours Routier & Technique ---
   [Description("Véhicule de Secours Routier")]
   VSR = 50,

   [Description("Véhicule d'Intervention Risques Technologiques")]
   VIRT = 51,

   [Description("Véhicule Plongeurs")]
   VPL = 52,

   [Description("Véhicule Cynotechnique")]
   CYNO = 53,

   // --- Commandement & Logistique ---
   [Description("Véhicule de Liaison")]
   VL = 90,

   [Description("Véhicule de Liaison Chef de Groupe")]
   VLCG = 91,

   [Description("Poste de Commandement de Colonne")]
   PCC = 92,

   [Description("Véhicule Tout Usage")]
   VTU = 99
}