using System;
using UnityEngine;

namespace Alcubierre
{
    /// <summary>
    /// Arquitectura y tipo de cohete ensamblado en el astillero.
    /// </summary>
    public enum RocketArchitecture
    {
        ChemicalRocket,       // Cohete Químico Industrial (Multietapa Al-Li 2195, inyectores LOX/LH2)
        AntimatterRocket,     // Cohete de Antimateria Relativista (Viga de tracción Valkyrie, trampa Penning)
        WarpDriveShip         // Nave de Curvatura Espaciotemporal (Anillos resonadores Casimir, IXS Enterprise)
    }

    /// <summary>
    /// Tipo de combustible o propelente cargado en los tanques de la nave.
    /// </summary>
    public enum FuelType
    {
        ChemicalCriogenic,    // Hidrógeno y Oxígeno Líquidos Criogénicos (LOX/LH2 - Isp ~450 s)
        AntimatterPlasma,     // Antiprotones en trampa magnética de vacío (Aniquilación 187 MeV)
        ExoticCasimirVacuum   // Fluctuaciones de vacío cuántico con densidad de energía negativa (rho < 0)
    }

    /// <summary>
    /// Sistema de protección térmica contra fricción atmosférica y polvo interestelar (TPS).
    /// </summary>
    public enum ThermalProtectionSystem
    {
        None_0mm,                     // Sin protección térmica (Fusión del morro a v > 35 km/s)
        CeramicTiles_25mm,            // Losetas cerámicas HRSI de 25 mm (Resiste hasta 1.800 K)
        PICAX_Ablative_50mm,          // Escudo ablativo reforzado PICA-X de 50 mm (Resiste hasta 3.200 K)
        UHTC_HafniumCarbide_100mm     // Cerámica de Ultra-Alta Temperatura UHTC de 100 mm (Resiste > 4.000 K)
    }

    /// <summary>
    /// Los tres Tiers de materiales estructurales del casco según contexto/04_materiales_de_la_nave.
    /// </summary>
    public enum ShipMaterialTier
    {
        Tier1_Aerospace_AlLi,         // Aleación Al-Li 2195 / Titanio CFRP (Química estándar)
        Tier2_Shielded_TungstenHTS,   // Tungsteno denso + Berilio ISM + Superconductores HTS (Antimateria relativista)
        Tier3_Metamaterial_Casimir    // Nanotubos CNT + Metamaterial Hiperbólico resonante (Alcubierre Warp)
    }

    /// <summary>
    /// Espesor del blindaje contra radiación y gas interestelar (ISM).
    /// </summary>
    public enum ShieldThickness
    {
        None_0cm,
        Standard_2_5cm,
        Reinforced_5cm,
        Heavy_10cm
    }

    /// <summary>
    /// Perfil de ruta de navegación interplanetaria/interestelar y trazado de trayectorias.
    /// </summary>
    public enum RouteProfile
    {
        DirectMaximumBurn,        // Ruta Directa: menor tiempo, máxima aceleración, atraviesa zonas de radiación
        HohmannElliptic,          // Transferencia Hohmann: arco elíptico amplio de mínima energía
        SafeShieldedCorridor,     // Corredor Seguro: esquiva cinturones de Van Allen y magnetosfera joviana
        GravityAssistSlingshot,   // Asistencia Gravitatoria: impulso por pozo gravitacional planetario
        CustomManualWaypoint      // Trazado Manual: el piloto ajusta curvatura e inflexión libremente
    }

    /// <summary>
    /// Niveles de alerta de supervivencia del vehículo según contexto/04_materiales_de_la_nave/02_estados_limite.
    /// </summary>
    public enum AlertLevel
    {
        Nominal,    // 100% - 70%: Sistemas verdes y óptimos
        Warning,    // 70% - 35%: Luces ámbar parpadeantes, aviso acústico
        Critical,   // 35% - 1%: Alarma roja estroboscópica, micro-vibración, peligro inminente
        Failed      // 0%: Ruptura de casco, sobrepresión de tanques o pérdida de confinamiento
    }

    /// <summary>
    /// Propiedades físicas y computacionales de cada material de la nave.
    /// </summary>
    [Serializable]
    public struct MaterialProperties
    {
        public string name;
        public ShipMaterialTier tier;
        public double baseDryMassKg;
        public double density_gcm3;
        public double maxSafeSpeedFractionC;
        public double gammaShieldFactor;       // 0.0 (nulo) a 1.0 (bloqueo total 99.9%)
        public double thermalDissipationRating; // Capacidad de disipación de calor del reactor
        public double tensileYieldStrengthMPa; // Resistencia mecánica contra sobrepresión / Fuerzas G
        public string description;
        public Color tierColor;
    }

