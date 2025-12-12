namespace SDMISAppQG.Models.Enums.Incidents;

[Obsolete("Plus utilisé pour l'instant")]
public enum IncidentTypeOld {
   // ==========================================
   // 100 SERIES: FIRE (Incendies)
   // ==========================================

   /// <summary>Generic fire incident (Unknown type).</summary>
   Fire = 100,

   /// <summary>Fire in a building (house, apartment, commercial).</summary>
   StructureFire = 110,

   /// <summary>Fire contained to a non-combustible container (e.g., food on stove).</summary>
   CookingFire = 113,

   /// <summary>Fire in mobile property (cars, trucks, buses).</summary>
   PassengerVehicleFire = 131,

   /// <summary>Fire in heavy equipment (tractors, construction).</summary>
   HeavyEquipmentFire = 132,

   /// <summary>Forest, woods, or wildland fire.</summary>
   WildlandFire = 141,

   /// <summary>Brush or brush-and-grass mixture fire.</summary>
   BrushFire = 142,

   /// <summary>Outside rubbish, trash, or waste fire.</summary>
   TrashFire = 151,

   /// <summary>Dumpster or outside trash receptacle fire.</summary>
   DumpsterFire = 154,

   // ==========================================
   // 200 SERIES: EXPLOSION / RUPTURE (Explosions)
   // ==========================================

   /// <summary>Generic explosion or rupture (no fire initially).</summary>
   Explosion = 200,

   /// <summary>Overpressure rupture of steam pipe or boiler.</summary>
   SteamRupture = 210,

   /// <summary>Munitions or bomb explosion (no fire).</summary>
   MunitionExplosion = 241,

   // ==========================================
   // 300 SERIES: RESCUE & EMS (Secours à personne)
   // ==========================================

   /// <summary>Generic rescue or medical emergency.</summary>
   RescueOrEms = 300,

   /// <summary>Medical assistance call (heart attack, stroke, etc.).</summary>
   MedicalAssist = 311,

   /// <summary>Motor vehicle accident with injuries.</summary>
   VehicleAccidentWithInjuries = 322,

   /// <summary>Motor vehicle accident with NO injuries.</summary>
   VehicleAccidentNoInjuries = 324,

   /// <summary>Pedestrian struck by vehicle.</summary>
   PedestrianStruck = 323,

   /// <summary>Search for lost person.</summary>
   SearchMission = 340,

   /// <summary>Extrication of victim from vehicle or machinery.</summary>
   Extrication = 350,

   /// <summary>Water rescue (river, lake, swimming pool).</summary>
   WaterRescue = 360,

   /// <summary>Electrical rescue (person in contact with power).</summary>
   ElectricalRescue = 381,

   // ==========================================
   // 400 SERIES: HAZARDOUS CONDITIONS (Risques techno)
   // ==========================================

   /// <summary>Generic hazardous condition.</summary>
   HazardousCondition = 400,

   /// <summary>Gas leak (natural gas or LPG).</summary>
   GasLeak = 412,

   /// <summary>Oil or other combustible liquid spill.</summary>
   OilSpill = 413,

   /// <summary>Chemical hazard (no spill or leak yet).</summary>
   ChemicalHazard = 420,

   /// <summary>Chemical spill or leak.</summary>
   ChemicalSpill = 422,

   /// <summary>Carbon monoxide incident.</summary>
   CarbonMonoxideIncident = 424,

   /// <summary>Power line down.</summary>
   PowerLineDown = 444,

   /// <summary>Bomb scare or suspicious package.</summary>
   BombThreat = 445,

   // ==========================================
   // 500 SERIES: SERVICE CALLS (Services divers)
   // ==========================================

   /// <summary>Generic service call.</summary>
   ServiceCall = 500,

   /// <summary>Person in distress (e.g. ring stuck, locked in room).</summary>
   PersonInDistress = 510,

   /// <summary>Water problem (evacuation of water, leak).</summary>
   WaterProblem = 520,

   /// <summary>Smoke or odor removal.</summary>
   SmokeOrOdorRemoval = 531,

   /// <summary>Animal problem or rescue.</summary>
   AnimalRescue = 540,

   /// <summary>Public service assistance (lift assist, unlocking door).</summary>
   PublicServiceAssistance = 550,

   /// <summary>Cover assignment, standby at fire station.</summary>
   StandbyCover = 571,

   // ==========================================
   // 600 SERIES: GOOD INTENT (Bonne foi / Annulé)
   // ==========================================

   /// <summary>Generic good intent call.</summary>
   GoodIntentCall = 600,

   /// <summary>Dispatched but cancelled en route.</summary>
   CancelledEnRoute = 611,

   /// <summary>Wrong location.</summary>
   WrongLocation = 621,

   /// <summary>Controlled burning (authorized).</summary>
   ControlledBurn = 631,

   /// <summary>Steam, vapor, fog, or dust thought to be smoke.</summary>
   MistakenForSmoke = 650,

   // ==========================================
   // 700 SERIES: FALSE ALARMS (Fausses alertes)
   // ==========================================

   /// <summary>Generic false alarm.</summary>
   FalseAlarm = 700,

   /// <summary>Malicious, mischievous false call.</summary>
   MaliciousFalseCall = 710,

   /// <summary>System malfunction (sprinkler, detection).</summary>
   SystemMalfunction = 730,

   /// <summary>Smoke detector activation (no fire, malfunction).</summary>
   DetectorMalfunction = 740,

   /// <summary>Unintentional transmission of alarm (testing).</summary>
   UnintentionalTransmission = 743,

   // ==========================================
   // 800 SERIES: WEATHER & DISASTER (Intempéries)
   // ==========================================

   /// <summary>Generic severe weather or natural disaster.</summary>
   SevereWeatherOrDisaster = 800,

   /// <summary>Earthquake assessment.</summary>
   Earthquake = 812,

   /// <summary>Wind storm, tornado, hurricane.</summary>
   WindStorm = 813,

   /// <summary>Lightning strike (investigation).</summary>
   LightningStrike = 814,

   /// <summary>Flood assessment.</summary>
   FloodAssessment = 815
}