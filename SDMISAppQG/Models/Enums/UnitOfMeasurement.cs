namespace SDMISAppQG.Models.Enums;

public enum UnitOfMeasurement {
   Unknown = 0,
   Unitless = 1,           // For raw counters or error codes
   Boolean = 2,            // 0 or 1 (e.g., Lights on, Door open, Parking brake)

   // --- 1. Geolocation & Navigation ---
   DecimalDegrees = 10,    // Latitude / Longitude
   Degrees = 11,           // Heading (0-360°) or Ladder angle
   Meters = 12,            // Altitude, GPS Accuracy, Ladder extension
   Kilometers = 13,        // Odometer
   KilometersPerHour = 14, // Vehicle speed

   // --- 2. Mechanical & Energy ---
   Percentage = 20,        // Fuel Level, AdBlue, Battery Level, Pedal position
   Volts = 21,             // Battery Voltage
   Amperes = 22,           // Electrical Current
   RevolutionsPerMinute = 23, // Engine RPM
   Hours = 24,             // Engine hours / Pump hours
   Celsius = 25,           // Temperature (Engine, Oil, Outside)

   // --- 3. Hydraulics (Firefighter specifics) ---
   Bar = 30,               // Pressure (Water pump, Air brakes, O2 cylinder)
   Liters = 31,            // Volume (Water Tank, Foam Tank)
   LitersPerMinute = 32,   // Flow rate (Water Canon, Pump output)
   CubicMetersPerHour = 33,// Flow rate (High volume pumps)

   // --- 4. Safety & Environment ---
   PartsPerMillion = 40,   // PPM (CO detector, Gas)
   PercentageLEL = 41,     // Lower Explosive Limit (Explosive gas concentration)
   Sievert = 42            // Radiation (NRBC events)
}