    /// <summary>
    /// Dictamen de compatibilidad del astillero antes de iniciar el vuelo.
    /// </summary>
    [Serializable]
    public struct CompatibilityVerdict
    {
        public bool isViable;
        public bool hasWarning;
        public bool hasCriticalRisk;
        public string title;
        public string detailedReport;
        public string fuelVerdict;
        public Color fuelStatusColor;
    }

    /// <summary>
    /// Configuración de ensamblaje, materiales y combustible elegidos para la nave.
    /// </summary>
    [Serializable]
    public class ShipConfiguration
    {
        [Header("Arquitectura y Propelente")]
        public RocketArchitecture rocketType = RocketArchitecture.ChemicalRocket;
        public FuelType fuelType = FuelType.ChemicalCriogenic;

        [Header("Materiales y Estructura")]
        public ShipMaterialTier materialTier = ShipMaterialTier.Tier1_Aerospace_AlLi;
        public ShieldThickness shieldThickness = ShieldThickness.Standard_2_5cm;
        public ThermalProtectionSystem thermalProtection = ThermalProtectionSystem.CeramicTiles_25mm;

        [Header("Combustible y Almacenamiento")]
        [Range(20f, 250f)]
        public float fuelLoadingPercentage = 115f; // 115% = Nominal + 15% margen de seguridad

        [Header("Ruta de Navegación")]
        public RouteProfile routeProfile = RouteProfile.SafeShieldedCorridor;

        [Header("Trazado Manual Interactivo")]
        [Range(-15f, 15f)] public float manualArcHeight = 5.0f;       // Desvío en eje Y (-15 a +15 UA/unidades)
        [Range(0.1f, 0.9f)] public float manualInflectionT = 0.5f;    // Inflexión de la curva Bezier (0.1 a 0.9)

        public static MaterialProperties GetProperties(ShipMaterialTier tier)
        {
            switch (tier)
            {
                case ShipMaterialTier.Tier1_Aerospace_AlLi:
                    return new MaterialProperties
                    {
                        name = "Tier 1: Aleación Aeroespacial Al-Li 2195 & Titanio CFRP",
                        tier = tier,
                        baseDryMassKg = 1000.0,
                        density_gcm3 = 2.60,
                        maxSafeSpeedFractionC = 0.001, // 300 km/s
                        gammaShieldFactor = 0.12,      // Blindaje muy bajo contra radiación nuclear
                        thermalDissipationRating = 0.65,
                        tensileYieldStrengthMPa = 580.0,
                        description = "Estructura ligera convencional óptima para vuelos químicos en el Sistema Solar interior.",
                        tierColor = new Color(0.80f, 0.85f, 0.90f)
                    };

                case ShipMaterialTier.Tier2_Shielded_TungstenHTS:
                    return new MaterialProperties
                    {
                        name = "Tier 2: Blindaje de Tungsteno, Berilio & Superconductores HTS",
                        tier = tier,
                        baseDryMassKg = 2500.0, // Más pesada por el mamparo de tungsteno
                        density_gcm3 = 19.25,
                        maxSafeSpeedFractionC = 0.50,  // 150.000 km/s
                        gammaShieldFactor = 0.995,     // Absorbe 99.5% de rayos gamma de aniquilación
                        thermalDissipationRating = 0.92,
                        tensileYieldStrengthMPa = 1450.0,
                        description = "Blindaje masivo contra radiación gamma de 187 MeV y deflector de erosión interestelar (ISM).",
                        tierColor = new Color(0.50f, 0.52f, 0.55f)
                    };

                case ShipMaterialTier.Tier3_Metamaterial_Casimir:
                    return new MaterialProperties
                    {
                        name = "Tier 3: Metamaterial Hiperbólico & Grafeno Cuántico",
                        tier = tier,
                        baseDryMassKg = 800.0,  // Ultraligera
                        density_gcm3 = 1.40,
                        maxSafeSpeedFractionC = 100.0, // Superlumínica aparente
                        gammaShieldFactor = 0.70,
                        thermalDissipationRating = 0.98,
                        tensileYieldStrengthMPa = 4200.0,
                        description = "Canaliza gradientes de densidad de energía exótica (rho < 0) en caída libre geodésica pura.",
                        tierColor = new Color(0.15f, 0.85f, 1.0f)
                    };

                default:
                    return default;
            }
        }

        public double GetShieldThicknessCm()
        {
            switch (shieldThickness)
            {
                case ShieldThickness.None_0cm: return 0.0;
                case ShieldThickness.Standard_2_5cm: return 2.5;
                case ShieldThickness.Reinforced_5cm: return 5.0;
                case ShieldThickness.Heavy_10cm: return 10.0;
                default: return 2.5;
            }
        }

        public double CalculateShieldMassKg()
        {
            double thickCm = GetShieldThicknessCm();
            if (thickCm <= 0.0) return 0.0;

            MaterialProperties props = GetProperties(materialTier);
            double cabinSurfaceAreaCm2 = 220000.0;
            double volumeCm3 = cabinSurfaceAreaCm2 * thickCm;
            double massGrams = volumeCm3 * props.density_gcm3 * 0.15;
            return massGrams / 1000.0;
        }

        public double GetThermalProtectionThicknessMm()
        {
            switch (thermalProtection)
            {
                case ThermalProtectionSystem.None_0mm: return 0.0;
                case ThermalProtectionSystem.CeramicTiles_25mm: return 25.0;
                case ThermalProtectionSystem.PICAX_Ablative_50mm: return 50.0;
                case ThermalProtectionSystem.UHTC_HafniumCarbide_100mm: return 100.0;
                default: return 25.0;
            }
        }

        public double CalculateThermalShieldMassKg()
        {
            switch (thermalProtection)
            {
                case ThermalProtectionSystem.None_0mm: return 0.0;
                case ThermalProtectionSystem.CeramicTiles_25mm: return 350.0;
                case ThermalProtectionSystem.PICAX_Ablative_50mm: return 850.0;
                case ThermalProtectionSystem.UHTC_HafniumCarbide_100mm: return 1900.0;
                default: return 350.0;
            }
        }

        public double GetTotalDryMassKg()
        {
            MaterialProperties props = GetProperties(materialTier);
            return props.baseDryMassKg + CalculateShieldMassKg() + CalculateThermalShieldMassKg();
        }

        /// <summary>
        /// Evalúa la compatibilidad física y los riesgos de combustible antes del lanzamiento.
        /// </summary>
        public CompatibilityVerdict EvaluateCompatibility(PropulsionMode mode, double distanceKm, double nominalRequiredFuelKg)
        {
            CompatibilityVerdict verdict = new CompatibilityVerdict();
            verdict.isViable = true;
            verdict.hasWarning = false;
            verdict.hasCriticalRisk = false;

            MaterialProperties props = GetProperties(materialTier);
            double actualFuelKg = nominalRequiredFuelKg * (fuelLoadingPercentage / 100.0);

            // 1. Incompatibilidad Catastrófica Cohete vs Combustible
            if (rocketType == RocketArchitecture.ChemicalRocket && fuelType == FuelType.AntimatterPlasma)
            {
                verdict.title = "🚨 ¡CATÁSTROFE REAL: ANTIMATERIA EN COHETE QUÍMICO!";
                verdict.detailedReport = "INCOMPATIBILIDAD CRÍTICA: Inyectar plasma de antiprotones en tanques convencionales de Al-Li sin confinamiento Penning desatará una aniquilación gamma instantánea de 187 MeV que destruirá la nave en la plataforma.";
                verdict.hasCriticalRisk = true;
            }
            else if (rocketType == RocketArchitecture.ChemicalRocket && fuelType == FuelType.ExoticCasimirVacuum)
            {
                verdict.title = "⛔ FALLO: ENERGÍA CUÁNTICA EN INYECTORES QUÍMICOS";
                verdict.detailedReport = "Los inyectores químicos de LOX/LH2 no interactúan con el vacío cuántico Casimir. Cero empuje disponible.";
                verdict.hasCriticalRisk = true;
            }
            else if (rocketType == RocketArchitecture.AntimatterRocket && fuelType == FuelType.ChemicalCriogenic)
            {
                verdict.title = "⛔ FALLO: PROPULSIÓN QUÍMICA EN TOBERA MAGNÉTICA";
                verdict.detailedReport = "La tobera de piones relativista no puede procesar combustión LOX/LH2. Pérdida total de impulso.";
                verdict.hasCriticalRisk = true;
            }
            else if (rocketType == RocketArchitecture.WarpDriveShip && fuelType != FuelType.ExoticCasimirVacuum)
            {
                verdict.title = "🚨 ¡COLAPSO MÉTRICO WARP SIN ENERGÍA EXÓTICA!";
                verdict.detailedReport = "La burbuja de Alcubierre requiere densidad de energía negativa (rho < 0). Utilizar combustible químico o plasma convencional provocará el colapso inmediato del anillo de curvatura.";
                verdict.hasCriticalRisk = true;
            }
            else
            {
                // Compatibilidad Cohete vs Material del Casco
                if (rocketType == RocketArchitecture.ChemicalRocket)
                {
                    if (distanceKm > SpaceConstants.KmPerAU * 100.0)
                    {
                        verdict.title = "ℹ️ Vuelo Químico Generacional (~75.000 años)";
                        verdict.detailedReport = "El chasis Al-Li soporta el vuelo químico, pero la distancia interestelar requiere milenios a 17 km/s.";
                        verdict.hasWarning = true;
                    }
                    else if (materialTier == ShipMaterialTier.Tier1_Aerospace_AlLi)
                    {
                        verdict.title = "✅ Configuración Óptima (100% Eficiencia)";
                        verdict.detailedReport = "El chasis Al-Li 2195 es la combinación nativa para quemados químicos LOX/LH2.";
                    }
                    else
                    {
                        verdict.title = "⚠️ Ineficiencia por Sobrepeso Estructural";
                        verdict.detailedReport = "El blindaje denso añade masa innecesaria para un vuelo químico convencional.";
                        verdict.hasWarning = true;
                    }
                }
                else if (rocketType == RocketArchitecture.AntimatterRocket)
                {
                    if (materialTier == ShipMaterialTier.Tier1_Aerospace_AlLi)
                    {
                        verdict.title = "⚠️ RIESGO CRÍTICO: Casco Al-Li sin Blindaje Nuclear";
                        verdict.detailedReport = "El casco Al-Li carece de blindaje de tungsteno. Los rayos gamma de 187 MeV irradiarán la cabina.";
                        verdict.hasCriticalRisk = true;
                    }
                    else
                    {
                        verdict.title = "✅ Compatibilidad Óptima (Protección Relativista 99.5%)";
                        verdict.detailedReport = "Mamparo de tungsteno y escudo ISM de berilio absorben radiación gamma y choque de partículas.";
                    }
                }
                else
                {
                    if (materialTier == ShipMaterialTier.Tier3_Metamaterial_Casimir)
                    {
                        verdict.title = "✅ Compatibilidad Cuántica Perfecta (rho < 0)";
                        verdict.detailedReport = "El metamaterial canaliza la densidad de energía exótica sin tensión inercial interna (0.00g).";
                    }
                    else
                    {
                        verdict.title = "⚠️ RIESGO CRÍTICO: Chasis Incompatible con Métrica Warp";
                        verdict.detailedReport = "El casco convencional no puede canalizar fluctuaciones Casimir. La burbuja sufrirá inestabilidad cuántica.";
                        verdict.hasCriticalRisk = true;
                    }
                }
            }

            // 2. Verificación de Protección Térmica y Radiológica
            if (thermalProtection == ThermalProtectionSystem.None_0mm)
            {
                verdict.detailedReport += "\n⚠️ SIN ESCUDO TÉRMICO (0 mm): Peligro inminente de ablación y destrucción en zonas de alta velocidad o calor.";
                verdict.hasWarning = true;
            }
            if (shieldThickness == ShieldThickness.None_0cm)
            {
                verdict.detailedReport += "\n⚠️ SIN BLINDAJE DE RADIACIÓN (0 cm): La tripulación recibirá dosis letales de radiación en el espacio profundo.";
                verdict.hasWarning = true;
            }

            // 3. Evaluación del Combustible Embarcado
            if (fuelType != FuelType.ExoticCasimirVacuum)
            {
                if (fuelLoadingPercentage < 95f)
                {
                    verdict.fuelVerdict = $"⚠️ DÉFICIT ({fuelLoadingPercentage:F0}%): Combustible insuficiente. La nave quedará a la deriva antes de desacelerar en destino.";
                    verdict.fuelStatusColor = new Color(1f, 0.35f, 0.2f);
                    verdict.hasWarning = true;
                }
                else if (fuelLoadingPercentage <= 130f)
                {
                    verdict.fuelVerdict = $"🟢 ÓPTIMO ({fuelLoadingPercentage:F0}%): Margen de seguridad estándar (+15%) sin sobrecarga de inercia.";
                    verdict.fuelStatusColor = new Color(0.2f, 1f, 0.4f);
                }
                else if (fuelLoadingPercentage <= 180f)
                {
                    verdict.fuelVerdict = $"⚠️ SOBRECARGA ({fuelLoadingPercentage:F0}%): Aumento de masa seca. Riesgo de sobrepresión en empuje máximo.";
                    verdict.fuelStatusColor = new Color(1f, 0.85f, 0.2f);
                    verdict.hasWarning = true;
                }
                else
                {
                    verdict.fuelVerdict = $"🚨 RIESGO CRÍTICO ({fuelLoadingPercentage:F0}%): Sobrellenado extremo (>180%). Peligro de explosión y rotura de tanques.";
                    verdict.fuelStatusColor = new Color(1f, 0.2f, 0.2f);
                    verdict.hasCriticalRisk = true;
                }
            }
            else
            {
                verdict.fuelVerdict = "🌌 VACÍO DINÁMICO CASIMIR: Energía negativa ilimitada (rho < 0). Sin consumo de masa de reacción.";
                verdict.fuelStatusColor = new Color(0.2f, 0.85f, 1f);
            }

            return verdict;
        }
    }

    /// <summary>
    /// Sistema dinámico de simulación de supervivencia, físicas límite y telemetría de fallos en tiempo real.
    /// </summary>
    [Serializable]
    public class ShipSurvivalSystem
    {
        [Header("Integridad y Parámetros Vitales")]
        public float hullIntegrityPct = 100f;
        public double accumulatedRadiation_mSv = 0.05;
        public float engineThermalStressPct = 10f;
        public float warpBubbleStabilityPct = 100f;

        [Header("Estado y Alertas")]
        public AlertLevel currentAlert = AlertLevel.Nominal;
        public string activeAlertMessage = "SISTEMAS NOMINALES";
        public bool isMissionFailed = false;
        public string failureReason = "";

        [Header("Secuencia de Catástrofe / Explosión")]
        public bool isCatastropheActive = false;
        public float catastropheTimer = 0f;
        public string catastropheType = "";
        public const float CatastropheDuration = 4.0f;

        [Header("Subsistemas Interactivos de Cabina")]
        public bool isISMDeflectorActive = false;

        public void Reset()
        {
            hullIntegrityPct = 100f;
            accumulatedRadiation_mSv = 0.05;
            engineThermalStressPct = 10f;
            warpBubbleStabilityPct = 100f;
            currentAlert = AlertLevel.Nominal;
            activeAlertMessage = "SISTEMAS NOMINALES";
            isMissionFailed = false;
            failureReason = "";
            isCatastropheActive = false;
            catastropheTimer = 0f;
            catastropheType = "";
            isISMDeflectorActive = false;
        }

        public void PurgeCryoValves()
        {
            if (engineThermalStressPct > 15f)
            {
                engineThermalStressPct = Mathf.Max(10f, engineThermalStressPct - 30f);
                activeAlertMessage = "💨 VÁLVULAS PURGADAS: Presión criogénica y sobrecalentamiento aliviados.";
            }
        }

        public void ToggleISMDeflector()
        {
            isISMDeflectorActive = !isISMDeflectorActive;
            activeAlertMessage = isISMDeflectorActive 
                ? "🛡️ DEFLECTOR ISM ACTIVADO: Campo magnético frontal desplegado contra polvo interestelar." 
                : "🛡️ DEFLECTOR ISM DESACTIVADO: Energía reasignada a propulsión.";
        }

        /// <summary>
        /// Evoluciona la salud física de la nave según las condiciones del vuelo y el entorno astrofísico.
        /// </summary>
        public void UpdateSurvivalTick(float deltaTime, float flightProgress, double currentSpeedKms, 
                                       double velocityFractionC, double feltG, PropulsionMode mode, 
                                       ShipConfiguration config, double distanceToSunKm, 
                                       bool insideVanAllen, bool insideJupiterMagnetosphere,
                                       bool insideSolarCorona, bool insideAsteroidBelt,
                                       bool insidePulsarBeam, bool insideBlackHoleAccretion,
                                       string hazardZoneName)
        {
            if (isMissionFailed) return;

            // 0. COMPROBACIÓN DE INCOMPATIBILIDAD ARQUITECTÓNICA CATASTRÓFICA
            if (config.rocketType == RocketArchitecture.ChemicalRocket && config.fuelType == FuelType.AntimatterPlasma)
            {
                isCatastropheActive = true;
                catastropheTimer = CatastropheDuration;
                currentAlert = AlertLevel.Failed;
                hullIntegrityPct = 0f;
                catastropheType = "ANIQUILACIÓN DE ANTIMATERIA EN TANQUES QUÍMICOS";
                failureReason = "💥 ANATOMÍA DEL DESASTRE: Inyección de antimateria en tanques de aluminio y cámaras químicas sin trampa Penning. La aniquilación espontánea materia-antimateria (187 MeV) destruyó la nave en la plataforma de despegue.";
                return;
            }

            if (config.rocketType == RocketArchitecture.WarpDriveShip && config.fuelType != FuelType.ExoticCasimirVacuum)
            {
                isCatastropheActive = true;
                catastropheTimer = CatastropheDuration;
                currentAlert = AlertLevel.Failed;
                warpBubbleStabilityPct = 0f;
                catastropheType = "COLAPSO CUÁNTICO POR FALTA DE ENERGÍA NEGATIVA";
                failureReason = "🌌 ANATOMÍA DEL DESASTRE: La nave Alcubierre intentó curvar el espaciotiempo sin densidad de energía exótica (rho < 0). El colapso métrico provocó la destrucción del anillo toroidal.";
                return;
            }

            if (config.rocketType == RocketArchitecture.ChemicalRocket && config.fuelType == FuelType.ExoticCasimirVacuum)
            {
                isCatastropheActive = true;
                catastropheTimer = CatastropheDuration;
                currentAlert = AlertLevel.Failed;
                catastropheType = "CERO IMPULSO / FALLO DE INYECCIÓN CUÁNTICA";
                failureReason = "⚠️ ANATOMÍA DEL DESASTRE: Los inyectores químicos convencionales no pueden interactuar con fluctuaciones de vacío Casimir. Pérdida total de control y colapso de la nave.";
                return;
            }

            if (config.rocketType == RocketArchitecture.AntimatterRocket && config.fuelType == FuelType.ChemicalCriogenic)
            {
                isCatastropheActive = true;
                catastropheTimer = CatastropheDuration;
                currentAlert = AlertLevel.Failed;
                hullIntegrityPct = 0f;
                catastropheType = "COMBUSTIÓN QUÍMICA EN TOBERA MAGNÉTICA RELATIVISTA";
                failureReason = "💥 ANATOMÍA DEL DESASTRE: Los inyectores magnéticos de antimateria intentaron procesar propelente criogénico LOX/LH2. Obstrucción catastrófica y explosión térmica en la tobera de confinamiento.";
                return;
            }

            if (config.rocketType == RocketArchitecture.AntimatterRocket && config.fuelType == FuelType.ExoticCasimirVacuum)
            {
                isCatastropheActive = true;
                catastropheTimer = CatastropheDuration;
                currentAlert = AlertLevel.Failed;
                hullIntegrityPct = 0f;
                catastropheType = "INCOMPATIBILIDAD CUÁNTICA EN TRAMPA PENNING";
                failureReason = "🌌 ANATOMÍA DEL DESASTRE: La trampa magnética Penning no puede almacenar fluctuaciones de vacío Casimir (rho < 0). Desestabilización de campo e implosión del núcleo.";
                return;
            }

            // Si la secuencia de catástrofe está activa, consume el temporizador y sacude la nave
            if (isCatastropheActive)
            {
                catastropheTimer -= deltaTime;
                activeAlertMessage = $"💥 ¡CATÁSTROFE VEHICULAR INMINENTE: {catastropheType}! ({catastropheTimer:F1}s)";
                if (catastropheTimer <= 0f)
                {
                    isMissionFailed = true;
                    isCatastropheActive = false;
                }
                return;
            }

            MaterialProperties props = ShipConfiguration.GetProperties(config.materialTier);
            double shieldThick = config.GetShieldThicknessCm();

            // 1. EVALUACIÓN DE RADIACIÓN AMBIENTAL
            // Radiación cósmica galáctica de fondo (GCR) ~ 1.8 mSv/día = 0.00003 mSv/s
            double radiationRate_mSv_per_sec = 0.00003;

            // Radiación solar decae con (1 AU / r)^2
            double distAU = Math.Max(0.1, distanceToSunKm / SpaceConstants.KmPerAU);
            double solarFluxMultiplier = 1.0 / (distAU * distAU);
            radiationRate_mSv_per_sec += 0.00005 * solarFluxMultiplier;

            // Factores de atenuación de blindaje (Beer-Lambert)
            double attenuationFactor = Math.Max(0.004, 1.0 - (props.gammaShieldFactor * (1.0 + shieldThick * 0.09)));
            if (config.materialTier == ShipMaterialTier.Tier1_Aerospace_AlLi && mode == PropulsionMode.Antimatter)
            {
                attenuationFactor = 0.95; // Al-Li deja pasar los rayos gamma de 187 MeV
            }
            if (isISMDeflectorActive)
            {
                attenuationFactor *= 0.65; // Deflector magnético absorbe partículas cargadas
            }

            // Zonas críticas: Van Allen, Corona Solar, Magnetosfera Joviana, Púlsar, Agujero Negro
            if (insideVanAllen && config.routeProfile != RouteProfile.SafeShieldedCorridor)
            {
                radiationRate_mSv_per_sec += 0.035; // 126 mSv/hora
            }
            if (insideJupiterMagnetosphere)
            {
                double jupiterIntensity = (config.routeProfile == RouteProfile.SafeShieldedCorridor) ? 0.04 : 0.55;
                radiationRate_mSv_per_sec += jupiterIntensity;
            }
            if (insideSolarCorona)
            {
                radiationRate_mSv_per_sec += 0.12;
                engineThermalStressPct = Mathf.Min(100f, engineThermalStressPct + 25f * deltaTime);
            }
            if (insidePulsarBeam)
            {
                radiationRate_mSv_per_sec += 0.95; // Haz gamma sincrotrón extremadamente letal
            }
            if (insideBlackHoleAccretion)
            {
                radiationRate_mSv_per_sec += 0.65; // Rayos X duros del disco de acreción
                feltG += 22.0; // Mareas gravitacionales
                if (props.tensileYieldStrengthMPa < 2000.0)
                {
                    hullIntegrityPct -= 12f * deltaTime;
                }
            }

            // Fuga parásita por almacenamiento excesivo de antimateria
            if (mode == PropulsionMode.Antimatter && config.fuelLoadingPercentage > 140f)
            {
                float excessRatio = (config.fuelLoadingPercentage - 140f) / 100f;
                radiationRate_mSv_per_sec += 0.10 * excessRatio;
            }

            accumulatedRadiation_mSv += radiationRate_mSv_per_sec * attenuationFactor * deltaTime * 20.0;

            // 2. EVALUACIÓN DE ESTRÉS TÉRMICO
            if (mode == PropulsionMode.Chemical)
            {
                float targetHeat = (feltG > 0.5f) ? (config.fuelLoadingPercentage > 150f ? 82f : 45f) : 12f;
                engineThermalStressPct = Mathf.Lerp(engineThermalStressPct, targetHeat, deltaTime * 0.5f);

                // Sobrepresión en tanques por sobrecarga excesiva (> 180%)
                if (config.fuelLoadingPercentage > 185f && feltG > 1.5f)
                {
                    hullIntegrityPct -= 9.5f * deltaTime;
                }
            }
            else if (mode == PropulsionMode.Antimatter)
            {
                float targetHeat = (float)(20f + velocityFractionC * 65f);
                engineThermalStressPct = Mathf.Lerp(engineThermalStressPct, targetHeat, deltaTime * 0.8f);
            }
            else
            {
                engineThermalStressPct = 8f; // En Warp no hay calentamiento mecánico
            }

            // Evaluación de Fricción Térmica del Fuselaje según Escudo TPS
            if (config.thermalProtection == ThermalProtectionSystem.None_0mm)
            {
                if (currentSpeedKms > 25.0 || insideSolarCorona || insideAsteroidBelt)
                {
                    engineThermalStressPct = Mathf.Min(100f, engineThermalStressPct + 28f * deltaTime);
                    hullIntegrityPct -= 6.5f * deltaTime;
                }
            }
            else if (insideSolarCorona && config.thermalProtection != ThermalProtectionSystem.UHTC_HafniumCarbide_100mm)
            {
                engineThermalStressPct = Mathf.Min(100f, engineThermalStressPct + 15f * deltaTime);
            }

            // 3. EROSIÓN RELATIVISTA Y POLVO ESPACIAL
            if (insideAsteroidBelt && currentSpeedKms > 35.0)
            {
                if (config.materialTier != ShipMaterialTier.Tier2_Shielded_TungstenHTS && !isISMDeflectorActive)
                {
                    float beltAbrasion = (float)(currentSpeedKms * 0.08f);
                    hullIntegrityPct -= beltAbrasion * deltaTime;
                }
            }

            if (velocityFractionC > 0.05 && mode == PropulsionMode.Antimatter)
            {
                if (config.materialTier != ShipMaterialTier.Tier2_Shielded_TungstenHTS && !isISMDeflectorActive)
                {
                    float abrasionRate = (float)(velocityFractionC * 14.0f);
                    hullIntegrityPct -= abrasionRate * deltaTime;
                }
            }

            // Inestabilidad de burbuja Warp si el casco es incompatible
            if (mode == PropulsionMode.WarpDrive)
            {
                if (config.materialTier != ShipMaterialTier.Tier3_Metamaterial_Casimir)
                {
                    warpBubbleStabilityPct -= 20.0f * deltaTime;
                }
            }

            // 4. DETECCIÓN DE CATÁSTROFE / ESTADOS LÍMITE
            if (hullIntegrityPct <= 0f || accumulatedRadiation_mSv > 3500.0 || warpBubbleStabilityPct <= 0f || engineThermalStressPct >= 100f)
            {
                isCatastropheActive = true;
                catastropheTimer = CatastropheDuration;
                currentAlert = AlertLevel.Failed;

                if (hullIntegrityPct <= 0f)
                {
                    hullIntegrityPct = 0f;
                    catastropheType = "RUPTURA Y DESCOMPRESIÓN DE CASCO";
                    failureReason = "💥 COLAPSO ESTRUCTURAL DEL CASCO: Rotura mecánica por fatiga, sobrepresión de tanques o abrasión en cinturón de asteroides.";
                }
                else if (accumulatedRadiation_mSv > 3500.0)
                {
                    catastropheType = "DOSIS LETAL DE RADIACIÓN";
                    failureReason = "☠️ DOSIS LETAL DE RADIACIÓN: La tripulación superó el límite biológico de 3.5 Sv por radiación cósmica sin blindaje adecuado.";
                }
                else if (engineThermalStressPct >= 100f)
                {
                    catastropheType = "EXPLOSIÓN TÉRMICA DEL REACTOR";
                    failureReason = "🔥 EXPLOSIÓN TÉRMICA: Sobrecalentamiento extremo en cámara de combustión o aproximación destructiva a la corona solar.";
                }
                else
                {
                    warpBubbleStabilityPct = 0f;
                    catastropheType = "COLAPSO CUÁNTICO DE LA BURBUJA WARP";
                    failureReason = "🌌 COLAPSO CUÁNTICO DE LA BURBUJA: El material de la nave no pudo canalizar la energía exótica Casimir (rho < 0).";
                }
                return;
            }

            // 5. CLASIFICACIÓN DEL NIVEL DE ALERTA
            if (hullIntegrityPct < 35f || accumulatedRadiation_mSv > 1000.0 || engineThermalStressPct > 85f || warpBubbleStabilityPct < 35f)
            {
                currentAlert = AlertLevel.Critical;
                if (hullIntegrityPct < 35f) activeAlertMessage = "🚨 CRÍTICO: Integridad de Casco < 35% (Fisuras Detectadas)";
                else if (accumulatedRadiation_mSv > 1000.0) activeAlertMessage = "🚨 CRÍTICO: Dosis de Radiación Peligrosa (> 1.0 Sv)";
                else if (engineThermalStressPct > 85f) activeAlertMessage = "🚨 CRÍTICO: Temperatura de Reactor en Zona Roja";
                else activeAlertMessage = "🚨 CRÍTICO: Inestabilidad Métrica Warp Severa";
            }
            else if (hullIntegrityPct < 70f || accumulatedRadiation_mSv > 250.0 || engineThermalStressPct > 60f || warpBubbleStabilityPct < 70f || !string.IsNullOrEmpty(hazardZoneName))
            {
                currentAlert = AlertLevel.Warning;
                if (!string.IsNullOrEmpty(hazardZoneName)) activeAlertMessage = $"⚠️ ATRAVESANDO: {hazardZoneName}";
                else if (accumulatedRadiation_mSv > 250.0) activeAlertMessage = "⚠️ ADVERTENCIA: Nivel de Radiación Elevado (> 250 mSv)";
                else if (hullIntegrityPct < 70f) activeAlertMessage = "⚠️ ADVERTENCIA: Desgaste Abrasivo en Casco Frontal";
                else if (engineThermalStressPct > 60f) activeAlertMessage = "⚠️ ADVERTENCIA: Sobrecalentamiento Térmico";
                else activeAlertMessage = "⚠️ ADVERTENCIA: Fluctuación Cuántica en Métrica Warp";
            }
            else
            {
                currentAlert = AlertLevel.Nominal;
                activeAlertMessage = "🟢 SISTEMAS NOMINALES: Vuelo Estable";
            }
        }
    }
}

