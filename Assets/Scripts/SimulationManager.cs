using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Alcubierre.Cockpit;

namespace Alcubierre
{
    public enum ViewPerspective
    {
        CockpitPOV3D,   // Vista en primera persona dentro de la cabina 3D
        TacticalMap2D   // Mapa orbital general para planificación astronómica
    }

    public enum SimulationUIState
    {
        MainMenu,           // Menú principal centrado con catálogo de 5 simulaciones y configuración
        PreFlightChecklist, // Modal interactivo de lista de comprobación de subsistemas antes del despegue
        InFlight,           // Simulación activa en ejecución (Cabina 3D POV o Mapa Táctico 2D)
        Paused              // Menú de pausa modal centrado con reanudar, reiniciar y opciones
    }

    /// <summary>
    /// Controlador principal de la simulación astronómica completa.
    /// Integra el Sistema Solar completo, hitos interestelares, cuadrícula astronómica,
    /// cámara multiescala libre/seguimiento, visualización normalizada sin superposiciones
    /// y cálculo analítico estricto según la física de Tsiolkovsky, Antimateria y Alcubierre Warp.
    /// </summary>
    public class SimulationManager : MonoBehaviour
    {
        [Header("--- NAVEGACIÓN Y ESTADO DE SIMULACIÓN ---")]
        public SimulationUIState currentUIState = SimulationUIState.MainMenu;
        public ViewPerspective currentPerspective = ViewPerspective.CockpitPOV3D;
        public PropulsionMode selectedPropulsion = PropulsionMode.Chemical;
        public int selectedDestinationIndex = 3; // Por defecto: Luna (Simulación 1)
        public string originBodyName = "Tierra (Origen de Referencia)";

        [Header("--- CONTROL DE TIEMPO Y ACELERACIÓN (TIME CONTROLS & REWIND) ---")]
        [Range(0f, 100f)] public float timeWarpMultiplier = 1.0f;
        public bool isPaused = false;
        public bool isRewinding = false;
        public float baseFlightDurationSec = 180f;

        [Header("--- CONFIGURACIÓN DE NAVE Y TALLER (ASTILLERO) ---")]
        public ShipConfiguration shipConfig = new ShipConfiguration();
        public ShipSurvivalSystem survivalSystem = new ShipSurvivalSystem();
        private int mainMenuTab = 0; // 0 = 5 Misiones, 1 = Taller de Ensamblaje, 2 = Configuración Libre

        [Header("--- PROTOCOLO DE DESPEGUE & CUENTA REGRESIVA ---")]
        public bool[] preFlightChecks = new bool[5];
        public float launchCountdownTimer = -1f;
        public const float LaunchCountdownDuration = 3.5f;
        public bool isInspectingDashboard = false;
        private float launchBannerFadeTimer = 0f;

        [Header("--- DINÁMICA ORBITAL & ESTADO DE ATERRIZAJE ---")]
        public bool isMissionSuccessModalOpen = false;
        private Vector3 departureOriginPos = Vector3.zero;

        [Header("--- VISIBILIDAD DE INTERFAZ ---")]
        public bool showHUD = true;

        [Header("--- VARIABLES EN EJECUCIÓN (TELEMETRÍA) ---")]
        [SerializeField] private bool isFlying = false;
        [SerializeField] private float flightProgress = 0f;
        [SerializeField] private double flightElapsedTimeEarth = 0.0;
        [SerializeField] private double flightElapsedTimeShip = 0.0;
        [SerializeField] private double currentDistanceCoveredKm = 0.0;
        [SerializeField] private double currentRemainingDistanceKm = 0.0;
        [SerializeField] private double currentVelocityKms = 0.0;
        [SerializeField] private double currentLorentzGamma = 1.0;
        [SerializeField] private double currentGForce = 0.0;
        [SerializeField] private double currentRemainingFuelKg = 1000.0;
        [SerializeField] private string flightStatus = "Listo para iniciar simulación.";

        // Componentes y referencias visuales
        private GameObject cosmos2DRoot;
        private List<CelestialBody> celestialBodies = new List<CelestialBody>();
        private CelestialBody earthBody;
        private CelestialBody sunBody;
        private CelestialBody targetBody;
        private GameObject spaceshipObj;
        private LineRenderer trajectoryLine;
        private LineRenderer[] tickMarkers;
        private TrailRenderer shipTrail;
        private GameObject warpBubbleVisual;
        private SpaceCamera spaceCamera;
        private AstronomicalGrid astroGrid;
        private CockpitManager cockpitManager;

        // Estilos e imágenes para la interfaz gráfica escalada
        private GUIStyle titleStyle;
        private GUIStyle subtitleStyle;
        private GUIStyle headerStyle;
        private GUIStyle bodyStyle;
        private GUIStyle smallStyle;
        private GUIStyle panelBoxStyle;
        private GUIStyle presetCardStyle;
        private GUIStyle presetTitleStyle;
        private GUIStyle telemetryCardStyle;
        private GUIStyle buttonPrimaryStyle;
        private GUIStyle buttonSecondaryStyle;

        private Texture2D texPanelDark;
        private Texture2D texPanelAccent;
        private Texture2D texButtonPrimary;
        private Texture2D texButtonPrimaryHover;
        private Texture2D texButtonSecondary;
        private Texture2D texButtonSecondaryHover;
        private Texture2D texBarGreen;
        private Texture2D texBarCyan;
        private Texture2D texBarRed;
        private Texture2D texBarYellow;
        private Texture2D texAlertGreen;
        private Texture2D texAlertAmber;
        private Texture2D texAlertRed;
        private Texture2D texVignette;
        private bool stylesInitialized = false;

        // Sprites de planetas y fondos
        private Sprite earthSprite;
        private Sprite sunSprite;
        private Sprite mercurySprite;
        private Sprite venusSprite;
        private Sprite marsSprite;
        private Sprite moonSprite;
        private Sprite jupiterSprite;
        private Sprite saturnSprite;
        private Sprite uranusSprite;
        private Sprite neptuneSprite;
        private Sprite plutoSprite;
        private Sprite asteroidSprite;
        private Sprite shipSprite;
        private Sprite spaceBgSprite;

        // Estructura de datos para los cuerpos celestes del escenario
        public struct CelestialData
        {
            public string name;
            public double distanceKm;
            public double distanceAU;
            public Vector2 visualPos;
            public float visualDiameter;
            public Color color;
            public BodyCategory category;
            public string description;
        }

        private CelestialData[] fullCosmosData;
        private List<GameObject> asteroidBeltObjects = new List<GameObject>();

        private void Awake()
        {
            LoadAllSprites();
            InitializeCosmosData();
        }

        private void Start()
        {
            // 1. Construir entorno visual con escalas y separaciones normalizadas bajo Cosmos2D_Root
            BuildCosmosEnvironment();
            SetupSpaceship();
            SetupTrajectoryAndTrail();
            SetupAstronomicalGrid();

            // 2. Configurar cámara 2D
            Camera cam = Camera.main;
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.015f, 0.018f, 0.035f);
                spaceCamera = cam.GetComponent<SpaceCamera>() ?? cam.gameObject.AddComponent<SpaceCamera>();
                spaceCamera.enabled = false;
            }

            // 3. Inicializar Ecosistema de Cabinas 3D (Cockpit POV)
            GameObject cockpitObj = new GameObject("CockpitManager_Root");
            cockpitObj.transform.position = new Vector3(0f, 0f, -250f);
            cockpitManager = cockpitObj.AddComponent<CockpitManager>();
            cockpitManager.SetCockpitMode(selectedPropulsion);

            // 4. Configurar misión precalibrada inicial (Simulación 1: Luna Química)
            ApplyPresetConfiguration(1);
            DrawView();

            // 5. Calibrar perspectiva inicial y abrir en el Menú Principal Centrado
            currentUIState = SimulationUIState.MainMenu;
            ApplyPerspective();

            Debug.Log($"[Simulación Iniciada] Pantalla de inicio centrada lista. Origen: {originBodyName} | Destino: {targetBody?.bodyName}");
        }

        private void Update()
        {
            if (currentPerspective == ViewPerspective.CockpitPOV3D)
            {
                if (spaceCamera != null && spaceCamera.enabled)
                {
                    spaceCamera.enabled = false;
                }
            }

            HandleKeyboardShortcuts();

            // Dinámica orbital Kepleriana y rotación axial en tiempo real de los cuerpos celestes
            float orbitalTimeScale = isPaused ? 0f : (isFlying ? Mathf.Max(1.0f, timeWarpMultiplier * 0.25f) : 1.0f);
            if (earthBody != null)
            {
                earthBody.UpdateOrbitalMotion(orbitalTimeScale, Time.deltaTime);
            }
            for (int b = 0; b < celestialBodies.Count; b++)
            {
                if (celestialBodies[b] != null)
                {
                    celestialBodies[b].UpdateOrbitalMotion(orbitalTimeScale, Time.deltaTime);
                }
            }

            // Traslación orbital de los asteroides del cinturón alrededor del Sol
            if (asteroidBeltObjects != null && orbitalTimeScale > 0f)
            {
                Vector3 sunCenter = (sunBody != null) ? sunBody.transform.position : new Vector3(-14f, 0f, 0f);
                float astSpeed = 0.05f * orbitalTimeScale * Time.deltaTime;
                for (int a = 0; a < asteroidBeltObjects.Count; a++)
                {
                    if (asteroidBeltObjects[a] != null)
                    {
                        asteroidBeltObjects[a].transform.RotateAround(sunCenter, Vector3.forward, astSpeed);
                    }
                }
            }

            // Si la nave está en reposo previa al despegue, acompaña el movimiento orbital terrestre
            if (!isFlying && spaceshipObj != null && earthBody != null)
            {
                spaceshipObj.transform.position = earthBody.transform.position + new Vector3(0.9f, 0.3f, 0f);
            }

            // Secuencia cinemática de cuenta regresiva de despegue (3, 2, 1... IGNICIÓN)
            if (launchCountdownTimer > 0f)
            {
                launchCountdownTimer -= Time.deltaTime;
                if (launchCountdownTimer <= 0f)
                {
                    launchCountdownTimer = 0f;
                    launchBannerFadeTimer = 1.6f;
                    if (cockpitManager != null) cockpitManager.TriggerCatastropheShake(false, 0f);
                    StartFlight();
                }
                else
                {
                    float tProgress = 1f - (launchCountdownTimer / LaunchCountdownDuration);
                    if (cockpitManager != null && selectedPropulsion != PropulsionMode.WarpDrive)
                    {
                        cockpitManager.TriggerCatastropheShake(true, 0.2f + tProgress * 0.7f);
                    }
                }
            }
            else if (launchBannerFadeTimer > 0f)
            {
                launchBannerFadeTimer -= Time.deltaTime;
            }

            // Evolución cinemática y física del vuelo
            if (isFlying && !isPaused && timeWarpMultiplier > 0f)
            {
                Simulate();
            }

            // Actualización continua de telemetría hacia los instrumentos de la cabina activa
            if (cockpitManager != null)
            {
                FlightCalculation calc = (targetBody != null)
                    ? PropulsionPhysics.CalculateTrip(selectedPropulsion, targetBody.distanceFromEarthKm, shipConfig.GetTotalDryMassKg(), shipConfig.fuelLoadingPercentage)
                    : default;

                float fuelRatio = (calc.fuelMassKg > 0.0)
                    ? Mathf.Clamp01((float)(currentRemainingFuelKg / calc.fuelMassKg))
                    : 1.0f;

                float feltG = (float)currentGForce;
                float velC = (float)calc.velocityFractionC;
                bool isWarning = (survivalSystem.currentAlert == AlertLevel.Warning || fuelRatio < 0.20f);
                bool isCritical = (survivalSystem.currentAlert == AlertLevel.Critical || survivalSystem.currentAlert == AlertLevel.Failed || fuelRatio < 0.05f);

                cockpitManager.UpdateActiveCockpitTelemetry(fuelRatio, flightProgress, (float)currentVelocityKms, feltG, velC, survivalSystem.accumulatedRadiation_mSv, isFlying, isWarning, isCritical);
            }

            // Actualización de gráficos dinámicos (trayectoria, marcas, curvatura)
            DrawView();
        }

        private void HandleKeyboardShortcuts()
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    if (isInspectingDashboard)
                    {
                        isInspectingDashboard = false;
                        if (cockpitManager != null) cockpitManager.LookForward();
                    }
                    else if (currentUIState == SimulationUIState.PreFlightChecklist)
                    {
                        currentUIState = SimulationUIState.MainMenu;
                    }
                    else if (currentUIState == SimulationUIState.InFlight)
                    {
                        currentUIState = SimulationUIState.Paused;
                        isPaused = true;
                    }
                    else if (currentUIState == SimulationUIState.Paused)
                    {
                        currentUIState = SimulationUIState.InFlight;
                        isPaused = false;
                    }
                }

                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    if (currentUIState == SimulationUIState.InFlight)
                    {
                        currentUIState = SimulationUIState.Paused;
                        isPaused = true;
                    }
                    else if (currentUIState == SimulationUIState.Paused)
                    {
                        currentUIState = SimulationUIState.InFlight;
                        isPaused = false;
                    }
                }

                if (Keyboard.current.vKey.wasPressedThisFrame) TogglePerspective();
                if (Keyboard.current.hKey.wasPressedThisFrame) showHUD = !showHUD;
                if (Keyboard.current.dKey.wasPressedThisFrame && currentPerspective == ViewPerspective.CockpitPOV3D)
                {
                    isInspectingDashboard = !isInspectingDashboard;
                    if (cockpitManager != null)
                    {
                        if (isInspectingDashboard) cockpitManager.LookAtDashboard();
                        else cockpitManager.LookForward();
                    }
                }
                if (Keyboard.current.digit1Key.wasPressedThisFrame)
                {
                    selectedPropulsion = PropulsionMode.Chemical;
                    if (cockpitManager != null) cockpitManager.SetCockpitMode(selectedPropulsion);
                }
                if (Keyboard.current.digit2Key.wasPressedThisFrame)
                {
                    selectedPropulsion = PropulsionMode.Antimatter;
                    if (cockpitManager != null) cockpitManager.SetCockpitMode(selectedPropulsion);
                }
                if (Keyboard.current.digit3Key.wasPressedThisFrame)
                {
                    selectedPropulsion = PropulsionMode.WarpDrive;
                    if (cockpitManager != null) cockpitManager.SetCockpitMode(selectedPropulsion);
                }
                if (Keyboard.current.rKey.wasPressedThisFrame) ResetToEarth();
                if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
                {
                    if (currentUIState == SimulationUIState.MainMenu) StartCustomSimulation();
                    else if (currentUIState == SimulationUIState.PreFlightChecklist) StartLaunchCountdown();
                    else if (!isFlying) StartFlight();
                }
            }
        }

        /// <summary>
        /// Alterna fluidamente entre la vista en primera persona dentro de la cabina 3D (POV)
        /// y el mapa orbital táctico 2D para planificación astrodinámica.
        /// </summary>
        public void TogglePerspective()
        {
            currentPerspective = (currentPerspective == ViewPerspective.CockpitPOV3D)
                ? ViewPerspective.TacticalMap2D
                : ViewPerspective.CockpitPOV3D;
            ApplyPerspective();
        }

        /// <summary>
        /// Aplica la configuración de cámara y visuales requerida para la perspectiva actual.
        /// </summary>
        public void ApplyPerspective()
        {
            if (currentPerspective == ViewPerspective.CockpitPOV3D)
            {
                if (cosmos2DRoot != null) cosmos2DRoot.SetActive(false);
                if (spaceCamera != null) spaceCamera.enabled = false;
                if (cockpitManager != null)
                {
                    cockpitManager.gameObject.SetActive(true);
                    cockpitManager.EnableCockpitPOV();
                    cockpitManager.SetCockpitMode(selectedPropulsion);
                }
            }
            else
            {
                if (cockpitManager != null)
                {
                    cockpitManager.DisableCockpitPOV();
                    cockpitManager.gameObject.SetActive(false);
                }
                if (cosmos2DRoot != null) cosmos2DRoot.SetActive(true);
                if (spaceCamera != null)
                {
                    spaceCamera.enabled = true;
                    spaceCamera.CenterOnPosition(new Vector3(0f, 0f, -10f), 32.0f);
                }
            }
        }

        /// <summary>
        /// Aplica la configuración completa de una de las 5 misiones precalibradas sobre la nave y los sistemas.
        /// </summary>
        public void ApplyPresetConfiguration(int presetNumber)
        {
            switch (presetNumber)
            {
                case 1: // Luna (Química)
                    selectedPropulsion = PropulsionMode.Chemical;
                    selectedDestinationIndex = 3; // Luna
                    shipConfig.rocketType = RocketArchitecture.ChemicalRocket;
                    shipConfig.fuelType = FuelType.ChemicalCriogenic;
                    shipConfig.thermalProtection = ThermalProtectionSystem.CeramicTiles_25mm;
                    shipConfig.materialTier = ShipMaterialTier.Tier1_Aerospace_AlLi;
                    shipConfig.shieldThickness = ShieldThickness.Standard_2_5cm;
                    shipConfig.fuelLoadingPercentage = 115f;
                    shipConfig.routeProfile = RouteProfile.SafeShieldedCorridor;
                    break;
                case 2: // Marte (Hohmann Química)
                    selectedPropulsion = PropulsionMode.Chemical;
                    selectedDestinationIndex = 4; // Marte
                    shipConfig.rocketType = RocketArchitecture.ChemicalRocket;
                    shipConfig.fuelType = FuelType.ChemicalCriogenic;
                    shipConfig.thermalProtection = ThermalProtectionSystem.CeramicTiles_25mm;
                    shipConfig.materialTier = ShipMaterialTier.Tier1_Aerospace_AlLi;
                    shipConfig.shieldThickness = ShieldThickness.Standard_2_5cm;
                    shipConfig.fuelLoadingPercentage = 115f;
                    shipConfig.routeProfile = RouteProfile.HohmannElliptic;
                    break;
                case 3: // Júpiter (Antimateria 0.15c)
                    selectedPropulsion = PropulsionMode.Antimatter;
                    selectedDestinationIndex = 6; // Júpiter
                    shipConfig.rocketType = RocketArchitecture.AntimatterRocket;
                    shipConfig.fuelType = FuelType.AntimatterPlasma;
                    shipConfig.thermalProtection = ThermalProtectionSystem.PICAX_Ablative_50mm;
                    shipConfig.materialTier = ShipMaterialTier.Tier2_Shielded_TungstenHTS;
                    shipConfig.shieldThickness = ShieldThickness.Reinforced_5cm;
                    shipConfig.fuelLoadingPercentage = 120f;
                    shipConfig.routeProfile = RouteProfile.SafeShieldedCorridor;
                    break;
                case 4: // Próxima Centauri (Antimateria 0.50c)
                    selectedPropulsion = PropulsionMode.Antimatter;
                    selectedDestinationIndex = 14; // Próxima Centauri
                    shipConfig.rocketType = RocketArchitecture.AntimatterRocket;
                    shipConfig.fuelType = FuelType.AntimatterPlasma;
                    shipConfig.thermalProtection = ThermalProtectionSystem.UHTC_HafniumCarbide_100mm;
                    shipConfig.materialTier = ShipMaterialTier.Tier2_Shielded_TungstenHTS;
                    shipConfig.shieldThickness = ShieldThickness.Heavy_10cm;
                    shipConfig.fuelLoadingPercentage = 125f;
                    shipConfig.routeProfile = RouteProfile.DirectMaximumBurn;
                    break;
                case 5: // Próxima Centauri (Alcubierre Warp 10c)
                    selectedPropulsion = PropulsionMode.WarpDrive;
                    selectedDestinationIndex = 14; // Próxima Centauri
                    shipConfig.rocketType = RocketArchitecture.WarpDriveShip;
                    shipConfig.fuelType = FuelType.ExoticCasimirVacuum;
                    shipConfig.thermalProtection = ThermalProtectionSystem.UHTC_HafniumCarbide_100mm;
                    shipConfig.materialTier = ShipMaterialTier.Tier3_Metamaterial_Casimir;
                    shipConfig.shieldThickness = ShieldThickness.None_0cm;
                    shipConfig.fuelLoadingPercentage = 100f;
                    shipConfig.routeProfile = RouteProfile.DirectMaximumBurn;
                    break;
            }

            SetDestination(selectedDestinationIndex);
            if (cockpitManager != null) cockpitManager.SetCockpitMode(selectedPropulsion);
        }

        /// <summary>
        /// Carga y prepara una de las 5 simulaciones precalibradas dando paso al protocolo de verificación pre-vuelo.
        /// </summary>
        public void LoadPresetSimulation(int presetNumber)
        {
            ApplyPresetConfiguration(presetNumber);
            currentPerspective = ViewPerspective.CockpitPOV3D;
            ApplyPerspective();
            OpenPreFlightChecklist();
        }

        /// <summary>
        /// Prepara la simulación personalizada y abre el protocolo de verificación pre-vuelo.
        /// </summary>
        public void StartCustomSimulation()
        {
            SetDestination(selectedDestinationIndex);
            currentPerspective = ViewPerspective.CockpitPOV3D;
            if (cockpitManager != null) cockpitManager.SetCockpitMode(selectedPropulsion);
            ApplyPerspective();
            OpenPreFlightChecklist();
        }

        /// <summary>
        /// Abre el modal interactivo de lista de comprobación de subsistemas previo al despegue.
        /// </summary>
        public void OpenPreFlightChecklist()
        {
            currentUIState = SimulationUIState.PreFlightChecklist;
            for (int i = 0; i < preFlightChecks.Length; i++)
            {
                preFlightChecks[i] = false;
            }
            launchCountdownTimer = -1f;
            launchBannerFadeTimer = 0f;
            isInspectingDashboard = false;
        }

        /// <summary>
        /// Inicia la cuenta regresiva cinemática de despegue (T-3, T-2, T-1, ¡IGNICIÓN!).
        /// </summary>
        public void StartLaunchCountdown()
        {
            currentUIState = SimulationUIState.InFlight;
            currentPerspective = ViewPerspective.CockpitPOV3D;
            ApplyPerspective();
            isPaused = false;
            isFlying = false;
            flightProgress = 0f;
            launchCountdownTimer = LaunchCountdownDuration;
            launchBannerFadeTimer = 0f;
            isInspectingDashboard = false;
            if (cockpitManager != null)
            {
                cockpitManager.LookForward();
                cockpitManager.TriggerCatastropheShake(true, 0.25f);
            }
        }

        /// <summary>
        /// Obtiene la duración realista base en segundos según el cuerpo astronómico objetivo.
        /// </summary>
        public float GetMissionBaseDurationSeconds()
        {
            if (targetBody == null) return 180f;

            if (selectedPropulsion == PropulsionMode.WarpDrive)
            {
                return 90f; // 1.5 minutos nominales para viajes de curvatura Warp
            }

            string name = targetBody.bodyName.ToLower();
            if (name.Contains("luna")) return 120f; // 2 minutos para la Luna
            if (name.Contains("marte") || name.Contains("venus") || name.Contains("mercurio")) return 180f; // 3 minutos para planetas interiores
            if (name.Contains("júpiter") || name.Contains("jupiter") || name.Contains("saturno")) return 210f; // 3.5 minutos para gigantes gaseosos
            return 240f; // 4 minutos para espacio profundo / Próxima Centauri
        }

        public string GetPhysicalFlightTimeDescription(PropulsionMode mode, double distanceKm)
        {
            if (distanceKm <= 0.0) return "0 s (En órbita terrestre)";

            switch (mode)
            {
                case PropulsionMode.Chemical:
                    // v ~ 17 km/s (escape hiperbólico estándar)
                    double tSec = distanceKm / 17.0;
                    if (tSec < 86400.0) return $"{tSec / 3600.0:F1} horas ({tSec / 86400.0:F2} días)";
                    if (tSec < 86400.0 * 365.25) return $"{tSec / 86400.0:F1} días ({(tSec / 86400.0 / 30.437):F1} meses)";
                    double years = tSec / (86400.0 * 365.25);
                    if (years > 1000.0)
                        return $"~{years:N0} AÑOS (VUELO GENERACIONAL - Inviable biológicamente sin estasis)";
                    return $"{years:F1} años terrestres a 17 km/s";

                case PropulsionMode.Antimatter:
                    // v = 0.50c = 149.896 km/s
                    double vAnti = 0.50 * SpaceConstants.SpeedOfLight_kms;
                    double tAntiSec = distanceKm / vAnti;
                    double gamma = 1.0 / Math.Sqrt(1.0 - 0.25); // 1.1547
                    double tShipSec = tAntiSec / gamma;
                    if (tAntiSec < 86400.0) return $"{tAntiSec / 3600.0:F1} horas";
                    if (tAntiSec < 86400.0 * 365.25) return $"{tAntiSec / 86400.0:F1} días (Tierra) | {tShipSec / 86400.0:F1} días (Tripulación tau)";
                    double yEarth = tAntiSec / (86400.0 * 365.25);
                    double yShip = tShipSec / (86400.0 * 365.25);
                    return $"{yEarth:F1} años (Tierra) | {yShip:F1} años (Tripulación tau, dilatación gamma = 1.15)";

                case PropulsionMode.WarpDrive:
                    // v = 10c = 2.997.924 km/s
                    double vWarp = 10.0 * SpaceConstants.SpeedOfLight_kms;
                    double tWarpSec = distanceKm / vWarp;
                    if (tWarpSec < 86400.0) return $"{tWarpSec / 3600.0:F1} horas ({tWarpSec / 60.0:F1} minutos)";
                    if (tWarpSec < 86400.0 * 365.25) return $"{tWarpSec / 86400.0:F1} días (Tiempo Tierra y Tripulación idénticos por curvatura métrica pura)";
                    double yWarp = tWarpSec / (86400.0 * 365.25);
                    return $"{yWarp:F1} años (Métrica Warp 10c, 0.00g inercial puro)";
            }
            return "";
        }

        public void SetDestinationByName(string keyword)
        {
            if (celestialBodies == null) return;
            for (int i = 0; i < celestialBodies.Count; i++)
            {
                if (celestialBodies[i].bodyName.ToLower().Contains(keyword.ToLower()))
                {
                    SetDestination(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Aplica las leyes físicas y matemáticas de evolución del sistema durante el vuelo.
        /// </summary>
        public void Simulate()
        {
            if (targetBody == null) return;

            double totalDistance = targetBody.distanceFromEarthKm;
            double effectiveDryMass = shipConfig.GetTotalDryMassKg();
            FlightCalculation calc = PropulsionPhysics.CalculateTrip(selectedPropulsion, totalDistance, effectiveDryMass, shipConfig.fuelLoadingPercentage);

            // Avance temporal con duración realista y soporte de Rebobinado (Rewind)
            float baseDuration = GetMissionBaseDurationSeconds();
            float progressStep = (Time.deltaTime / baseDuration) * Mathf.Max(0.1f, timeWarpMultiplier);

            if (isRewinding)
            {
                flightProgress -= progressStep;
                if (flightProgress <= 0f)
                {
                    flightProgress = 0f;
                    isRewinding = false;
                }
            }
            else
            {
                flightProgress += progressStep;
                if (flightProgress >= 1.0f)
                {
                    flightProgress = 1.0f;
                    isFlying = false;
                    bool canLand = targetBody != null && targetBody.canLandOnSurface;
                    if (canLand)
                    {
                        flightStatus = $"¡ATERRIZAJE EXITOSO EN LA SUPERFICIE DE {targetBody?.bodyName.ToUpper()}!";
                    }
                    else
                    {
                        flightStatus = $"¡INSERCIÓN ORBITAL CIENTÍFICA ESTABLE EN {targetBody?.bodyName.ToUpper()}!";
                    }
                    isMissionSuccessModalOpen = true;
                    Debug.Log($"[VIAJE COMPLETADO] {flightStatus}");
                }
                else
                {
                    flightStatus = $"En tránsito hacia {targetBody.bodyName}... ({(flightProgress * 100f):F1}%)";
                }
            }

            // Telemetría matemática exacta
            currentDistanceCoveredKm = totalDistance * flightProgress;
            currentRemainingDistanceKm = totalDistance * (1.0 - flightProgress);
            flightElapsedTimeEarth = calc.travelTimeEarthSeconds * flightProgress;
            flightElapsedTimeShip = calc.travelTimeShipSeconds * flightProgress;
            currentVelocityKms = calc.maxVelocityKms;
            currentLorentzGamma = calc.lorentzGamma;
            currentGForce = calc.feltGForce;

            if (selectedPropulsion == PropulsionMode.Chemical || selectedPropulsion == PropulsionMode.Antimatter)
            {
                // El combustible total embarcado incluye el margen/sobrecarga del usuario (ej. 120%)
                double totalLoadedFuelKg = calc.fuelMassKg * (shipConfig.fuelLoadingPercentage / 100.0);
                // El consumo en vuelo quema el propelente necesario para la misión
                double consumedTripFuelKg = calc.fuelMassKg * Mathf.Clamp01(flightProgress);
                currentRemainingFuelKg = Math.Max(0.0, totalLoadedFuelKg - consumedTripFuelKg);
            }
            else
            {
                currentRemainingFuelKg = 0.0; // Warp no gasta masa de reacción
            }

            // Interpolación cinemática de la nave en coordenadas espaciales según el Perfil de Ruta
            if (spaceshipObj != null && earthBody != null)
            {
                Vector3 startPos = (departureOriginPos != Vector3.zero) ? departureOriginPos : earthBody.transform.position;
                Vector3 endPos = targetBody.transform.position;
                Vector3 currentPos;
                Vector3 tangentDir;
                float t = flightProgress;

                if (selectedPropulsion == PropulsionMode.WarpDrive || shipConfig.routeProfile == RouteProfile.DirectMaximumBurn)
                {
                    currentPos = Vector3.Lerp(startPos, endPos, t);
                    tangentDir = endPos - startPos;
                }
                else
                {
                    Vector3 midArc;
                    switch (shipConfig.routeProfile)
                    {
                        case RouteProfile.HohmannElliptic:
                            midArc = Vector3.Lerp(startPos, endPos, 0.5f) + Vector3.up * 4.2f;
                            break;
                        case RouteProfile.GravityAssistSlingshot:
                            midArc = Vector3.Lerp(startPos, endPos, 0.35f) - Vector3.up * 3.8f;
                            break;
                        case RouteProfile.CustomManualWaypoint:
                            float infl = Mathf.Clamp(shipConfig.manualInflectionT, 0.05f, 0.95f);
                            midArc = Vector3.Lerp(startPos, endPos, infl) + Vector3.up * shipConfig.manualArcHeight;
                            break;
                        case RouteProfile.SafeShieldedCorridor:
                        default:
                            midArc = Vector3.Lerp(startPos, endPos, 0.5f) + Vector3.up * 2.5f;
                            break;
                    }

                    currentPos = (1f - t) * (1f - t) * startPos + 2f * (1f - t) * t * midArc + t * t * endPos;
                    tangentDir = 2f * (1f - t) * (midArc - startPos) + 2f * t * (endPos - midArc);
                }

                spaceshipObj.transform.position = currentPos;
                if (tangentDir != Vector3.zero)
                {
                    float angle = Mathf.Atan2(tangentDir.y, tangentDir.x) * Mathf.Rad2Deg;
                    spaceshipObj.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
                }
            }

            // Evaluación de Entorno Astrofísico y Supervivencia (Radiación, Asteroides, Púlsar, Agujero Negro)
            Vector2 ship2D = (spaceshipObj != null) ? new Vector2(spaceshipObj.transform.position.x, spaceshipObj.transform.position.y) : Vector2.zero;
            Vector2 sunPos = (sunBody != null) ? new Vector2(sunBody.transform.position.x, sunBody.transform.position.y) : new Vector2(-14f, 0f);
            Vector2 earthPos = (earthBody != null) ? new Vector2(earthBody.transform.position.x, earthBody.transform.position.y) : Vector2.zero;

            float distToEarth = Vector2.Distance(ship2D, earthPos);
            bool insideVanAllen = (distToEarth < 2.5f && flightProgress > 0.005f && selectedPropulsion != PropulsionMode.WarpDrive);

            float distToSun = Vector2.Distance(ship2D, sunPos);
            bool insideSolarCorona = (distToSun < 9.5f);
            bool insideAsteroidBelt = (distToSun >= 26f && distToSun <= 36f);

            bool insideJupiterMag = false;
            for (int i = 0; i < celestialBodies.Count; i++)
            {
                if (celestialBodies[i].bodyName.Contains("Júpiter"))
                {
                    if (Vector2.Distance(ship2D, new Vector2(celestialBodies[i].transform.position.x, celestialBodies[i].transform.position.y)) < 4.8f)
                    {
                        insideJupiterMag = true;
                    }
                    break;
                }
            }

            Vector2 pulsarPos = new Vector2(105f, -10f);
            bool insidePulsarBeam = (Vector2.Distance(ship2D, pulsarPos) < 7.5f);

            Vector2 blackHolePos = new Vector2(140f, 16f);
            bool insideBlackHoleAccretion = (Vector2.Distance(ship2D, blackHolePos) < 9.0f);

            string currentHazardName = "";
            if (insidePulsarBeam) currentHazardName = "⚡ Haz Gamma de Púlsar PSR B1257+12";
            else if (insideBlackHoleAccretion) currentHazardName = "🕳️ Disco de Acreción de Cygnus X-1";
            else if (insideSolarCorona) currentHazardName = "☀️ Corona Solar Térmica (T > 2.500 K)";
            else if (insideJupiterMag) currentHazardName = "☣️ Magnetosfera Joviana Sincrotrón";
            else if (insideAsteroidBelt) currentHazardName = "☄️ Cinturón Principal de Asteroides (Polvo ISM)";
            else if (insideVanAllen) currentHazardName = "⚠️ Cinturón de Van Allen Terrestre";

            double distSunKm = SpaceConstants.KmPerAU * Math.Max(0.1, (distToSun / 14.0));

            survivalSystem.UpdateSurvivalTick(
                Time.deltaTime * timeWarpMultiplier,
                flightProgress,
                currentVelocityKms,
                calc.velocityFractionC,
                currentGForce,
                selectedPropulsion,
                shipConfig,
                distSunKm,
                insideVanAllen,
                insideJupiterMag,
                insideSolarCorona,
                insideAsteroidBelt,
                insidePulsarBeam,
                insideBlackHoleAccretion,
                currentHazardName
            );

            // Transmitir sacudida catastrófica a la cabina si está en secuencia de explosión
            if (cockpitManager != null)
            {
                cockpitManager.TriggerCatastropheShake(survivalSystem.isCatastropheActive, survivalSystem.catastropheTimer);
            }

            if (survivalSystem.isMissionFailed)
            {
                isFlying = false;
                flightStatus = "MISIÓN ABORTADA: " + survivalSystem.failureReason;
            }
        }

        /// <summary>
        /// Representa visualmente el estado del vuelo, la trayectoria con marcas y la estela.
        /// </summary>
        public void DrawView()
        {
            if (trajectoryLine != null && earthBody != null && targetBody != null)
            {
                Vector3 start = earthBody.transform.position;
                Vector3 end = targetBody.transform.position;

                if (selectedPropulsion == PropulsionMode.WarpDrive || shipConfig.routeProfile == RouteProfile.DirectMaximumBurn)
                {
                    trajectoryLine.positionCount = 2;
                    trajectoryLine.SetPosition(0, start);
                    trajectoryLine.SetPosition(1, end);
                }
                else
                {
                    int segments = 28;
                    trajectoryLine.positionCount = segments;
                    Vector3 midArc;
                    switch (shipConfig.routeProfile)
                    {
                        case RouteProfile.HohmannElliptic:
                            midArc = Vector3.Lerp(start, end, 0.5f) + Vector3.up * 4.2f;
                            break;
                        case RouteProfile.GravityAssistSlingshot:
                            midArc = Vector3.Lerp(start, end, 0.35f) - Vector3.up * 3.8f;
                            break;
                        case RouteProfile.CustomManualWaypoint:
                            float infl = Mathf.Clamp(shipConfig.manualInflectionT, 0.05f, 0.95f);
                            midArc = Vector3.Lerp(start, end, infl) + Vector3.up * shipConfig.manualArcHeight;
                            break;
                        case RouteProfile.SafeShieldedCorridor:
                        default:
                            midArc = Vector3.Lerp(start, end, 0.5f) + Vector3.up * 2.5f;
                            break;
                    }

                    for (int i = 0; i < segments; i++)
                    {
                        float t = (float)i / (segments - 1);
                        Vector3 p = (1f - t) * (1f - t) * start + 2f * (1f - t) * t * midArc + t * t * end;
                        trajectoryLine.SetPosition(i, p);
                    }
                }

                Color trajColor = selectedPropulsion switch
                {
                    PropulsionMode.WarpDrive => new Color(0f, 1f, 1f, 0.85f),
                    PropulsionMode.Antimatter => new Color(0.85f, 0.3f, 1f, 0.85f),
                    _ => new Color(1f, 0.6f, 0.15f, 0.85f)
                };
                trajectoryLine.startColor = trajColor;
                trajectoryLine.endColor = trajColor;

                // Actualizar marcas de porcentaje a lo largo de la trayectoria (25%, 50%, 75%)
                if (tickMarkers != null && tickMarkers.Length == 3)
                {
                    Vector3 perp = Vector3.Cross(end - start, Vector3.forward).normalized * 0.5f;
                    float[] fractions = new float[] { 0.25f, 0.50f, 0.75f };
                    for (int i = 0; i < 3; i++)
                    {
                        Vector3 mid;
                        if (shipConfig.routeProfile == RouteProfile.SafeShieldedCorridor && selectedPropulsion != PropulsionMode.WarpDrive)
                        {
                            Vector3 midArc = Vector3.Lerp(start, end, 0.5f) + Vector3.up * 1.5f;
                            float t = fractions[i];
                            mid = (1f - t) * (1f - t) * start + 2f * (1f - t) * t * midArc + t * t * end;
                        }
                        else
                        {
                            mid = Vector3.Lerp(start, end, fractions[i]);
                        }
                        tickMarkers[i].SetPosition(0, mid - perp);
                        tickMarkers[i].SetPosition(1, mid + perp);
                        tickMarkers[i].startColor = trajColor;
                        tickMarkers[i].endColor = trajColor;
                    }
                }
            }

            // Anillo toroidal de curvatura de Alcubierre
            if (warpBubbleVisual != null)
            {
                bool showWarp = isFlying && selectedPropulsion == PropulsionMode.WarpDrive;
                warpBubbleVisual.SetActive(showWarp);
                if (showWarp)
                {
                    float pulse = 1.0f + 0.15f * Mathf.Sin(Time.time * (8f * Mathf.Max(1f, timeWarpMultiplier)));
                    warpBubbleVisual.transform.localScale = Vector3.one * (1.6f * pulse);
                }
            }
        }

        public void StartFlight()
        {
            if (targetBody == null) return;
            survivalSystem.Reset();
            isFlying = true;
            isRewinding = false;
            flightProgress = 0f;
            flightStatus = $"Iniciando trayectoria hacia {targetBody.bodyName} vía {selectedPropulsion}...";

            if (shipTrail != null) shipTrail.Clear();

            if (currentPerspective == ViewPerspective.TacticalMap2D)
            {
                if (spaceCamera != null && spaceshipObj != null)
                {
                    spaceCamera.SetMode(CameraFollowMode.FollowShip, spaceshipObj.transform, 6.0f);
                }
            }
            else
            {
                if (spaceCamera != null) spaceCamera.enabled = false;
                if (cockpitManager != null)
                {
                    cockpitManager.gameObject.SetActive(true);
                    cockpitManager.EnableCockpitPOV();
                    cockpitManager.SetCockpitMode(selectedPropulsion);
                }
            }

            Debug.Log($"[DESPEGUE] Vuelo iniciado hacia {targetBody.bodyName}");
        }

        public void ResetToEarth()
        {
            isFlying = false;
            isRewinding = false;
            launchCountdownTimer = -1f;
            launchBannerFadeTimer = 0f;
            isInspectingDashboard = false;
            survivalSystem.Reset();
            flightProgress = 0f;
            currentDistanceCoveredKm = 0.0;
            if (cockpitManager != null)
            {
                cockpitManager.TriggerCatastropheShake(false, 0f);
                cockpitManager.LookForward();
            }
            if (targetBody != null) currentRemainingDistanceKm = targetBody.distanceFromEarthKm;
            flightElapsedTimeEarth = 0.0;
            flightElapsedTimeShip = 0.0;
            flightStatus = "En órbita terrestre, trayectoria en reposo.";

            if (targetBody != null && (selectedPropulsion == PropulsionMode.Chemical || selectedPropulsion == PropulsionMode.Antimatter))
            {
                FlightCalculation calc = PropulsionPhysics.CalculateTrip(selectedPropulsion, targetBody.distanceFromEarthKm, shipConfig.GetTotalDryMassKg(), shipConfig.fuelLoadingPercentage);
                currentRemainingFuelKg = calc.fuelMassKg * (shipConfig.fuelLoadingPercentage / 100.0);
            }

            if (spaceshipObj != null && earthBody != null)
            {
                spaceshipObj.transform.position = earthBody.transform.position + new Vector3(0.9f, 0.3f, 0f);
                spaceshipObj.transform.rotation = Quaternion.identity;
            }

            if (shipTrail != null) shipTrail.Clear();

            if (currentPerspective == ViewPerspective.TacticalMap2D)
            {
                if (spaceCamera != null && earthBody != null)
                {
                    spaceCamera.SetMode(CameraFollowMode.FocusEarth, earthBody.transform, 6.0f);
                }
            }
            else
            {
                if (spaceCamera != null) spaceCamera.enabled = false;
                if (cockpitManager != null)
                {
                    cockpitManager.gameObject.SetActive(true);
                    cockpitManager.EnableCockpitPOV();
                    cockpitManager.SetCockpitMode(selectedPropulsion);
                }
            }
        }

        public void SetDestination(int index)
        {
            if (index < 0 || index >= celestialBodies.Count) return;

            selectedDestinationIndex = index;
            targetBody = celestialBodies[index];
            currentRemainingDistanceKm = targetBody.distanceFromEarthKm;

            if (cockpitManager != null && targetBody != null)
            {
                Color targetColor = (fullCosmosData != null && index >= 0 && index < fullCosmosData.Length)
                    ? fullCosmosData[index].color
                    : targetBody.orbitColor;
                cockpitManager.SetTargetDestination(targetBody.bodyName, targetColor);
            }

            if (!isFlying)
            {
                ResetToEarth();
            }

            Debug.Log($"Destino seleccionado: {targetBody.bodyName} ({SpaceConstants.FormatDistance(targetBody.distanceFromEarthKm)})");
        }

        // --- DEFINICIÓN PRECISA DEL ESCENARIO CÓSMICO (POSICIONES Y TAMAÑOS NORMALIZADOS) ---
        private void InitializeCosmosData()
        {
            // El Sol está situado en (-14, 0). La Tierra (1 UA) está en (0, 0). Distancia Sol-Tierra = 14 unidades Unity.
            // Todos los planetas se ubican en radios orbitales precisos con ángulos estéticos no superpuestos.
            fullCosmosData = new CelestialData[]
            {
                // 1. Sol
                new CelestialData {
                    name = "Sol (Centro del Sistema)",
                    distanceKm = 149597870.7,
                    distanceAU = 1.0,
                    visualPos = new Vector2(-14.0f, 0.0f),
                    visualDiameter = 3.6f,
                    color = Color.yellow,
                    category = BodyCategory.Star,
                    description = "Estrella central. Masa: 1.989 × 10³⁰ kg. Fuente gravitacional primaria del sistema solar."
                },
                // 2. Mercurio (~0.39 UA -> radio 5.4 desde el Sol)
                new CelestialData {
                    name = "Mercurio",
                    distanceKm = 91691000.0,
                    distanceAU = 0.387,
                    visualPos = new Vector2(-10.2f, 3.8f),
                    visualDiameter = 0.55f,
                    color = new Color(0.75f, 0.75f, 0.75f),
                    category = BodyCategory.Planet,
                    description = "Planeta rocoso más interior. Sin atmósfera y con gran oscilación térmica (-180 °C a 430 °C)."
                },
                // 3. Venus (~0.72 UA -> radio 9.8 desde el Sol)
                new CelestialData {
                    name = "Venus",
                    distanceKm = 41400000.0,
                    distanceAU = 0.723,
                    visualPos = new Vector2(-8.4f, -8.0f),
                    visualDiameter = 0.95f,
                    color = new Color(0.95f, 0.85f, 0.55f),
                    category = BodyCategory.Planet,
                    description = "Efecto invernadero desbocado con densa atmósfera de CO2. Presión 92 atm y 465 °C."
                },
                // 4. Luna (Satélite natural de la Tierra en el origen)
                new CelestialData {
                    name = "Luna",
                    distanceKm = 384400.0,
                    distanceAU = 0.00257,
                    visualPos = new Vector2(1.8f, 1.2f),
                    visualDiameter = 0.40f,
                    color = new Color(0.85f, 0.85f, 0.85f),
                    category = BodyCategory.Moon,
                    description = "Satélite natural terrestre a 384.400 km. Banco de pruebas para propulsión lunar."
                },
                // 5. Marte (~1.52 UA -> radio 21.0 desde el Sol)
                new CelestialData {
                    name = "Marte",
                    distanceKm = 225000000.0,
                    distanceAU = 1.524,
                    visualPos = new Vector2(5.7f, 7.2f),
                    visualDiameter = 0.75f,
                    color = new Color(1f, 0.4f, 0.2f),
                    category = BodyCategory.Planet,
                    description = "Planeta rojo con cañones colosales (Valles Marineris) y monte Olimpo."
                },
                // 6. Cinturón Principal de Asteroides (Ceres) (~2.8 UA -> radio 32.0 desde el Sol)
                new CelestialData {
                    name = "Cinturón de Asteroides (Ceres)",
                    distanceKm = 414000000.0,
                    distanceAU = 2.77,
                    visualPos = new Vector2(16.5f, -11.0f),
                    visualDiameter = 0.65f,
                    color = new Color(0.7f, 0.65f, 0.6f),
                    category = BodyCategory.DeepSpaceRegion,
                    description = "Región toroidal con millones de asteroides y el planeta enano Ceres."
                },
                // 7. Júpiter (~5.2 UA -> radio 46.0 desde el Sol)
                new CelestialData {
                    name = "Júpiter",
                    distanceKm = 778500000.0,
                    distanceAU = 5.204,
                    visualPos = new Vector2(30.4f, 11.9f),
                    visualDiameter = 2.20f,
                    color = new Color(0.9f, 0.7f, 0.5f),
                    category = BodyCategory.Planet,
                    description = "Gigante gaseoso colosal con 95 lunas y la emblemática Gran Mancha Roja."
                },
                // 8. Saturno (~9.58 UA -> radio 62.0 desde el Sol)
                new CelestialData {
                    name = "Saturno",
                    distanceKm = 1433000000.0,
                    distanceAU = 9.582,
                    visualPos = new Vector2(45.0f, -19.2f),
                    visualDiameter = 2.00f,
                    color = new Color(0.95f, 0.88f, 0.65f),
                    category = BodyCategory.Planet,
                    description = "Magnífica joya del sistema solar con su vasto sistema de anillos de hielo y polvo."
                },
                // 9. Urano (~19.2 UA -> radio 80.0 desde el Sol)
                new CelestialData {
                    name = "Urano",
                    distanceKm = 2871000000.0,
                    distanceAU = 19.22,
                    visualPos = new Vector2(60.2f, 26.0f),
                    visualDiameter = 1.40f,
                    color = new Color(0.55f, 0.85f, 0.95f),
                    category = BodyCategory.Planet,
                    description = "Gigante de hielo con eje de rotación tumbado 98° y atmósfera helada de metano."
                },
                // 10. Neptuno (~30.0 UA -> radio 98.0 desde el Sol)
                new CelestialData {
                    name = "Neptuno",
                    distanceKm = 4495000000.0,
                    distanceAU = 30.05,
                    visualPos = new Vector2(81.8f, -20.4f),
                    visualDiameter = 1.40f,
                    color = new Color(0.25f, 0.45f, 0.95f),
                    category = BodyCategory.Planet,
                    description = "El planeta más lejano del Sistema Solar. Vientos supersónicos de más de 2.100 km/h."
                },
                // 11. Plutón (~40 UA -> radio 116.0 desde el Sol)
                new CelestialData {
                    name = "Plutón (Cinturón de Kuiper)",
                    distanceKm = 5906000000.0,
                    distanceAU = 39.48,
                    visualPos = new Vector2(100.8f, 16.2f),
                    visualDiameter = 0.50f,
                    color = new Color(0.75f, 0.65f, 0.55f),
                    category = BodyCategory.Planet,
                    description = "Planeta enano con glaciares de nitrógeno sólido (Tombaugh Regio)."
                },
                // 12. Cinturón de Kuiper Exterior (50 UA)
                new CelestialData {
                    name = "Cinturón de Kuiper Exterior",
                    distanceKm = 50.0 * SpaceConstants.KmPerAU,
                    distanceAU = 50.0,
                    visualPos = new Vector2(122.0f, -14.0f),
                    visualDiameter = 1.0f,
                    color = new Color(0.5f, 0.7f, 0.9f),
                    category = BodyCategory.DeepSpaceRegion,
                    description = "Enjambre de cometas y cuerpos transneptunianos congelados."
                },
                // 13. Heliopausa (120 UA)
                new CelestialData {
                    name = "Heliopausa (Frontera Solar)",
                    distanceKm = 120.0 * SpaceConstants.KmPerAU,
                    distanceAU = 120.0,
                    visualPos = new Vector2(142.0f, 18.0f),
                    visualDiameter = 1.2f,
                    color = new Color(0.4f, 0.8f, 1f),
                    category = BodyCategory.DeepSpaceRegion,
                    description = "Límite donde la presión del viento solar se equilibra con el medio interestelar."
                },
                // 14. Nube de Oort (10.000 UA)
                new CelestialData {
                    name = "Nube de Oort (Límite Gravitacional)",
                    distanceKm = 10000.0 * SpaceConstants.KmPerAU,
                    distanceAU = 10000.0,
                    visualPos = new Vector2(168.0f, -22.0f),
                    visualDiameter = 1.5f,
                    color = new Color(0.6f, 0.9f, 1f),
                    category = BodyCategory.DeepSpaceRegion,
                    description = "Nube esférica primordial de cometas que rodea todo el Sistema Solar."
                },
                // 15. Próxima Centauri (4.246 años luz)
                new CelestialData {
                    name = "Próxima Centauri",
                    distanceKm = 4.246 * SpaceConstants.KmPerLightYear,
                    distanceAU = 4.246 * SpaceConstants.AUPerLightYear,
                    visualPos = new Vector2(200.0f, 22.0f),
                    visualDiameter = 1.6f,
                    color = new Color(1f, 0.35f, 0.3f),
                    category = BodyCategory.InterstellarStar,
                    description = "Enana roja más cercana a nuestro Sol a 4.24 años luz. Alberga el exoplaneta Próxima b."
                },
                // 16. Alfa Centauri A/B (4.37 años luz)
                new CelestialData {
                    name = "Alfa Centauri A/B",
                    distanceKm = 4.37 * SpaceConstants.KmPerLightYear,
                    distanceAU = 4.37 * SpaceConstants.AUPerLightYear,
                    visualPos = new Vector2(214.0f, 28.0f),
                    visualDiameter = 1.8f,
                    color = new Color(1f, 0.9f, 0.4f),
                    category = BodyCategory.InterstellarStar,
                    description = "Sistema estelar binario de estrellas de tipo solar a 4.37 años luz."
                },
                // 17. Sirio (8.6 años luz)
                new CelestialData {
                    name = "Sirio (Estrella más brillante)",
                    distanceKm = 8.6 * SpaceConstants.KmPerLightYear,
                    distanceAU = 8.6 * SpaceConstants.AUPerLightYear,
                    visualPos = new Vector2(242.0f, -30.0f),
                    visualDiameter = 2.0f,
                    color = new Color(0.8f, 0.95f, 1f),
                    category = BodyCategory.InterstellarStar,
                    description = "Estrella blanca tipo A1V en Can Mayor, dos veces más masiva que el Sol."
                },
                // 18. Centro Galáctico (26.670 años luz)
                new CelestialData {
                    name = "Centro Galáctico (Sagitario A*)",
                    distanceKm = 26670.0 * SpaceConstants.KmPerLightYear,
                    distanceAU = 26670.0 * SpaceConstants.AUPerLightYear,
                    visualPos = new Vector2(285.0f, 35.0f),
                    visualDiameter = 2.8f,
                    color = new Color(0.9f, 0.3f, 1f),
                    category = BodyCategory.InterstellarStar,
                    description = "Agujero negro supermasivo de 4 millones de masas solares en el núcleo de la Vía Láctea."
                },
                // 19. Galaxia de Andrómeda (2.537.000 años luz)
                new CelestialData {
                    name = "Galaxia de Andrómeda (M31)",
                    distanceKm = 2537000.0 * SpaceConstants.KmPerLightYear,
                    distanceAU = 2537000.0 * SpaceConstants.AUPerLightYear,
                    visualPos = new Vector2(340.0f, -40.0f),
                    visualDiameter = 3.8f,
                    color = new Color(0.7f, 0.9f, 1f),
                    category = BodyCategory.Galaxy,
                    description = "Galaxia espiral gigante vecina con más de 1 billón de estrellas en el Grupo Local."
                }
            };
        }

        private void BuildCosmosEnvironment()
        {
            cosmos2DRoot = new GameObject("Cosmos2D_Root");

            // 1. Fondo estelar ultra amplio que cubre todo el plano de coordenadas
            GameObject bgObj = new GameObject("DeepSpaceBackground");
            bgObj.transform.SetParent(cosmos2DRoot.transform);
            bgObj.transform.position = new Vector3(80f, 0f, 20f);
            bgObj.transform.localScale = new Vector3(1200f, 750f, 1f);
            var bgRenderer = bgObj.AddComponent<SpriteRenderer>();
            bgRenderer.sprite = spaceBgSprite;
            bgRenderer.sortingOrder = -20;
            bgRenderer.color = new Color(1f, 1f, 1f, 0.40f);

            // 2. Tierra como origen de referencia en el centro (0, 0, 0)
            GameObject earthObj = new GameObject("Tierra (Origen)");
            earthObj.transform.SetParent(cosmos2DRoot.transform);
            earthObj.transform.position = Vector3.zero;
            earthBody = earthObj.AddComponent<CelestialBody>();
            earthBody.bodyName = "Tierra";
            earthBody.isReferenceOrigin = true;
            earthBody.realRadiusKm = 6371.0;
            earthBody.realDistanceAU = 1.0;
            earthBody.distanceFromEarthKm = 0.0;
            earthBody.visualRadius = 1.20f;
            earthBody.bodySprite = earthSprite;

            var earthRenderer = earthObj.GetComponent<SpriteRenderer>();
            earthRenderer.sprite = earthSprite;
            earthRenderer.sortingOrder = 3;

            Vector3 sunCenter = new Vector3(-14.0f, 0.0f, 0.0f);
            earthBody.ConfigurePhysicalProperties();
            earthBody.InitializeOrbit(sunCenter);

            // 3. Crear todos los cuerpos celestes con posiciones y escalas calibradas
            celestialBodies.Clear();

            foreach (var data in fullCosmosData)
            {
                GameObject obj = new GameObject(data.name);
                obj.transform.SetParent(cosmos2DRoot.transform);
                obj.transform.position = new Vector3(data.visualPos.x, data.visualPos.y, 0f);

                CelestialBody body = obj.AddComponent<CelestialBody>();
                body.bodyName = data.name;
                body.realDistanceAU = data.distanceAU;
                body.distanceFromEarthKm = data.distanceKm;
                body.visualRadius = data.visualDiameter;
                body.category = data.category;
                body.orbitColor = new Color(data.color.r, data.color.g, data.color.b, 0.35f);

                // Centro orbital: El Sol para planetas, o la Tierra para la Luna
                bool isMoon = data.name.Contains("Luna");
                body.orbitCenter = isMoon ? Vector3.zero : sunCenter;
                body.parentBody = isMoon ? earthBody : null;
                body.ConfigurePhysicalProperties();
                body.InitializeOrbit(sunCenter, isMoon ? earthBody : null);

                var ren = obj.GetComponent<SpriteRenderer>();
                ren.sortingOrder = 3;

                // Asignación de sprites normalizados
                if (data.name.Contains("Sol")) { ren.sprite = sunSprite; sunBody = body; }
                else if (data.name.Contains("Mercurio")) ren.sprite = mercurySprite;
                else if (data.name.Contains("Venus")) ren.sprite = venusSprite;
                else if (data.name.Contains("Luna")) ren.sprite = moonSprite;
                else if (data.name.Contains("Marte")) ren.sprite = marsSprite;
                else if (data.name.Contains("Ceres") || data.name.Contains("Asteroide")) ren.sprite = asteroidSprite;
                else if (data.name.Contains("Júpiter")) ren.sprite = jupiterSprite;
                else if (data.name.Contains("Saturno")) ren.sprite = saturnSprite;
                else if (data.name.Contains("Urano")) ren.sprite = uranusSprite;
                else if (data.name.Contains("Neptuno")) ren.sprite = neptuneSprite;
                else if (data.name.Contains("Plutón")) ren.sprite = plutoSprite;
                else
                {
                    ren.sprite = CreateCircularSprite(data.color);
                }

                celestialBodies.Add(body);
            }

            // 4. Generar Enjambre realista del Cinturón de Asteroides (entre Marte y Júpiter)
            SpawnAsteroidBeltSwarm(sunCenter, 28f, 35f, 40);
        }

        private void SpawnAsteroidBeltSwarm(Vector3 center, float rMin, float rMax, int count)
        {
            GameObject beltParent = new GameObject("Cinturón_Asteroides_Swarm");
            if (cosmos2DRoot != null) beltParent.transform.SetParent(cosmos2DRoot.transform);

            for (int i = 0; i < count; i++)
            {
                float angle = (i / (float)count) * Mathf.PI * 2f + UnityEngine.Random.Range(-0.06f, 0.06f);
                float radius = UnityEngine.Random.Range(rMin, rMax);
                Vector3 pos = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0.1f);

                GameObject ast = new GameObject($"Asteroide_{i}");
                ast.transform.SetParent(beltParent.transform);
                ast.transform.position = pos;
                float s = UnityEngine.Random.Range(0.20f, 0.40f);
                ast.transform.localScale = new Vector3(s, s, s);

                var sr = ast.AddComponent<SpriteRenderer>();
                sr.sprite = asteroidSprite;
                sr.sortingOrder = 2;
                sr.color = new Color(0.85f, 0.8f, 0.75f, 0.85f);

                asteroidBeltObjects.Add(ast);
            }
        }

        private void SetupSpaceship()
        {
            spaceshipObj = new GameObject("Nave Científica (Alcubierre)");
            if (cosmos2DRoot != null) spaceshipObj.transform.SetParent(cosmos2DRoot.transform);
            spaceshipObj.transform.position = new Vector3(0.9f, 0.3f, 0f);
            spaceshipObj.transform.localScale = Vector3.one * 0.55f;

            var sr = spaceshipObj.AddComponent<SpriteRenderer>();
            sr.sprite = shipSprite;
            sr.sortingOrder = 6;

            // Anillo toroidal de curvatura de Alcubierre
            warpBubbleVisual = new GameObject("WarpBubbleEffect");
            warpBubbleVisual.transform.SetParent(spaceshipObj.transform);
            warpBubbleVisual.transform.localPosition = Vector3.zero;

            var bubbleRenderer = warpBubbleVisual.AddComponent<SpriteRenderer>();
            bubbleRenderer.sprite = CreateGlowRingSprite(new Color(0f, 0.85f, 1f, 0.65f));
            bubbleRenderer.sortingOrder = 5;
            warpBubbleVisual.SetActive(false);

            // Estela de trayectoria detrás de la nave
            shipTrail = spaceshipObj.AddComponent<TrailRenderer>();
            shipTrail.time = 6.0f;
            shipTrail.startWidth = 0.15f;
            shipTrail.endWidth = 0.02f;
            shipTrail.material = new Material(Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/Unlit"));
            shipTrail.startColor = new Color(0f, 1f, 1f, 0.85f);
            shipTrail.endColor = new Color(0.5f, 0f, 1f, 0f);
        }

        private void SetupTrajectoryAndTrail()
        {
            GameObject lineObj = new GameObject("TrajectoryLine");
            if (cosmos2DRoot != null) lineObj.transform.SetParent(cosmos2DRoot.transform);
            trajectoryLine = lineObj.AddComponent<LineRenderer>();
            trajectoryLine.useWorldSpace = true;
            trajectoryLine.startWidth = 0.06f;
            trajectoryLine.endWidth = 0.06f;
            trajectoryLine.sortingOrder = 2;

            var mat = new Material(Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/Unlit"));
            trajectoryLine.material = mat;

            // Crear marcas de distancia a lo largo de la trayectoria (25%, 50%, 75%)
            tickMarkers = new LineRenderer[3];
            for (int i = 0; i < 3; i++)
            {
                GameObject tickObj = new GameObject($"TrajectoryTick_{i}");
                tickObj.transform.SetParent(lineObj.transform);
                var lr = tickObj.AddComponent<LineRenderer>();
                lr.useWorldSpace = true;
                lr.positionCount = 2;
                lr.startWidth = 0.05f;
                lr.endWidth = 0.05f;
                lr.material = mat;
                lr.sortingOrder = 3;
                tickMarkers[i] = lr;
            }
        }

        private void SetupAstronomicalGrid()
        {
            GameObject gridObj = new GameObject("AstronomicalGrid");
            if (cosmos2DRoot != null) gridObj.transform.SetParent(cosmos2DRoot.transform);
            astroGrid = gridObj.AddComponent<AstronomicalGrid>();
            astroGrid.sunCenter = new Vector3(-14.0f, 0.0f, 0.0f);
            astroGrid.gridOpacity = 0.18f;
        }

        private void LoadAllSprites()
        {
            earthSprite = LoadTextureAsSprite("Assets/Textures/Earth.png");
            sunSprite = LoadTextureAsSprite("Assets/Textures/Sun.png");
            mercurySprite = LoadTextureAsSprite("Assets/Textures/Mercury.png");
            venusSprite = LoadTextureAsSprite("Assets/Textures/Venus.png");
            marsSprite = LoadTextureAsSprite("Assets/Textures/Mars.png");
            moonSprite = LoadTextureAsSprite("Assets/Textures/Moon.png");
            jupiterSprite = LoadTextureAsSprite("Assets/Textures/Jupiter.png");
            saturnSprite = LoadTextureAsSprite("Assets/Textures/Saturn.png");
            uranusSprite = LoadTextureAsSprite("Assets/Textures/Uranus.png");
            neptuneSprite = LoadTextureAsSprite("Assets/Textures/Neptune.png");
            plutoSprite = LoadTextureAsSprite("Assets/Textures/Pluto.png");
            asteroidSprite = LoadTextureAsSprite("Assets/Textures/Asteroid.png");
            shipSprite = LoadTextureAsSprite("Assets/Textures/Spaceship.png");
            spaceBgSprite = LoadTextureAsSprite("Assets/Textures/DeepSpaceBackground.jpg");
        }

        /// <summary>
        /// Carga cualquier textura y normaliza su escala con PixelsPerUnit = Max(ancho, alto),
        /// de modo que todas las texturas tengan exactamente 1.0 x 1.0 unidades de tamaño base nativo.
        /// Esto previene definitivamente que sprites de alta resolución (1024x1024) se vean colosales o superpuestos.
        /// </summary>
        private Sprite LoadTextureAsSprite(string path)
        {
            if (System.IO.File.Exists(path))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(bytes))
                {
                    float ppu = Mathf.Max(tex.width, tex.height);
                    return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu);
                }
            }
            return null;
        }

        private Sprite CreateCircularSprite(Color color)
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size);
            float radius = size * 0.46f;
            Vector2 center = new Vector2(size / 2f, size / 2f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), center);
                    if (d <= radius)
                    {
                        float edge = Mathf.Clamp01((radius - d) / 2f);
                        tex.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * edge));
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        private Sprite CreateGlowRingSprite(Color color)
        {
            int size = 128;
            Texture2D tex = new Texture2D(size, size);
            float rInner = size * 0.28f;
            float rOuter = size * 0.46f;
            Vector2 center = new Vector2(size / 2f, size / 2f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), center);
                    if (d >= rInner && d <= rOuter)
                    {
                        float mid = (rInner + rOuter) / 2f;
                        float intensity = 1f - (Mathf.Abs(d - mid) / (rOuter - mid));
                        tex.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * intensity));
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        // --- SISTEMA DE ESCALADO Y RENDERIZADO DE INTERFAZ GRÁFICA FUTURISTA ---
        private Vector2 destScrollPos = Vector2.zero;

        private Texture2D MakeSolidTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;
            Texture2D tex = new Texture2D(width, height);
            tex.SetPixels(pix);
            tex.Apply();
            return tex;
        }

        private void InitializeGUIStyles()
        {
            if (stylesInitialized) return;

            texPanelDark = MakeSolidTex(2, 2, new Color(0.04f, 0.07f, 0.13f, 0.94f));
            texPanelAccent = MakeSolidTex(2, 2, new Color(0.06f, 0.12f, 0.22f, 0.95f));
            texButtonPrimary = MakeSolidTex(2, 2, new Color(0.02f, 0.52f, 0.42f, 0.95f));
            texButtonPrimaryHover = MakeSolidTex(2, 2, new Color(0.05f, 0.72f, 0.58f, 1.0f));
            texButtonSecondary = MakeSolidTex(2, 2, new Color(0.09f, 0.16f, 0.28f, 0.92f));
            texButtonSecondaryHover = MakeSolidTex(2, 2, new Color(0.14f, 0.25f, 0.42f, 0.98f));
            texBarGreen = MakeSolidTex(2, 2, new Color(0.0f, 0.92f, 0.45f, 1.0f));
            texBarCyan = MakeSolidTex(2, 2, new Color(0.0f, 0.88f, 1.0f, 1.0f));
            texBarRed = MakeSolidTex(2, 2, new Color(1.0f, 0.22f, 0.22f, 1.0f));
            texBarYellow = MakeSolidTex(2, 2, new Color(1.0f, 0.78f, 0.15f, 1.0f));
            texAlertGreen = MakeSolidTex(2, 2, new Color(0.06f, 0.42f, 0.22f, 0.95f));
            texAlertAmber = MakeSolidTex(2, 2, new Color(0.85f, 0.52f, 0.05f, 0.95f));
            texAlertRed = MakeSolidTex(2, 2, new Color(0.85f, 0.12f, 0.12f, 0.95f));
            texVignette = MakeSolidTex(2, 2, new Color(0.01f, 0.02f, 0.04f, 0.85f));

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            titleStyle.normal.textColor = new Color(0.0f, 0.92f, 1.0f);

            subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleCenter
            };
            subtitleStyle.normal.textColor = new Color(0.70f, 0.84f, 0.96f);

            headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            headerStyle.normal.textColor = Color.white;

            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };
            bodyStyle.normal.textColor = new Color(0.85f, 0.92f, 0.98f);

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };
            smallStyle.normal.textColor = new Color(0.68f, 0.78f, 0.88f);

            panelBoxStyle = new GUIStyle(GUI.skin.box);
            panelBoxStyle.normal.background = texPanelDark;
            panelBoxStyle.border = new RectOffset(4, 4, 4, 4);

            presetCardStyle = new GUIStyle(GUI.skin.box);
            presetCardStyle.normal.background = texPanelAccent;
            presetCardStyle.border = new RectOffset(4, 4, 4, 4);
            presetCardStyle.padding = new RectOffset(14, 14, 8, 8);
            presetCardStyle.margin = new RectOffset(0, 0, 4, 6);

            presetTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                richText = true,
                wordWrap = false
            };
            presetTitleStyle.normal.textColor = Color.white;

            telemetryCardStyle = new GUIStyle(GUI.skin.box);
            telemetryCardStyle.normal.background = texPanelDark;
            telemetryCardStyle.border = new RectOffset(4, 4, 4, 4);

            buttonPrimaryStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            buttonPrimaryStyle.normal.background = texButtonPrimary;
            buttonPrimaryStyle.hover.background = texButtonPrimaryHover;
            buttonPrimaryStyle.normal.textColor = Color.white;
            buttonPrimaryStyle.hover.textColor = Color.white;

            buttonSecondaryStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
            buttonSecondaryStyle.normal.background = texButtonSecondary;
            buttonSecondaryStyle.hover.background = texButtonSecondaryHover;
            buttonSecondaryStyle.normal.textColor = new Color(0.85f, 0.92f, 1.0f);
            buttonSecondaryStyle.hover.textColor = Color.white;

            stylesInitialized = true;
        }

        private void BeginGUIScaling()
        {
            float targetW = 1920f;
            float targetH = 1080f;
            float scaleX = (float)Screen.width / targetW;
            float scaleY = (float)Screen.height / targetH;
            float scale = Mathf.Min(scaleX, scaleY);
            if (scale < 0.45f) scale = 0.45f;

            float offsetX = (Screen.width - targetW * scale) * 0.5f;
            float offsetY = (Screen.height - targetH * scale) * 0.5f;

            GUI.matrix = Matrix4x4.TRS(new Vector3(offsetX, offsetY, 0), Quaternion.identity, new Vector3(scale, scale, 1.0f));
        }

        private void EndGUIScaling()
        {
            GUI.matrix = Matrix4x4.identity;
        }

        private void OnGUI()
        {
            InitializeGUIStyles();
            BeginGUIScaling();

            switch (currentUIState)
            {
                case SimulationUIState.MainMenu:
                    DrawMainMenu();
                    break;

                case SimulationUIState.PreFlightChecklist:
                    DrawPreFlightChecklistModal();
                    break;

                case SimulationUIState.InFlight:
                    DrawInFlightUI();
                    if (survivalSystem != null && survivalSystem.isCatastropheActive)
                    {
                        DrawCatastropheAlarmSequence();
                    }
                    else if (survivalSystem != null && survivalSystem.isMissionFailed)
                    {
                        DrawMissionFailureModal();
                    }
                    break;

                case SimulationUIState.Paused:
                    DrawInFlightUI();
                    DrawPauseMenu();
                    break;
            }

            EndGUIScaling();
        }

        /// <summary>
        /// Pantalla de inicio centrada con las 5 simulaciones precalibradas, taller de ensamblaje y selector libre.
        /// </summary>
        private void DrawMainMenu()
        {
            GUI.DrawTexture(new Rect(0, 0, 1920, 1080), texVignette);

            float panelW = 1440f;
            float panelH = 920f;
            float panelX = (1920f - panelW) * 0.5f;
            float panelY = (1080f - panelH) * 0.5f;

            GUI.Box(new Rect(panelX, panelY, panelW, panelH), "", panelBoxStyle);

            // Título principal centrado
            GUILayout.BeginArea(new Rect(panelX + 25, panelY + 18, panelW - 50, 75));
            GUILayout.Label("🚀 SIMULADOR DE PROPULSIÓN ESPACIAL & MÉTRICA DE ALCUBIERRE", titleStyle);
            GUILayout.Space(2);
            GUILayout.Label("Laboratorio Interactivo de Física Astrodinámica: Química • Antimateria • Curvatura Espaciotemporal", subtitleStyle);
            GUILayout.EndArea();

            // Pestañas de Navegación del Menú Principal
            GUILayout.BeginArea(new Rect(panelX + 30, panelY + 95, panelW - 60, 44));
            GUILayout.BeginHorizontal();
            GUI.color = (mainMenuTab == 0) ? Color.cyan : Color.white;
            if (GUILayout.Button("🎯 1. MISIONES DESTACADAS (1 Clic)", buttonSecondaryStyle, GUILayout.Height(38))) mainMenuTab = 0;
            GUI.color = (mainMenuTab == 1) ? Color.cyan : Color.white;
            if (GUILayout.Button("🛠️ 2. TALLER DE ENSAMBLAJE & MATERIALES", buttonSecondaryStyle, GUILayout.Height(38))) mainMenuTab = 1;
            GUI.color = (mainMenuTab == 2) ? Color.cyan : Color.white;
            if (GUILayout.Button("⚙️ 3. RUTA Y DESTINOS LIBRES", buttonSecondaryStyle, GUILayout.Height(38))) mainMenuTab = 2;
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            float colY = panelY + 148;
            float colH = 745;

            if (mainMenuTab == 0)
            {
                // PESTAÑA 0: 5 MISIONES DESTACADAS (Acceso Inmediato)
                float leftColW = 750;
                GUILayout.BeginArea(new Rect(panelX + 30, colY, leftColW, colH));
                GUILayout.Label("<b>🎯 5 MISIONES Y SIMULACIONES DESTACADAS (1 Clic):</b>", headerStyle);
                GUILayout.Space(8);

                DrawPresetButton(1, "🌙 Simulación 1: Tierra ➔ Luna (Química Apolo/Artemis)", 
                                 "Casco Al-Li 2195 • Aceleración 3.5g • LOX/LH2 • Distancia: 384.400 km • Tiempo: 3 días", 
                                 new Color(1f, 0.65f, 0.2f));

                DrawPresetButton(2, "🔴 Simulación 2: Tierra ➔ Marte (Hohmann Interplanetario)", 
                                 "Casco Al-Li 2195 • Transferencia elíptica inercial • Distancia: 78.3M km • Tiempo: 214 días", 
                                 new Color(1f, 0.45f, 0.25f));

                DrawPresetButton(3, "⚡ Simulación 3: Tierra ➔ Júpiter (Antimateria Relativista)", 
                                 "Casco Tungsteno HTS • Haz de piones v = 0.15c • Trampa Penning • Distancia: 628M km • Tiempo: 3.8h", 
                                 new Color(0.85f, 0.35f, 1f));

                DrawPresetButton(4, "🌌 Simulación 4: Tierra ➔ Próxima Centauri (Antimateria 0.50c)", 
                                 "Casco Blindado Pesado • Dilatación gamma = 1.154 • Tierra: 8.5 años / Nave: 7.3 años • 4.25 ly", 
                                 new Color(0.70f, 0.40f, 1f));

                DrawPresetButton(5, "🛸 Simulación 5: Tierra ➔ Próxima Centauri (Alcubierre Warp 10c)", 
                                 "Metamaterial Cuántico Casimir • 10.0c • 0.00g inercial puro • d tau = dt exacto • Tiempo: 155 días", 
                                 new Color(0.0f, 0.95f, 1f));

                GUILayout.EndArea();

                // Columna Derecha: Vista Rápida de la Misión Activa
                float rightColW = panelW - leftColW - 90;
                float rightColX = panelX + leftColW + 55;
                GUILayout.BeginArea(new Rect(rightColX, colY, rightColW, colH));
                GUILayout.Label("<b>📊 ESTADO DE LA NAVE SELECCIONADA:</b>", headerStyle);
                GUILayout.Space(10);

                MaterialProperties matProps = ShipConfiguration.GetProperties(shipConfig.materialTier);
                GUILayout.Box($"<b>Chasis Actual:</b> <color=cyan>{matProps.name}</color>\n" +
                              $"<b>Arquitectura:</b> {shipConfig.rocketType} | <b>Propelente:</b> {shipConfig.fuelType}\n" +
                              $"<b>Masa Seca Base:</b> {matProps.baseDryMassKg:N0} kg | <b>Blindaje Gamma:</b> {(matProps.gammaShieldFactor * 100f):F1}%\n" +
                              $"<b>Espesor de Blindaje:</b> {shipConfig.GetShieldThicknessCm()} cm (+{shipConfig.CalculateShieldMassKg():N0} kg)\n" +
                              $"<b>Protección Térmica:</b> {shipConfig.GetThermalProtectionThicknessMm()} mm (+{shipConfig.CalculateThermalShieldMassKg():N0} kg)\n" +
                              $"<b>Carga de Combustible:</b> {shipConfig.fuelLoadingPercentage:F0}% ({((shipConfig.fuelLoadingPercentage > 130f) ? "Sobrecarga" : "Nominal")})\n" +
                              $"<b>Ruta Planificada:</b> {((shipConfig.routeProfile == RouteProfile.SafeShieldedCorridor) ? "Corredor Seguro (Defensa)" : (shipConfig.routeProfile == RouteProfile.HohmannElliptic ? "Transferencia Hohmann" : "Ruta Directa (Aceleración)"))}",
                              presetCardStyle, GUILayout.Height(155));

                GUILayout.Space(10);
                if (targetBody != null)
                {
                    FlightCalculation calc = PropulsionPhysics.CalculateTrip(selectedPropulsion, targetBody.distanceFromEarthKm, shipConfig.GetTotalDryMassKg(), shipConfig.fuelLoadingPercentage);
                    GUILayout.Box($"<b>Misión Actual:</b> {originBodyName} ➔ <color=cyan>{targetBody.bodyName}</color>\n" +
                                  $"<b>Distancia:</b> {SpaceConstants.FormatDistance(targetBody.distanceFromEarthKm)}\n" +
                                  $"<b>Modo Activo:</b> {calc.modeName}\n" +
                                  $"<b>Tiempo Real Físico:</b> <color=yellow>{SpaceConstants.FormatTime(calc.travelTimeEarthSeconds)}</color>\n" +
                                  $"<b>Duración en Simulación:</b> {GetMissionBaseDurationSeconds():N0} segundos a escala 1x\n" +
                                  $"<b>Propelente Requerido:</b> {calc.fuelMassKg:N0} kg",
                                  presetCardStyle, GUILayout.Height(135));
                }

                GUILayout.Space(14);
                GUI.color = new Color(0.2f, 1f, 0.6f);
                if (GUILayout.Button("▶ DESPEGAR AHORA MISMO", buttonPrimaryStyle, GUILayout.Height(52)))
                {
                    StartCustomSimulation();
                }
                GUI.color = Color.white;
                GUILayout.Space(10);
                if (GUILayout.Button("🛠️ Personalizar Materiales en Astillero ➔", buttonSecondaryStyle, GUILayout.Height(36)))
                {
                    mainMenuTab = 1;
                }
                GUILayout.EndArea();
            }
            else if (mainMenuTab == 1)
            {
                // PESTAÑA 1: TALLER DE ENSAMBLAJE & MATERIALES (ESTRICTAMENTE CONSTRUCCIÓN DE VEHÍCULO)
                // Se retiran destinos y perfiles de vuelo (pertenecientes a Pestaña 3).
                float colW = (panelW - 90) * 0.5f;

                // Columna Izquierda: Arquitectura de Cohete, Combustible y Material Estructural
                GUILayout.BeginArea(new Rect(panelX + 30, colY, colW, colH));

                // 1. Arquitectura de Fuselaje / Nave
                GUILayout.Label("<b>1. Arquitectura de Fuselaje / Nave (Chasis Base):</b>", headerStyle);
                GUILayout.Space(2);
                GUILayout.BeginVertical();
                bool isChemRock = (shipConfig.rocketType == RocketArchitecture.ChemicalRocket);
                bool isAntiRock = (shipConfig.rocketType == RocketArchitecture.AntimatterRocket);
                bool isWarpRock = (shipConfig.rocketType == RocketArchitecture.WarpDriveShip);

                GUI.color = isChemRock ? Color.cyan : Color.white;
                if (GUILayout.Button("🚀 Cohete Químico Industrial (Multietapa Al-Li 2195)", buttonSecondaryStyle, GUILayout.Height(32)))
                {
                    shipConfig.rocketType = RocketArchitecture.ChemicalRocket;
                    selectedPropulsion = PropulsionMode.Chemical;
                    shipConfig.fuelType = FuelType.ChemicalCriogenic;
                    shipConfig.materialTier = ShipMaterialTier.Tier1_Aerospace_AlLi;
                    shipConfig.shieldThickness = ShieldThickness.Standard_2_5cm;
                    shipConfig.thermalProtection = ThermalProtectionSystem.CeramicTiles_25mm;
                    shipConfig.fuelLoadingPercentage = 115f;
                    if (cockpitManager != null) cockpitManager.SetCockpitMode(selectedPropulsion);
                }

                GUI.color = isAntiRock ? Color.cyan : Color.white;
                if (GUILayout.Button("⚛️ Viga Tensora Valkyrie (Bobinas y Trampa Penning)", buttonSecondaryStyle, GUILayout.Height(32)))
                {
                    shipConfig.rocketType = RocketArchitecture.AntimatterRocket;
                    selectedPropulsion = PropulsionMode.Antimatter;
                    shipConfig.fuelType = FuelType.AntimatterPlasma;
                    shipConfig.materialTier = ShipMaterialTier.Tier2_Shielded_TungstenHTS;
                    shipConfig.shieldThickness = ShieldThickness.Reinforced_5cm;
                    shipConfig.thermalProtection = ThermalProtectionSystem.PICAX_Ablative_50mm;
                    shipConfig.fuelLoadingPercentage = 100f;
                    if (cockpitManager != null) cockpitManager.SetCockpitMode(selectedPropulsion);
                }

                GUI.color = isWarpRock ? Color.cyan : Color.white;
                if (GUILayout.Button("🌌 Nave Cuántica Alcubierre (Anillo Toroidal Casimir)", buttonSecondaryStyle, GUILayout.Height(32)))
                {
                    shipConfig.rocketType = RocketArchitecture.WarpDriveShip;
                    selectedPropulsion = PropulsionMode.WarpDrive;
                    shipConfig.fuelType = FuelType.ExoticCasimirVacuum;
                    shipConfig.materialTier = ShipMaterialTier.Tier3_Metamaterial_Casimir;
                    shipConfig.shieldThickness = ShieldThickness.Standard_2_5cm;
                    shipConfig.thermalProtection = ThermalProtectionSystem.UHTC_HafniumCarbide_100mm;
                    shipConfig.fuelLoadingPercentage = 100f;
                    if (cockpitManager != null) cockpitManager.SetCockpitMode(selectedPropulsion);
                }
                GUI.color = Color.white;
                GUILayout.EndVertical();

                GUILayout.Space(8);
                // 2. Tipo de Propelente Embarcado (Libre selección para permitir catástrofes)
                GUILayout.Label("<b>2. Tipo de Propelente Embarcado (Libre Selección):</b>", headerStyle);
                GUILayout.Space(2);
                GUILayout.BeginVertical();
                bool isChemFuel = (shipConfig.fuelType == FuelType.ChemicalCriogenic);
                bool isAntiFuel = (shipConfig.fuelType == FuelType.AntimatterPlasma);
                bool isExotFuel = (shipConfig.fuelType == FuelType.ExoticCasimirVacuum);

                GUI.color = isChemFuel ? Color.cyan : Color.white;
                if (GUILayout.Button("💧 Criogénico LOX/LH2 (450 s Isp - Combustión Química)", buttonSecondaryStyle, GUILayout.Height(30)))
                    shipConfig.fuelType = FuelType.ChemicalCriogenic;

                GUI.color = isAntiFuel ? Color.cyan : Color.white;
                if (GUILayout.Button("⚡ Plasma de Antimateria (Antiprotones 187 MeV / Haz Piones)", buttonSecondaryStyle, GUILayout.Height(30)))
                    shipConfig.fuelType = FuelType.AntimatterPlasma;

                GUI.color = isExotFuel ? Color.cyan : Color.white;
                if (GUILayout.Button("🔷 Fluctuaciones de Vacío Cuántico Casimir (rho < 0)", buttonSecondaryStyle, GUILayout.Height(30)))
                    shipConfig.fuelType = FuelType.ExoticCasimirVacuum;
                GUI.color = Color.white;
                GUILayout.EndVertical();

                GUILayout.Space(8);
                // 3. Material Estructural del Casco
                GUILayout.Label("<b>3. Material Estructural del Casco:</b>", headerStyle);
                GUILayout.Space(2);
                GUILayout.BeginVertical();
                bool isT1 = (shipConfig.materialTier == ShipMaterialTier.Tier1_Aerospace_AlLi);
                bool isT2 = (shipConfig.materialTier == ShipMaterialTier.Tier2_Shielded_TungstenHTS);
                bool isT3 = (shipConfig.materialTier == ShipMaterialTier.Tier3_Metamaterial_Casimir);

                GUI.color = isT1 ? Color.cyan : Color.white;
                if (GUILayout.Button("🚀 Tier 1: Aleación Al-Li 2195 & Titanio CFRP (Liviano)", buttonSecondaryStyle, GUILayout.Height(30)))
                    shipConfig.materialTier = ShipMaterialTier.Tier1_Aerospace_AlLi;

                GUI.color = isT2 ? Color.cyan : Color.white;
                if (GUILayout.Button("⚛️ Tier 2: Tungsteno, Berilio & Superconductores HTS", buttonSecondaryStyle, GUILayout.Height(30)))
                    shipConfig.materialTier = ShipMaterialTier.Tier2_Shielded_TungstenHTS;

                GUI.color = isT3 ? Color.cyan : Color.white;
                if (GUILayout.Button("🌌 Tier 3: Metamaterial Hiperbólico & Grafeno Casimir", buttonSecondaryStyle, GUILayout.Height(30)))
                    shipConfig.materialTier = ShipMaterialTier.Tier3_Metamaterial_Casimir;
                GUI.color = Color.white;
                GUILayout.EndVertical();

                MaterialProperties curProps = ShipConfiguration.GetProperties(shipConfig.materialTier);
                GUILayout.Space(4);
                GUILayout.Box($"<b>Chasis:</b> {curProps.name}\n" +
                              $"<b>Masa Base:</b> {curProps.baseDryMassKg:N0} kg | <b>Límite Seguro:</b> {(curProps.maxSafeSpeedFractionC >= 1.0 ? $"{curProps.maxSafeSpeedFractionC:F0}c" : $"{(curProps.maxSafeSpeedFractionC * 100f):F2}% c")}\n" +
                              $"<b>Atenuación Gamma:</b> {(curProps.gammaShieldFactor * 100f):F1}% | <b>Disipación Térmica:</b> {(curProps.thermalDissipationRating * 100f):F0}% | <b>Tracción:</b> {curProps.tensileYieldStrengthMPa:N0} MPa",
                              presetCardStyle, GUILayout.Height(62));

                GUILayout.EndArea();

                // Columna Derecha: TPS, HVL, Combustible y Diagnóstico Técnico
                float rightColX = panelX + colW + 60;
                GUILayout.BeginArea(new Rect(rightColX, colY, colW, colH));

                // 4. Sistema de Protección Térmica (TPS)
                GUILayout.Label("<b>4. Protección Térmica contra Fricción (TPS):</b>", headerStyle);
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle(shipConfig.thermalProtection == ThermalProtectionSystem.None_0mm, "0 mm", buttonSecondaryStyle, GUILayout.Height(28)))
                    shipConfig.thermalProtection = ThermalProtectionSystem.None_0mm;
                if (GUILayout.Toggle(shipConfig.thermalProtection == ThermalProtectionSystem.CeramicTiles_25mm, "25 mm", buttonSecondaryStyle, GUILayout.Height(28)))
                    shipConfig.thermalProtection = ThermalProtectionSystem.CeramicTiles_25mm;
                if (GUILayout.Toggle(shipConfig.thermalProtection == ThermalProtectionSystem.PICAX_Ablative_50mm, "50 mm", buttonSecondaryStyle, GUILayout.Height(28)))
                    shipConfig.thermalProtection = ThermalProtectionSystem.PICAX_Ablative_50mm;
                if (GUILayout.Toggle(shipConfig.thermalProtection == ThermalProtectionSystem.UHTC_HafniumCarbide_100mm, "100 mm", buttonSecondaryStyle, GUILayout.Height(28)))
                    shipConfig.thermalProtection = ThermalProtectionSystem.UHTC_HafniumCarbide_100mm;
                GUILayout.EndHorizontal();
                double tpsMass = shipConfig.CalculateThermalShieldMassKg();
                GUILayout.Label($"Escudo Térmico: <b>+{tpsMass:N0} kg</b> ({shipConfig.GetThermalProtectionThicknessMm()} mm)", smallStyle);

                GUILayout.Space(6);
                // 5. Blindaje de Radiación (HVL)
                GUILayout.Label("<b>5. Espesor de Blindaje de Radiación (HVL):</b>", headerStyle);
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle(shipConfig.shieldThickness == ShieldThickness.None_0cm, "0 cm", buttonSecondaryStyle, GUILayout.Height(28)))
                    shipConfig.shieldThickness = ShieldThickness.None_0cm;
                if (GUILayout.Toggle(shipConfig.shieldThickness == ShieldThickness.Standard_2_5cm, "2.5 cm", buttonSecondaryStyle, GUILayout.Height(28)))
                    shipConfig.shieldThickness = ShieldThickness.Standard_2_5cm;
                if (GUILayout.Toggle(shipConfig.shieldThickness == ShieldThickness.Reinforced_5cm, "5 cm", buttonSecondaryStyle, GUILayout.Height(28)))
                    shipConfig.shieldThickness = ShieldThickness.Reinforced_5cm;
                if (GUILayout.Toggle(shipConfig.shieldThickness == ShieldThickness.Heavy_10cm, "10 cm", buttonSecondaryStyle, GUILayout.Height(28)))
                    shipConfig.shieldThickness = ShieldThickness.Heavy_10cm;
                GUILayout.EndHorizontal();
                double shieldMass = shipConfig.CalculateShieldMassKg();
                double totalDry = shipConfig.GetTotalDryMassKg();
                GUILayout.Label($"Blindaje extra: <b>+{shieldMass:N0} kg</b> | <b>Masa Seca Total Nave (m_f): {totalDry:N0} kg</b>", smallStyle);

                GUILayout.Space(6);
                // 6. Gestión de Carga de Combustible (%)
                string section6Title = (selectedPropulsion == PropulsionMode.WarpDrive)
                    ? "<b>6. Banco de Energía Exótica Casimir Embarcado:</b>"
                    : "<b>6. Gestión de Carga de Propelente Embarcado:</b>";
                GUILayout.Label(section6Title, headerStyle);
                FlightCalculation calcForTarget = (targetBody != null)
                    ? PropulsionPhysics.CalculateTrip(selectedPropulsion, targetBody.distanceFromEarthKm, totalDry, 100f)
                    : default;
                double nominalFuel = calcForTarget.fuelMassKg;
                double actualLoadedFuel = nominalFuel * (shipConfig.fuelLoadingPercentage / 100.0);

                GUILayout.BeginHorizontal();
                string loadDesc = (selectedPropulsion == PropulsionMode.WarpDrive)
                    ? $"Carga Banco: <b>{shipConfig.fuelLoadingPercentage:F0}%</b> ({calcForTarget.exoticEnergyJoules * (shipConfig.fuelLoadingPercentage / 100.0):E2} J)"
                    : $"Carga: <b>{shipConfig.fuelLoadingPercentage:F0}%</b> ({actualLoadedFuel:N0} kg)";
                GUILayout.Label(loadDesc, bodyStyle, GUILayout.Width(220));
                shipConfig.fuelLoadingPercentage = GUILayout.HorizontalSlider(shipConfig.fuelLoadingPercentage, 20f, 250f);
                GUILayout.EndHorizontal();

                Rect gaugeRect = GUILayoutUtility.GetRect(colW - 20, 14);
                GUI.DrawTexture(gaugeRect, texPanelAccent);
                float fillW = (gaugeRect.width * (shipConfig.fuelLoadingPercentage / 250f));
                GUI.DrawTexture(new Rect(gaugeRect.x, gaugeRect.y, fillW, gaugeRect.height), (shipConfig.fuelLoadingPercentage <= 130f) ? texBarGreen : ((shipConfig.fuelLoadingPercentage <= 180f) ? texBarYellow : texBarRed));

                CompatibilityVerdict verdict = shipConfig.EvaluateCompatibility(selectedPropulsion, targetBody?.distanceFromEarthKm ?? 1000000.0, nominalFuel);
                GUI.color = verdict.fuelStatusColor;
                GUILayout.Label($"<b>{verdict.fuelVerdict}</b>", smallStyle);
                GUI.color = Color.white;

                GUILayout.Space(6);
                // 7. Diagnóstico Técnico del Astillero
                GUILayout.Label("<b>7. Diagnóstico Técnico del Astillero:</b>", headerStyle);
                GUI.color = verdict.hasCriticalRisk ? new Color(1f, 0.25f, 0.25f) : (verdict.hasWarning ? new Color(1f, 0.85f, 0.2f) : new Color(0.2f, 1f, 0.4f));
                GUILayout.Box($"<b>{verdict.title}</b>\n\n{verdict.detailedReport}", presetCardStyle, GUILayout.Height(100));
                GUI.color = Color.white;

                GUILayout.Space(10);
                // Botones de Lanzamiento
                GUI.color = verdict.hasCriticalRisk ? new Color(1f, 0.35f, 0.35f) : new Color(0.2f, 1f, 0.6f);
                string launchLabel = verdict.hasCriticalRisk 
                    ? "⛔ PROBAR NAVE CON RIESGO CRÍTICO (SIMULAR CATÁSTROFE)" 
                    : "▶ PROBAR NAVE EN VUELO (SIMULAR RESULTADO)";
                if (GUILayout.Button(launchLabel, buttonPrimaryStyle, GUILayout.Height(46)))
                {
                    StartCustomSimulation();
                }
                GUI.color = Color.white;

                GUILayout.Space(8);
                if (GUILayout.Button("⚙️ Planificar Ruta y Destinos (Pestaña 3) ➔", buttonSecondaryStyle, GUILayout.Height(34)))
                {
                    mainMenuTab = 2;
                }

                GUILayout.EndArea();
            }
            else
            {
                // PESTAÑA 2: RUTA Y DESTINOS LIBRES (PLANIFICACIÓN DE MISIÓN Y NAVEGACIÓN)
                float leftColW = 560;
                GUILayout.BeginArea(new Rect(panelX + 30, colY, leftColW, colH));
                GUILayout.Label("<b>🎯 SELECCIÓN DE DESTINO ASTRONÓMICO (15 CUERPOS):</b>", headerStyle);
                GUILayout.Label("<i>Elige el objetivo del viaje en el Sistema Solar o espacio interestelar:</i>", smallStyle);
                GUILayout.Space(6);

                destScrollPos = GUILayout.BeginScrollView(destScrollPos, GUILayout.Height(640));
                for (int i = 0; i < celestialBodies.Count; i++)
                {
                    var body = celestialBodies[i];
                    bool isSel = (i == selectedDestinationIndex);
                    string mark = isSel ? "▶ " : "  ";
                    GUI.color = isSel ? Color.cyan : Color.white;
                    if (GUILayout.Button($"{mark}{body.bodyName} ({SpaceConstants.FormatDistance(body.distanceFromEarthKm)})", buttonSecondaryStyle, GUILayout.Height(36)))
                    {
                        SetDestination(i);
                    }
                    GUI.color = Color.white;
                }
                GUILayout.EndScrollView();
                GUILayout.EndArea();

                // Columna Derecha: Perfil de Ruta, Perspectiva Temporal y Despacho
                float rightColW = panelW - leftColW - 90;
                float rightColX = panelX + leftColW + 55;
                GUILayout.BeginArea(new Rect(rightColX, colY, rightColW, colH));

                GUILayout.Label("<b>1. Perfil de Trayectoria Astrodinámica:</b>", headerStyle);
                GUILayout.Space(4);
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle(shipConfig.routeProfile == RouteProfile.DirectMaximumBurn, "⚡ Directa", buttonSecondaryStyle, GUILayout.Height(32)))
                    shipConfig.routeProfile = RouteProfile.DirectMaximumBurn;
                if (GUILayout.Toggle(shipConfig.routeProfile == RouteProfile.HohmannElliptic, "🔄 Hohmann", buttonSecondaryStyle, GUILayout.Height(32)))
                    shipConfig.routeProfile = RouteProfile.HohmannElliptic;
                if (GUILayout.Toggle(shipConfig.routeProfile == RouteProfile.SafeShieldedCorridor, "🛡️ Seguro", buttonSecondaryStyle, GUILayout.Height(32)))
                    shipConfig.routeProfile = RouteProfile.SafeShieldedCorridor;
                if (GUILayout.Toggle(shipConfig.routeProfile == RouteProfile.GravityAssistSlingshot, "🪐 Slingshot", buttonSecondaryStyle, GUILayout.Height(32)))
                    shipConfig.routeProfile = RouteProfile.GravityAssistSlingshot;
                if (GUILayout.Toggle(shipConfig.routeProfile == RouteProfile.CustomManualWaypoint, "✏️ Manual", buttonSecondaryStyle, GUILayout.Height(32)))
                    shipConfig.routeProfile = RouteProfile.CustomManualWaypoint;
                GUILayout.EndHorizontal();

                if (shipConfig.routeProfile == RouteProfile.CustomManualWaypoint)
                {
                    GUILayout.Space(6);
                    GUILayout.BeginVertical(presetCardStyle);
                    GUILayout.Label("<b>Ajuste Fino de Trayectoria Bezier (Evitar Zonas de Peligro):</b>", smallStyle);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"Curvatura Arco: <b>{shipConfig.manualArcHeight:F1} UA</b>", smallStyle, GUILayout.Width(150));
                    shipConfig.manualArcHeight = GUILayout.HorizontalSlider(shipConfig.manualArcHeight, -25f, 25f);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"Punto Inflexión: <b>{shipConfig.manualInflectionT:F2}</b>", smallStyle, GUILayout.Width(150));
                    shipConfig.manualInflectionT = GUILayout.HorizontalSlider(shipConfig.manualInflectionT, 0.05f, 0.95f);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("🛡️ Evadir Sol (+16.0)", buttonSecondaryStyle, GUILayout.Height(24)))
                    {
                        shipConfig.manualArcHeight = 16.0f;
                        shipConfig.manualInflectionT = 0.45f;
                    }
                    if (GUILayout.Button("🪐 Slingshot (-12.0)", buttonSecondaryStyle, GUILayout.Height(24)))
                    {
                        shipConfig.manualArcHeight = -12.0f;
                        shipConfig.manualInflectionT = 0.35f;
                    }
                    if (GUILayout.Button("🛡️ Desvío (+8.0)", buttonSecondaryStyle, GUILayout.Height(24)))
                    {
                        shipConfig.manualArcHeight = 8.0f;
                        shipConfig.manualInflectionT = 0.50f;
                    }
                    if (GUILayout.Button("⚡ Recta (0.0)", buttonSecondaryStyle, GUILayout.Height(24)))
                    {
                        shipConfig.manualArcHeight = 0.0f;
                        shipConfig.manualInflectionT = 0.50f;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }

                GUILayout.Space(10);
                GUILayout.Label("<b>2. Perspectiva Astrodinámica & Físicas del Vuelo:</b>", headerStyle);
                GUILayout.Space(4);

                if (targetBody != null)
                {
                    FlightCalculation calc = PropulsionPhysics.CalculateTrip(selectedPropulsion, targetBody.distanceFromEarthKm, shipConfig.GetTotalDryMassKg(), shipConfig.fuelLoadingPercentage);
                    double distKm = targetBody.distanceFromEarthKm;
                    string physicalTimeDesc = GetPhysicalFlightTimeDescription(selectedPropulsion, distKm);
                    double loadedFuelKg = calc.fuelMassKg * (shipConfig.fuelLoadingPercentage / 100.0);

                    GUILayout.Box($"<b>Destino:</b> <color=cyan>{targetBody.bodyName}</color> | <b>Distancia:</b> {SpaceConstants.FormatDistance(distKm)}\n" +
                                  $"<b>Nave Ensamblada:</b> {shipConfig.rocketType} • {shipConfig.fuelType}\n" +
                                  $"• <b>Tiempo Real Físico Estimado:</b> <color=yellow>{physicalTimeDesc}</color>\n" +
                                  $"• <b>Tiempo Medido en la Tierra:</b> {SpaceConstants.FormatTime(calc.travelTimeEarthSeconds)}\n" +
                                  $"• <b>Tiempo Propio Tripulación (tau):</b> {SpaceConstants.FormatTime(calc.travelTimeShipSeconds)}\n" +
                                  $"• <b>Velocidad Máxima:</b> {calc.maxVelocityKms:N0} km/s ({(calc.velocityFractionC >= 1.0 ? $"{calc.velocityFractionC:F1}c" : $"{(calc.velocityFractionC * 100f):F2}% c")})\n" +
                                  $"• <b>Factor Lorentz gamma:</b> {calc.lorentzGamma:F4} | <b>Fuerzas G Sentidas:</b> {calc.feltGForce:F2} g\n" +
                                  $"• <b>Combustible Requerido:</b> {calc.fuelMassKg:N0} kg | <b>Cargado:</b> {loadedFuelKg:N0} kg\n" +
                                  $"<i>Duración visual en simulación: {GetMissionBaseDurationSeconds():N0}s a escala 1x (Acelerable con ⏩).</i>",
                                  presetCardStyle, GUILayout.Height(180));
                }

                GUILayout.Space(12);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("🗺️ ABRIR MAPA TÁCTICO & TRAZAR RUTA (V)", buttonSecondaryStyle, GUILayout.Height(48)))
                {
                    currentPerspective = ViewPerspective.TacticalMap2D;
                    currentUIState = SimulationUIState.InFlight;
                    ApplyPerspective();
                }
                GUILayout.Space(10);
                GUI.color = new Color(0.2f, 1f, 0.6f);
                if (GUILayout.Button("▶ DESPEGAR AHORA MISMO (CABINA 3D)", buttonPrimaryStyle, GUILayout.Height(48)))
                {
                    currentPerspective = ViewPerspective.CockpitPOV3D;
                    StartCustomSimulation();
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();

                GUILayout.EndArea();
            }
        }

        private void DrawPresetButton(int num, string title, string description, Color accentColor)
        {
            GUILayout.BeginVertical(presetCardStyle, GUILayout.MinHeight(92));
            GUILayout.BeginHorizontal();
            GUI.color = accentColor;
            GUILayout.Label($"<b>{title}</b>", presetTitleStyle, GUILayout.Width(505), GUILayout.Height(26));
            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("VER ℹ️", buttonSecondaryStyle, GUILayout.Width(72), GUILayout.Height(28)))
            {
                ApplyPresetConfiguration(num);
            }
            GUILayout.Space(6);
            if (GUILayout.Button("EJECUTAR ▶", buttonPrimaryStyle, GUILayout.Width(115), GUILayout.Height(28)))
            {
                LoadPresetSimulation(num);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            GUILayout.Label($"<size=11>{description}</size>", smallStyle, GUILayout.Height(24));
            GUILayout.EndVertical();
            GUILayout.Space(6);
        }

        /// <summary>
        /// Interfaz activa en pantalla durante el vuelo (Top bar, barra de tiempo centrada, telemetría y controles contextuales).
        /// </summary>
        private void DrawInFlightUI()
        {
            // Barra superior fija de navegación
            GUI.Box(new Rect(0, 0, 1920, 52), "", panelBoxStyle);
            GUILayout.BeginArea(new Rect(25, 8, 1870, 40));
            GUILayout.BeginHorizontal();

            GUILayout.Label($"<b>🚀 MISIÓN:</b> {originBodyName} ➔ <color=cyan>{targetBody?.bodyName}</color> | <b>Modo:</b> {selectedPropulsion} | <b>Progreso:</b> {(flightProgress * 100f):F1}% | <b>Estado:</b> {flightStatus}", bodyStyle, GUILayout.Height(34));
            GUILayout.FlexibleSpace();

            string viewLabel = (currentPerspective == ViewPerspective.CockpitPOV3D)
                ? "🗺️ Abrir Mapa 2D (V)"
                : "💺 Entrar a Cabina 3D (V)";
            if (GUILayout.Button(viewLabel, buttonSecondaryStyle, GUILayout.Width(195), GUILayout.Height(34)))
            {
                TogglePerspective();
            }

            if (GUILayout.Button(isPaused ? "▶ Reanudar (Espacio)" : "⏸ Pausa (Espacio)", buttonSecondaryStyle, GUILayout.Width(170), GUILayout.Height(34)))
            {
                currentUIState = (currentUIState == SimulationUIState.Paused) ? SimulationUIState.InFlight : SimulationUIState.Paused;
                isPaused = (currentUIState == SimulationUIState.Paused);
            }

            if (GUILayout.Button(showHUD ? "👁 Ocultar HUD (H)" : "👁 Ver HUD (H)", buttonSecondaryStyle, GUILayout.Width(140), GUILayout.Height(34)))
            {
                showHUD = !showHUD;
            }

            GUI.color = new Color(1f, 0.8f, 0.4f);
            if (GUILayout.Button("🏠 Menú Principal", buttonSecondaryStyle, GUILayout.Width(150), GUILayout.Height(34)))
            {
                currentUIState = SimulationUIState.MainMenu;
                isPaused = true;
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (!showHUD) return;

            // 1. Barra Central de Control Temporal y Scrubbing Interactivo (1x a 100x, Pausa y Rebobinado)
            DrawTimeControlsBar();

            // 2. Banner Dinámico de Estado y Alertas de Supervivencia (Verde, Ámbar, Rojo, Fallo)
            DrawAlertBanner();

            // 3. Renderizado según perspectiva activa
            if (currentPerspective == ViewPerspective.CockpitPOV3D)
            {
                DrawCockpitPOVUI();
            }
            else
            {
                DrawTacticalMap2DUI();
            }

            // 4. Cuenta regresiva y anuncio de despegue cinemático
            if (launchCountdownTimer > 0f || launchBannerFadeTimer > 0f)
            {
                DrawLaunchCountdownOverlay();
            }
        }

        /// <summary>
        /// Barra superior centrada para manipular la escala de tiempo, pausar, rebobinar o fregar la trayectoria.
        /// </summary>
        private void DrawTimeControlsBar()
        {
            GUI.Box(new Rect(360, 58, 1200, 48), "", telemetryCardStyle);
            GUILayout.BeginArea(new Rect(375, 63, 1170, 38));
            GUILayout.BeginHorizontal();

            // Botón Rebobinar
            GUI.color = isRewinding ? new Color(1f, 0.5f, 0.1f) : Color.white;
            string rewindText = isRewinding ? "⏪ REBOBINANDO..." : "⏪ Rebobinar";
            if (GUILayout.Button(rewindText, buttonSecondaryStyle, GUILayout.Width(140), GUILayout.Height(30)))
            {
                isRewinding = !isRewinding;
                if (isRewinding) isFlying = true;
            }
            GUI.color = Color.white;

            // Botón Play / Pausa
            if (GUILayout.Button(isPaused ? "▶" : "⏸", buttonSecondaryStyle, GUILayout.Width(45), GUILayout.Height(30)))
            {
                isPaused = !isPaused;
                currentUIState = isPaused ? SimulationUIState.Paused : SimulationUIState.InFlight;
            }

            // Botones de aceleración temporal
            float[] speeds = new float[] { 1f, 2f, 5f, 10f, 25f, 50f, 100f };
            for (int s = 0; s < speeds.Length; s++)
            {
                float sp = speeds[s];
                bool isCurSpeed = Mathf.Approximately(timeWarpMultiplier, sp);
                GUI.color = isCurSpeed ? Color.cyan : Color.white;
                if (GUILayout.Button($"{sp:F0}x", buttonSecondaryStyle, GUILayout.Width(42), GUILayout.Height(30)))
                {
                    timeWarpMultiplier = sp;
                }
            }
            GUI.color = Color.white;

            GUILayout.Space(10);
            GUILayout.Label("<b>Línea de Tiempo:</b>", smallStyle, GUILayout.Width(105));

            // Slider interactivo de scrubbing
            float curProg = flightProgress;
            float scrubProg = GUILayout.HorizontalSlider(curProg, 0f, 1f, GUILayout.Width(220));
            if (Mathf.Abs(scrubProg - curProg) > 0.002f)
            {
                flightProgress = scrubProg;
            }

            GUILayout.Space(6);
            float baseDuration = GetMissionBaseDurationSeconds();
            float curSec = flightProgress * baseDuration;
            GUILayout.Label($"<b>{curSec:F1}s / {baseDuration:F0}s</b> ({(flightProgress * 100f):F0}%)", smallStyle, GUILayout.Width(130));

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Banner horizontal dinámico de alertas de supervivencia y salud del vehículo en tiempo real.
        /// </summary>
        private void DrawAlertBanner()
        {
            Texture2D bannerTex = texAlertGreen;
            Color bannerTextColor = Color.white;
            string alertMsg = "";

            if (survivalSystem.isMissionFailed || survivalSystem.currentAlert == AlertLevel.Failed)
            {
                bannerTex = texAlertRed;
                alertMsg = $"💥 FALLO CATASTRÓFICO: {survivalSystem.failureReason}";
            }
            else if (survivalSystem.currentAlert == AlertLevel.Critical)
            {
                bannerTex = texAlertRed;
                bool blink = (Mathf.Sin(Time.time * 8f) > 0f);
                bannerTextColor = blink ? Color.yellow : Color.white;
                alertMsg = $"🚨 ALERTA CRÍTICA: {survivalSystem.activeAlertMessage} — ¡RIESGO DE DESTRUCCIÓN O PÉRDIDA!";
            }
            else if (survivalSystem.currentAlert == AlertLevel.Warning)
            {
                bannerTex = texAlertAmber;
                alertMsg = $"⚠️ ADVERTENCIA: {survivalSystem.activeAlertMessage} — Operando cerca de límites estructurales.";
            }
            else
            {
                bannerTex = texAlertGreen;
                alertMsg = $"🟢 SISTEMAS NOMINALES | Casco: {survivalSystem.hullIntegrityPct:F0}% | Dosis: {survivalSystem.accumulatedRadiation_mSv:F2} mSv | Blindaje e integridad en márgenes óptimos.";
            }

            GUI.DrawTexture(new Rect(360, 110, 1200, 36), bannerTex);
            GUI.color = bannerTextColor;
            GUI.Label(new Rect(375, 115, 1170, 26), $"<b>{alertMsg}</b>", bodyStyle);
            GUI.color = Color.white;
        }

        /// <summary>
        /// Renderizado de telemetría y salud del vehículo en el modo Cabina 3D POV.
        /// </summary>
        private void DrawCockpitPOVUI()
        {
            // Tarjeta Superior: Telemetría de Vuelo y Astrodinámica
            Rect cardRect = new Rect(1410, 60, 485, 420);
            GUI.Box(cardRect, "", telemetryCardStyle);
            GUILayout.BeginArea(new Rect(1425, 70, 455, 400));

            GUILayout.Label($"<b>📊 TELEMETRÍA EN CABINA POV</b>", headerStyle);
            GUILayout.Space(6);
            GUILayout.Label($"• <b>Destino:</b> <color=cyan>{targetBody?.bodyName}</color> ({SpaceConstants.FormatDistance(currentRemainingDistanceKm)} rest.)", bodyStyle);
            GUILayout.Label($"• <b>Fuerza G Sentida:</b> {currentGForce:F2} g ({((currentGForce == 0) ? "Inercial Cero-G Puro" : "Aceleración")})", bodyStyle);
            GUILayout.Label($"• <b>Velocidad:</b> {currentVelocityKms:N0} km/s", bodyStyle);

            if (targetBody != null)
            {
                FlightCalculation calc = PropulsionPhysics.CalculateTrip(selectedPropulsion, targetBody.distanceFromEarthKm);
                GUILayout.Label($"• <b>Velocidad %c:</b> {(calc.velocityFractionC >= 1.0 ? $"{calc.velocityFractionC:F1}c" : $"{(calc.velocityFractionC * 100f):F2}% c")}", bodyStyle);
                GUILayout.Label($"• <b>Tiempo Tierra:</b> {SpaceConstants.FormatTime(flightElapsedTimeEarth)}", bodyStyle);
                GUILayout.Label($"• <b>Tiempo Tripulación (tau):</b> {SpaceConstants.FormatTime(flightElapsedTimeShip)}", bodyStyle);

                GUILayout.Space(6);
                if (selectedPropulsion == PropulsionMode.WarpDrive)
                    GUILayout.Label("<b>Reserva del Reactor Cuántico Casimir (ρ < 0):</b>", bodyStyle);
                else
                    GUILayout.Label("<b>Consumo de Masa de Reacción (Propelente):</b>", bodyStyle);

                float fuelFraction = (calc.fuelMassKg > 0.0) ? Mathf.Clamp01((float)(currentRemainingFuelKg / calc.fuelMassKg)) : 1.0f;

                Rect barBg = GUILayoutUtility.GetRect(440, 18);
                GUI.DrawTexture(barBg, texPanelAccent);
                Rect barFill = new Rect(barBg.x, barBg.y, barBg.width * fuelFraction, barBg.height);
                GUI.DrawTexture(barFill, (fuelFraction < 0.15f) ? texBarRed : (selectedPropulsion == PropulsionMode.WarpDrive ? texBarCyan : texBarGreen));

                if (selectedPropulsion == PropulsionMode.WarpDrive)
                    GUILayout.Label($"Energía Exótica Disponible: {calc.exoticEnergyJoules:E2} J [Sin masa de reacción]", smallStyle);
                else
                    GUILayout.Label($"Combustible Restante: {currentRemainingFuelKg:N0} kg ({(fuelFraction * 100f):F1}%)", smallStyle);
            }

            GUILayout.EndArea();

            // Tarjeta Inferior: Supervivencia, Salud y Ensamblaje del Vehículo
            Rect survCardRect = new Rect(1410, 492, 485, 516);
            GUI.Box(survCardRect, "", telemetryCardStyle);
            GUILayout.BeginArea(new Rect(1425, 502, 455, 496));

            GUILayout.Label("<b>🛡️ SUPERVIVENCIA & ENSAMBLAJE</b>", headerStyle);
            GUILayout.Space(4);

            MaterialProperties matProps = ShipConfiguration.GetProperties(shipConfig.materialTier);
            GUILayout.Label($"• <b>Chasis:</b> <color=cyan>{matProps.name}</color>", bodyStyle);
            GUILayout.Label($"• <b>Blindaje:</b> {shipConfig.GetShieldThicknessCm()} cm (+{shipConfig.CalculateShieldMassKg():N0} kg)", bodyStyle);
            GUILayout.Label($"• <b>Perfil:</b> {((shipConfig.routeProfile == RouteProfile.SafeShieldedCorridor) ? "🛡️ Corredor Seguro" : "⚡ Ruta Directa")}", bodyStyle);

            GUILayout.Space(6);
            // 1. Integridad del Casco
            GUILayout.Label($"<b>Integridad Estructural del Casco:</b> {survivalSystem.hullIntegrityPct:F1}%", bodyStyle);
            Rect hullBg = GUILayoutUtility.GetRect(440, 16);
            GUI.DrawTexture(hullBg, texPanelAccent);
            float hullW = hullBg.width * Mathf.Clamp01(survivalSystem.hullIntegrityPct / 100f);
            Texture2D hullBarColor = (survivalSystem.hullIntegrityPct > 70f) ? texBarGreen : (survivalSystem.hullIntegrityPct > 35f ? texBarYellow : texBarRed);
            GUI.DrawTexture(new Rect(hullBg.x, hullBg.y, hullW, hullBg.height), hullBarColor);

            GUILayout.Space(6);
            // 2. Dosis de Radiación Acumulada
            GUILayout.Label($"<b>Dosis de Radiación:</b> {survivalSystem.accumulatedRadiation_mSv:F2} mSv / 3.500 mSv", bodyStyle);
            Rect radBg = GUILayoutUtility.GetRect(440, 16);
            GUI.DrawTexture(radBg, texPanelAccent);
            float radFraction = Mathf.Clamp01((float)(survivalSystem.accumulatedRadiation_mSv / 3500.0));
            Texture2D radBarColor = (radFraction < 0.28f) ? texBarGreen : (radFraction < 0.70f ? texBarYellow : texBarRed);
            GUI.DrawTexture(new Rect(radBg.x, radBg.y, radBg.width * radFraction, radBg.height), radBarColor);
            GUILayout.Label("<size=11><i>< 250 mSv: Seguro | > 1.000 mSv: Daño Celular | 3.500 mSv: Letal</i></size>", smallStyle);

            GUILayout.Space(6);
            if (selectedPropulsion == PropulsionMode.WarpDrive)
            {
                // Integridad Geométrica Burbuja Casimir
                GUILayout.Label($"<b>Integridad Geométrica de la Burbuja:</b> {survivalSystem.warpBubbleStabilityPct:F1}%", bodyStyle);
                Rect warpBg = GUILayoutUtility.GetRect(440, 16);
                GUI.DrawTexture(warpBg, texPanelAccent);
                float warpW = warpBg.width * Mathf.Clamp01(survivalSystem.warpBubbleStabilityPct / 100f);
                GUI.DrawTexture(new Rect(warpBg.x, warpBg.y, warpW, warpBg.height), (survivalSystem.warpBubbleStabilityPct > 50f) ? texBarCyan : texBarRed);
                GUILayout.Label("<size=11><i>Geometría métrica f(rs) | Sin deformación por marea ni colisión ISM</i></size>", smallStyle);
            }
            else
            {
                // Estrés Térmico del Reactor
                GUILayout.Label($"<b>Estrés Térmico de Propulsión:</b> {survivalSystem.engineThermalStressPct:F1}%", bodyStyle);
                Rect thermBg = GUILayoutUtility.GetRect(440, 16);
                GUI.DrawTexture(thermBg, texPanelAccent);
                float thermW = thermBg.width * Mathf.Clamp01(survivalSystem.engineThermalStressPct / 100f);
                Texture2D thermBarColor = (survivalSystem.engineThermalStressPct < 50f) ? texBarGreen : (survivalSystem.engineThermalStressPct < 75f ? texBarYellow : texBarRed);
                GUI.DrawTexture(new Rect(thermBg.x, thermBg.y, thermW, thermBg.height), thermBarColor);
            }

            GUILayout.Space(6);
            string fuelNote = (shipConfig.fuelLoadingPercentage > 180f) ? "<color=red>Sobrepresión Peligrosa</color>" : 
                              ((shipConfig.fuelLoadingPercentage > 130f) ? "<color=yellow>Sobrecarga</color>" : 
                              ((shipConfig.fuelLoadingPercentage < 95f) ? "<color=orange>Déficit de Retorno</color>" : "<color=green>Nominal (+15% Margen)</color>"));
            string fuelParamName = (selectedPropulsion == PropulsionMode.WarpDrive) ? "Banco Cuántico Cargado" : "Carga Combustible";
            GUILayout.Label($"• <b>{fuelParamName}:</b> {shipConfig.fuelLoadingPercentage:F0}% ({fuelNote})", bodyStyle);

            GUILayout.EndArea();

            // Barra inferior interactiva de Controles de Cabina 3D POV
            GUI.Box(new Rect(260, 1018, 1400, 52), "", telemetryCardStyle);
            GUILayout.BeginArea(new Rect(275, 1024, 1370, 40));
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("🔭 Mirar al Frente", buttonSecondaryStyle, GUILayout.Width(170), GUILayout.Height(32)))
            {
                isInspectingDashboard = false;
                if (cockpitManager != null) cockpitManager.LookForward();
            }

            GUI.color = isInspectingDashboard ? Color.cyan : Color.white;
            if (GUILayout.Button("🎛️ Diales y Tanques", buttonSecondaryStyle, GUILayout.Width(180), GUILayout.Height(32)))
            {
                isInspectingDashboard = !isInspectingDashboard;
                if (cockpitManager != null)
                {
                    if (isInspectingDashboard) cockpitManager.LookAtDashboard();
                    else cockpitManager.LookForward();
                }
            }
            GUI.color = Color.white;

            if (GUILayout.Button("💨 Purgar Válvulas Criogénicas", buttonSecondaryStyle, GUILayout.Width(235), GUILayout.Height(32)))
            {
                survivalSystem.PurgeCryoValves();
            }

            string ismText = survivalSystem.isISMDeflectorActive ? "🛡️ Escudo ISM: [ACTIVO]" : "🛡️ Escudo ISM: [OFF]";
            GUI.color = survivalSystem.isISMDeflectorActive ? Color.cyan : Color.white;
            if (GUILayout.Button(ismText, buttonSecondaryStyle, GUILayout.Width(200), GUILayout.Height(32)))
            {
                survivalSystem.ToggleISMDeflector();
            }
            GUI.color = Color.white;

            string toggleText = "🗺️ Mapa Táctico 2D (V)";
            if (GUILayout.Button(toggleText, buttonPrimaryStyle, GUILayout.Width(200), GUILayout.Height(32)))
            {
                TogglePerspective();
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("🖱️ <i>Arrastrar Clic Der: Mirar | D: Diales | Espacio: Pausa</i>", smallStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            // 5. Consola Holográfica Interactiva de Diales y Tanques al inspeccionar tablero
            if (isInspectingDashboard)
            {
                DrawDashboardInspectionHUD();
            }
        }

        /// <summary>
        /// Renderizado contextual en el modo Mapa Táctico 2D.
        /// </summary>
        private void DrawTacticalMap2DUI()
        {
            // MODO MAPA TÁCTICO 2D: Etiquetas interactivas sobre planetas y zonas de peligro
            DrawInteractiveBodyLabels();

            // Badge central superior de Perfil de Ruta
            GUI.Box(new Rect(420, 152, 1080, 36), "", telemetryCardStyle);
            string profileDesc = shipConfig.routeProfile switch
            {
                RouteProfile.CustomManualWaypoint => "<b>✏️ PERFIL ACTIVO: Waypoints Manuales Bezier (Control Interactivo de Esquiva)</b>",
                RouteProfile.SafeShieldedCorridor => "<b>🛡️ PERFIL ACTIVO: Corredor Seguro (Trayectoria Curva Anti-Radiación)</b>",
                RouteProfile.HohmannElliptic => "<b>🔄 PERFIL ACTIVO: Transferencia Hohmann (Mínima Energía Inercial)</b>",
                RouteProfile.GravityAssistSlingshot => "<b>🪐 PERFIL ACTIVO: Asistencia Gravitatoria (Impulso Planetario)</b>",
                _ => "<b>⚡ PERFIL ACTIVO: Ruta Directa (Trayectoria Rectilínea de Máximo Empuje)</b>"
            };
            GUI.Label(new Rect(435, 158, 1050, 24), profileDesc, bodyStyle);

            // Panel de Control Interactivo de Navegación Manual Bezier
            if (shipConfig.routeProfile == RouteProfile.CustomManualWaypoint)
            {
                Rect manualHudRect = new Rect(420, 194, 1080, 85);
                GUI.Box(manualHudRect, "", panelBoxStyle);
                GUILayout.BeginArea(new Rect(435, 198, 1050, 78));
                GUILayout.BeginHorizontal();
                GUILayout.Label("<b>📍 TRAZADO BEZIER (ESQUIVAR RADIACIÓN / POLVO ISM):</b>", headerStyle, GUILayout.Width(440));
                if (GUILayout.Button("🛡️ Evadir Sol (+16.0)", buttonSecondaryStyle, GUILayout.Width(170), GUILayout.Height(24)))
                {
                    shipConfig.manualArcHeight = 16.0f;
                    shipConfig.manualInflectionT = 0.45f;
                }
                if (GUILayout.Button("🪐 Slingshot (-12.0)", buttonSecondaryStyle, GUILayout.Width(160), GUILayout.Height(24)))
                {
                    shipConfig.manualArcHeight = -12.0f;
                    shipConfig.manualInflectionT = 0.35f;
                }
                if (GUILayout.Button("🛡️ Desvío (+8.0)", buttonSecondaryStyle, GUILayout.Width(140), GUILayout.Height(24)))
                {
                    shipConfig.manualArcHeight = 8.0f;
                    shipConfig.manualInflectionT = 0.50f;
                }
                if (GUILayout.Button("⚡ Recta (0.0)", buttonSecondaryStyle, GUILayout.Width(120), GUILayout.Height(24)))
                {
                    shipConfig.manualArcHeight = 0.0f;
                    shipConfig.manualInflectionT = 0.50f;
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Curvatura Arco: <b>{shipConfig.manualArcHeight:F1} UA</b>", smallStyle, GUILayout.Width(140));
                shipConfig.manualArcHeight = GUILayout.HorizontalSlider(shipConfig.manualArcHeight, -25f, 25f, GUILayout.Width(350));
                GUILayout.Space(25);
                GUILayout.Label($"Punto Inflexión: <b>{shipConfig.manualInflectionT:F2}</b>", smallStyle, GUILayout.Width(140));
                shipConfig.manualInflectionT = GUILayout.HorizontalSlider(shipConfig.manualInflectionT, 0.05f, 0.95f, GUILayout.Width(330));
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            // Panel Izquierdo: Selección y Despacho de Vuelo
            Rect leftRect = new Rect(25, 155, 380, 830);
            GUI.Box(leftRect, "", panelBoxStyle);
            GUILayout.BeginArea(new Rect(40, 168, 350, 800));

            GUILayout.Label("<b>🎯 SELECCIÓN DE DESTINO:</b>", headerStyle);
            destScrollPos = GUILayout.BeginScrollView(destScrollPos, GUILayout.Height(240));
            for (int i = 0; i < celestialBodies.Count; i++)
            {
                var body = celestialBodies[i];
                bool isSelected = (i == selectedDestinationIndex);
                string mark = isSelected ? "▶ " : "  ";
                GUI.color = isSelected ? Color.cyan : Color.white;
                if (GUILayout.Button($"{mark}{body.bodyName}", GUILayout.Height(24)))
                {
                    SetDestination(i);
                }
                GUI.color = Color.white;
            }
            GUILayout.EndScrollView();

            GUILayout.Space(10);
            GUILayout.Label("<b>⚡ Modo de Propulsión:</b>", headerStyle);
            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(selectedPropulsion == PropulsionMode.Chemical, "Química", buttonSecondaryStyle, GUILayout.Height(32)))
            {
                if (selectedPropulsion != PropulsionMode.Chemical) { selectedPropulsion = PropulsionMode.Chemical; if (cockpitManager != null) cockpitManager.SetCockpitMode(selectedPropulsion); }
            }
            if (GUILayout.Toggle(selectedPropulsion == PropulsionMode.Antimatter, "Antimateria", buttonSecondaryStyle, GUILayout.Height(32)))
            {
                if (selectedPropulsion != PropulsionMode.Antimatter) { selectedPropulsion = PropulsionMode.Antimatter; if (cockpitManager != null) cockpitManager.SetCockpitMode(selectedPropulsion); }
            }
            if (GUILayout.Toggle(selectedPropulsion == PropulsionMode.WarpDrive, "Warp", buttonSecondaryStyle, GUILayout.Height(32)))
            {
                if (selectedPropulsion != PropulsionMode.WarpDrive) { selectedPropulsion = PropulsionMode.WarpDrive; if (cockpitManager != null) cockpitManager.SetCockpitMode(selectedPropulsion); }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(14);
            GUI.color = isFlying ? Color.yellow : Color.green;
            if (GUILayout.Button(isFlying ? "🚀 EN VUELO..." : "▶ INICIAR VUELO", buttonPrimaryStyle, GUILayout.Height(44)))
            {
                if (!isFlying) StartFlight();
            }
            GUI.color = Color.white;

            GUILayout.Space(8);
            if (GUILayout.Button("↺ Regresar a Órbita Terrestre (R)", buttonSecondaryStyle, GUILayout.Height(32)))
            {
                ResetToEarth();
            }

            GUILayout.Space(14);
            GUILayout.Label("<b>🌐 Cuadrícula y Anillos UA:</b>", bodyStyle);
            if (astroGrid != null)
            {
                float newOp = GUILayout.HorizontalSlider(astroGrid.gridOpacity, 0.02f, 0.45f);
                if (Mathf.Abs(newOp - astroGrid.gridOpacity) > 0.005f) astroGrid.SetOpacity(newOp);
                GUILayout.Label($"Opacidad: {(astroGrid.gridOpacity * 100f):F0}%", smallStyle);
            }

            GUILayout.EndArea();

            // Panel Derecho: Telemetría Científica y Relativista
            Rect rightRect = new Rect(1515, 155, 380, 830);
            GUI.Box(rightRect, "", panelBoxStyle);
            GUILayout.BeginArea(new Rect(1530, 168, 350, 800));

            GUILayout.Label("<b>📊 TELEMETRÍA Y FÍSICA RELATIVISTA</b>", headerStyle);
            GUILayout.Space(8);

            if (targetBody != null)
            {
                FlightCalculation calc = PropulsionPhysics.CalculateTrip(selectedPropulsion, targetBody.distanceFromEarthKm);

                GUILayout.Label($"<b>Destino:</b> <color=cyan>{targetBody.bodyName}</color>", bodyStyle);
                GUILayout.Label($"<b>Distancia:</b> {SpaceConstants.FormatDistance(targetBody.distanceFromEarthKm)}", bodyStyle);
                GUILayout.Label($"<b>Principio:</b> {calc.modeName}", bodyStyle);
                GUILayout.Space(6);

                GUILayout.Box($"<b>ESTADO DEL VUELO:</b> {flightStatus}\n" +
                              $"<b>Progreso:</b> {(flightProgress * 100f):F1}%\n" +
                              $"<b>Recorrido:</b> {SpaceConstants.FormatDistance(currentDistanceCoveredKm)}\n" +
                              $"<b>Restante:</b> {SpaceConstants.FormatDistance(currentRemainingDistanceKm)}", presetCardStyle, GUILayout.Height(90));

                GUILayout.Space(8);
                GUILayout.Label($"• <b>Tiempo Tierra:</b> {SpaceConstants.FormatTime(flightElapsedTimeEarth)} / {SpaceConstants.FormatTime(calc.travelTimeEarthSeconds)}", bodyStyle);
                GUILayout.Label($"• <b>Tiempo Tripulación (tau):</b> {SpaceConstants.FormatTime(flightElapsedTimeShip)} / {SpaceConstants.FormatTime(calc.travelTimeShipSeconds)}", bodyStyle);
                GUILayout.Label($"• <b>Velocidad Máxima:</b> {calc.maxVelocityKms:N0} km/s ({(calc.velocityFractionC >= 1.0 ? $"{calc.velocityFractionC:F1}c" : $"{(calc.velocityFractionC * 100f):F2}% c")})", bodyStyle);
                GUILayout.Label($"• <b>Factor Lorentz gamma:</b> {calc.lorentzGamma:F4}", bodyStyle);
                GUILayout.Label($"• <b>Fuerzas G Sentidas:</b> {calc.feltGForce:F2} g", bodyStyle);

                if (selectedPropulsion == PropulsionMode.WarpDrive)
                {
                    GUILayout.Label($"• <b>Energía Exótica:</b> {calc.exoticEnergyJoules:E2} J", bodyStyle);
                    GUILayout.Label($"• <b>Masa de Reacción:</b> 0.0 kg (Espacio en compresión)", bodyStyle);
                }
                else if (selectedPropulsion == PropulsionMode.Antimatter)
                {
                    GUILayout.Label($"• <b>Antimateria:</b> {calc.antimatterMassKg:N1} kg", bodyStyle);
                    GUILayout.Label($"• <b>Combustible Restante:</b> {currentRemainingFuelKg:N1} kg", bodyStyle);
                }
                else
                {
                    GUILayout.Label($"• <b>Combustible Requerido:</b> {calc.fuelMassKg:N1} kg", bodyStyle);
                    GUILayout.Label($"• <b>Combustible Restante:</b> {currentRemainingFuelKg:N1} kg", bodyStyle);
                }

                GUILayout.Space(8);
                GUILayout.Label($"<b>Efecto Temporal:</b>\n<size=12>{calc.timeDilationSummary}</size>", smallStyle);
            }

            GUILayout.EndArea();

            // Panel Inferior: Controles de Cámara 2D
            Rect botRect = new Rect(25, 1000, 1870, 65);
            GUI.Box(botRect, "", panelBoxStyle);
            GUILayout.BeginArea(new Rect(40, 1012, 1840, 45));
            GUILayout.BeginHorizontal();

            GUILayout.Label("<b>🔭 Controles de Cámara 2D:</b>", headerStyle, GUILayout.Width(240));

            if (GUILayout.Button("Cámara Libre", buttonSecondaryStyle, GUILayout.Width(110), GUILayout.Height(30)))
            {
                if (spaceCamera != null) spaceCamera.SetMode(CameraFollowMode.Free);
            }
            if (GUILayout.Button("Fijar Nave", buttonSecondaryStyle, GUILayout.Width(100), GUILayout.Height(30)))
            {
                if (spaceCamera != null && spaceshipObj != null) spaceCamera.SetMode(CameraFollowMode.FollowShip, spaceshipObj.transform, 6.0f);
            }
            if (GUILayout.Button("Fijar Tierra", buttonSecondaryStyle, GUILayout.Width(100), GUILayout.Height(30)))
            {
                if (spaceCamera != null && earthBody != null) spaceCamera.SetMode(CameraFollowMode.FocusEarth, earthBody.transform, 6.0f);
            }
            if (GUILayout.Button("Fijar Destino", buttonSecondaryStyle, GUILayout.Width(110), GUILayout.Height(30)))
            {
                if (spaceCamera != null && targetBody != null) spaceCamera.SetMode(CameraFollowMode.FocusTarget, targetBody.transform, 8.0f);
            }

            GUILayout.Space(12);
            if (GUILayout.Button("🔍 +", buttonSecondaryStyle, GUILayout.Width(50), GUILayout.Height(30)))
            {
                if (spaceCamera != null) spaceCamera.ZoomIn();
            }
            if (GUILayout.Button("🔍 -", buttonSecondaryStyle, GUILayout.Width(50), GUILayout.Height(30)))
            {
                if (spaceCamera != null) spaceCamera.ZoomOut();
            }

            if (GUILayout.Button("Vista Solar", buttonSecondaryStyle, GUILayout.Width(110), GUILayout.Height(30)))
            {
                if (spaceCamera != null) spaceCamera.CenterOnPosition(new Vector3(0f, 0f, -10f), 32.0f);
            }
            if (GUILayout.Button("Oort / Interestelar", buttonSecondaryStyle, GUILayout.Width(150), GUILayout.Height(30)))
            {
                if (spaceCamera != null) spaceCamera.CenterOnPosition(new Vector3(140f, 0f, -10f), 85.0f);
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("🖱️ <b>Rueda:</b> Zoom | <b>Arrastrar Clic:</b> Paneo | <b>WASD:</b> Mover espacio | <b>V:</b> Volver a Cabina", bodyStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Menú de pausa modal centrado.
        /// </summary>
        private void DrawPauseMenu()
        {
            GUI.DrawTexture(new Rect(0, 0, 1920, 1080), texVignette);

            float modalW = 720f;
            float modalH = 500f;
            float modalX = (1920f - modalW) * 0.5f;
            float modalY = (1080f - modalH) * 0.5f;

            GUI.Box(new Rect(modalX, modalY, modalW, modalH), "", panelBoxStyle);

            GUILayout.BeginArea(new Rect(modalX + 35, modalY + 28, modalW - 70, modalH - 56));
            GUILayout.Label("⏸ SIMULACIÓN EN PAUSA", titleStyle);
            GUILayout.Space(6);
            GUILayout.Label($"Trayectoria actual: {originBodyName} ➔ <color=cyan>{targetBody?.bodyName}</color>", subtitleStyle);
            GUILayout.Space(14);

            GUILayout.Box($"<b>Progreso del Vuelo:</b> {(flightProgress * 100f):F1}%\n" +
                          $"<b>Distancia Recorrida:</b> {SpaceConstants.FormatDistance(currentDistanceCoveredKm)}\n" +
                          $"<b>Distancia Restante:</b> {SpaceConstants.FormatDistance(currentRemainingDistanceKm)}\n" +
                          $"<b>Tiempo Transcurrido (Tierra):</b> {SpaceConstants.FormatTime(flightElapsedTimeEarth)}\n" +
                          $"<b>Modo Activo:</b> {selectedPropulsion}", presetCardStyle, GUILayout.Height(105));

            GUILayout.Space(16);

            GUI.color = new Color(0.2f, 1f, 0.6f);
            if (GUILayout.Button("▶ REANUDAR VUELO (Espacio)", buttonPrimaryStyle, GUILayout.Height(46)))
            {
                currentUIState = SimulationUIState.InFlight;
                isPaused = false;
            }
            GUI.color = Color.white;
            GUILayout.Space(8);

            if (GUILayout.Button("↺ Reiniciar Vuelo a la Tierra (R)", buttonSecondaryStyle, GUILayout.Height(38)))
            {
                ResetToEarth();
                currentUIState = SimulationUIState.InFlight;
                isPaused = false;
            }
            GUILayout.Space(8);

            string toggleViewText = (currentPerspective == ViewPerspective.CockpitPOV3D)
                ? "🗺️ Cambiar a Mapa Táctico 2D (V)"
                : "💺 Entrar a Cabina 3D POV (V)";
            if (GUILayout.Button(toggleViewText, buttonSecondaryStyle, GUILayout.Height(38)))
            {
                TogglePerspective();
            }
            GUILayout.Space(8);

            GUI.color = new Color(1f, 0.75f, 0.4f);
            if (GUILayout.Button("🏠 Regresar al Menú Principal de Simulaciones", buttonSecondaryStyle, GUILayout.Height(38)))
            {
                currentUIState = SimulationUIState.MainMenu;
                isPaused = true;
            }
            GUI.color = Color.white;

            GUILayout.EndArea();
        }

        /// <summary>
        /// Animación y secuencia de alarma estroboscópica de 4 segundos previa a la autopsia técnica.
        /// </summary>
        private void DrawCatastropheAlarmSequence()
        {
            // Parpadeo estroboscópico de emergencia roja a pantalla completa
            float strobe = Mathf.Abs(Mathf.Sin(Time.time * 8.0f));
            GUI.color = new Color(1f, 0.05f, 0.05f, 0.35f * strobe);
            GUI.DrawTexture(new Rect(0, 0, 1920, 1080), texVignette);
            GUI.color = Color.white;

            // Alerta central palpitante de evacuación / destrucción del vehículo
            float boxW = 880f;
            float boxH = 210f;
            float boxX = (1920f - boxW) * 0.5f;
            float boxY = (1080f - boxH) * 0.5f - 60f;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "", panelBoxStyle);
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, 8), texAlertRed);

            GUILayout.BeginArea(new Rect(boxX + 30, boxY + 20, boxW - 60, boxH - 35));
            GUI.color = new Color(1f, 0.25f, 0.25f);
            GUILayout.Label("🚨 ¡ALARMA DE CATÁSTROFE / FALLO CRÍTICO EN PROCESO! 🚨", titleStyle);
            GUI.color = Color.white;
            GUILayout.Space(8);
            GUILayout.Label($"<b>Causa Inminente:</b> <color=red>{survivalSystem.catastropheType}</color>", headerStyle);
            GUILayout.Label($"<b>Colapso Estructural y Destrucción en:</b> <color=yellow>{survivalSystem.catastropheTimer:F1} segundos</color>", bodyStyle);
            GUILayout.Label("<i>¡Sacudida inercial violenta registrada en cabina! Registradores de vuelo grabando telemetría final...</i>", smallStyle);
            GUILayout.EndArea();
        }

        /// <summary>
        /// Modal de autopsia técnica y opciones de recuperación ante fallo catastrófico del vehículo.
        /// </summary>
        private void DrawMissionFailureModal()
        {
            GUI.DrawTexture(new Rect(0, 0, 1920, 1080), texVignette);

            float modalW = 820f;
            float modalH = 580f;
            float modalX = (1920f - modalW) * 0.5f;
            float modalY = (1080f - modalH) * 0.5f;

            GUI.Box(new Rect(modalX, modalY, modalW, modalH), "", panelBoxStyle);

            // Borde superior rojo de advertencia
            GUI.DrawTexture(new Rect(modalX, modalY, modalW, 6), texAlertRed);

            GUILayout.BeginArea(new Rect(modalX + 35, modalY + 22, modalW - 70, modalH - 44));

            GUI.color = new Color(1f, 0.25f, 0.25f);
            GUILayout.Label("💥 MISIÓN FALLIDA — CATASTROFE VEHICULAR", titleStyle);
            GUI.color = Color.white;
            GUILayout.Space(2);
            GUILayout.Label("La integridad de la nave o la vida de la tripulación han sido comprometidas.", subtitleStyle);
            GUILayout.Space(12);

            // Diagnóstico de Causa
            GUI.color = new Color(1f, 0.35f, 0.35f);
            GUILayout.Box($"<b>DICTAMEN DE FALLO:</b>\n<size=15>{survivalSystem.failureReason}</size>", presetCardStyle, GUILayout.Height(75));
            GUI.color = Color.white;

            GUILayout.Space(10);
            GUILayout.Label("<b>📊 Telemetría en el Momento del Colapso:</b>", headerStyle);
            MaterialProperties curProps = ShipConfiguration.GetProperties(shipConfig.materialTier);
            GUILayout.Box($"• <b>Misión:</b> {originBodyName} ➔ <color=cyan>{targetBody?.bodyName}</color> (Progreso alcanzado: {(flightProgress * 100f):F1}%)\n" +
                          $"• <b>Estructura / Chasis:</b> {curProps.name} | <b>Blindaje:</b> {shipConfig.GetShieldThicknessCm()} cm\n" +
                          $"• <b>Integridad del Casco:</b> <color=red>{survivalSystem.hullIntegrityPct:F1}%</color> | <b>Fuerza G:</b> {currentGForce:F2} g\n" +
                          $"• <b>Dosis de Radiación:</b> <color=orange>{survivalSystem.accumulatedRadiation_mSv:F2} mSv</color> (Límite biológico letal: 3.500 mSv)\n" +
                          $"• <b>Carga de Combustible Inicial:</b> {shipConfig.fuelLoadingPercentage:F0}% ({((shipConfig.fuelLoadingPercentage > 150f) ? "Sobrecarga Crítica" : "Estándar")})",
                          presetCardStyle, GUILayout.Height(130));

            GUILayout.Space(10);
            GUILayout.Label("<b>💡 Recomendación Técnica del Astillero:</b>", smallStyle);
            string tip = "Ajuste los materiales o la carga de propelente en el Taller de Ensamblaje.";
            if (shipConfig.fuelLoadingPercentage > 180f)
                tip = "Riesgo de sobrepresión criogénica o fuga magnética por exceso de propelente. Reduzca la carga a 100%-120%.";
            else if (survivalSystem.accumulatedRadiation_mSv >= 3500.0)
                tip = "Dosis letal de radiación cósmica. Equipe blindaje pesado (5-10 cm) o active el 'Corredor Seguro' para evitar cinturones radiactivos.";
            else if (shipConfig.materialTier == ShipMaterialTier.Tier1_Aerospace_AlLi && selectedPropulsion != PropulsionMode.Chemical)
                tip = "Incompatibilidad de material: La aleación Al-Li no resiste el bombardeo ISM relativista ni la curvatura Alcubierre. Actualice a Tier 2 o Tier 3.";
            GUILayout.Label($"<color=yellow><i>{tip}</i></color>", smallStyle);

            GUILayout.Space(16);
            GUILayout.BeginHorizontal();
            GUI.color = new Color(0.2f, 1f, 0.6f);
            if (GUILayout.Button("🔄 REINTENTAR MISIÓN", buttonPrimaryStyle, GUILayout.Height(46)))
            {
                ResetToEarth();
                StartFlight();
            }
            GUI.color = new Color(0.3f, 0.8f, 1f);
            if (GUILayout.Button("🛠️ IR AL TALLER DE ENSAMBLAJE", buttonSecondaryStyle, GUILayout.Height(46)))
            {
                ResetToEarth();
                currentUIState = SimulationUIState.MainMenu;
                mainMenuTab = 1;
            }
            GUI.color = new Color(1f, 0.8f, 0.4f);
            if (GUILayout.Button("🏠 MENÚ PRINCIPAL", buttonSecondaryStyle, GUILayout.Height(46)))
            {
                ResetToEarth();
                currentUIState = SimulationUIState.MainMenu;
                mainMenuTab = 0;
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        /// <summary>
        /// Modal interactivo de verificación previa al despegue (Pre-flight Checklist) para los 5 subsistemas de cada nave.
        /// </summary>
        private void DrawPreFlightChecklistModal()
        {
            GUI.DrawTexture(new Rect(0, 0, 1920, 1080), texVignette);

            float modalW = 1060f;
            float modalH = 720f;
            float modalX = (1920f - modalW) * 0.5f;
            float modalY = (1080f - modalH) * 0.5f;

            GUI.Box(new Rect(modalX, modalY, modalW, modalH), "", panelBoxStyle);

            GUILayout.BeginArea(new Rect(modalX + 35, modalY + 22, modalW - 70, modalH - 44));

            string propIcon = (selectedPropulsion == PropulsionMode.Chemical) ? "🚀" : (selectedPropulsion == PropulsionMode.Antimatter ? "⚛️" : "🌌");
            string propName = (selectedPropulsion == PropulsionMode.Chemical) ? "PROPULSIÓN QUÍMICA CRIOGÉNICA (LOX/LH2)" : (selectedPropulsion == PropulsionMode.Antimatter ? "REACTOR DE ANTIMATERIA RELATIVISTA (HAZ DE PIONES)" : "MOTOR DE CURVATURA WARP ALCUBIERRE (MÉTRICA CASIMIR)");

            GUILayout.Label("🛠️ PROTOCOLO DE COMPROBACIÓN PRE-VUELO (PRE-FLIGHT CHECKLIST)", titleStyle);
            GUILayout.Space(2);
            GUILayout.Label($"Nave: <b>{propIcon} {propName}</b> | Destino: <color=cyan>{targetBody?.bodyName}</color> | Distancia: {SpaceConstants.FormatDistance(targetBody?.distanceFromEarthKm ?? 0)}", subtitleStyle);
            GUILayout.Space(12);

            (string sysName, string sysDesc)[] checks = selectedPropulsion switch
            {
                PropulsionMode.Antimatter => new (string, string)[]
                {
                    ("1. Trampa Superconductora Penning (Campo 12.4 T / 4.2 K Helio)", "Bobinas Nb3Sn energizadas a 850 A. Criostato con helio superfluido a 4.2 K nominal."),
                    ("2. Vacío Ultra-Alto de Confinamiento (P < 1.2×10⁻¹² Torr)", "Cámara de reacción libre de materia bariónica espuria para evitar aniquilaciones descontroladas."),
                    ("3. Sincronizador de Inyección de Antiprotones (Haz 187 MeV)", "Haz colimado de antiprotones calibrado y sincronizado con deflector electrostático de flujo."),
                    ("4. Refrigeración Criogénica de Tobera Magnética de Piones", "Bobinas magnéticas superconductoras de 45 T refrigeradas para guiar el chorro de piones cargados."),
                    ("5. Deflector Magnético ISM Interestelar & Blindaje de Aniquilación", "Pantalla magnética de deflexión frontal armada para dispersar polvo espacial a velocidades relativistas.")
                },
                PropulsionMode.WarpDrive => new (string, string)[]
                {
                    ("1. Cavidad Cuántica Casimir Nanométrica (Gap d = 10 nm)", "Placas reflectantes paralelas calibradas a nivel submicrónico en cámara de ultra-vacío cuántico."),
                    ("2. Generador de Energía Exótica Negativa (ρ < 0 Estabilizado)", "Inyección de densidad de energía negativa para contraer el espaciotiempo sin romper la métrica."),
                    ("3. Resonancia Armónica del Anillo Toroidal de Curvatura (4.85 GHz)", "Armónicos de radiofrecuencia calibrados para moldear el tensor energía-momento de Alcubierre."),
                    ("4. Calibración Geodésica de Caída Libre (Inercial Cero: 0.00 g)", "Garantiza aceleración sentida exactamente nula (a^μ = 0) dentro del volumen plano de la burbuja."),
                    ("5. Escudo de Horizonte de Burbuja Métrica & Radiación Hawking", "Filtro de contención cuántica para absorber radiación de corrimiento al azul en la pared frontal.")
                },
                _ => new (string, string)[]
                {
                    ("1. Presurización Criogénica Tanques LOX / LH2 (90 K / 20 K a 3.2 bar)", "Tanques presurizados con helio gaseoso regulado. Válvulas de alivio en ciclo cerrado nominal."),
                    ("2. Enfriamiento & Purga de Turbobombas (Chilldown de Combustión)", "Flujo criogénico activo por las camisas térmicas para prevenir cavitación destructiva por gas."),
                    ("3. Soporte Vital & Atmósfera de Cabina (101.3 kPa, 21% O2)", "Presión normalizada a 1 atmósfera, reciclador de CO2 activo y sellado hermético verificado."),
                    ("4. Deflexión y Cardán de Toberas Gimbal (Actuadores Hidráulicos ±8°)", "Servomotores probados en todo el rango angular de vectorización de empuje. Respuesta instantánea."),
                    ("5. Secuencia Pirotécnica de Ignición TEA-TEB & Sensores", "Inyectores piezoeléctricos comprobados y mezcla de encendido químico espontáneo armada.")
                }
            };

            bool allChecked = true;
            for (int i = 0; i < 5; i++)
            {
                if (!preFlightChecks[i]) allChecked = false;

                GUILayout.BeginHorizontal(presetCardStyle, GUILayout.Height(62));

                GUI.color = preFlightChecks[i] ? new Color(0.2f, 1f, 0.5f) : new Color(1f, 0.75f, 0.2f);
                string btnText = preFlightChecks[i] ? "✅ SISTEMA GO" : "⚠️ REVISAR";
                if (GUILayout.Button(btnText, buttonSecondaryStyle, GUILayout.Width(130), GUILayout.Height(44)))
                {
                    preFlightChecks[i] = !preFlightChecks[i];
                }
                GUI.color = Color.white;

                GUILayout.Space(12);
                GUILayout.BeginVertical();
                GUILayout.Label($"<b>{checks[i].sysName}</b>", bodyStyle);
                GUILayout.Label($"<size=12><color=#A0B4C8>{checks[i].sysDesc}</color></size>", smallStyle);
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                GUILayout.Space(4);
            }

            GUILayout.Space(8);

            GUILayout.BeginHorizontal();

            GUI.color = new Color(0.3f, 0.85f, 1f);
            if (GUILayout.Button("⚡ AUTO-CHECK AUTOMÁTICO (VERIFICAR TODOS)", buttonSecondaryStyle, GUILayout.Width(350), GUILayout.Height(48)))
            {
                for (int i = 0; i < 5; i++) preFlightChecks[i] = true;
            }
            GUI.color = Color.white;

            GUILayout.FlexibleSpace();

            if (allChecked)
            {
                GUILayout.Label("<color=#00FF99><b>✅ 5/5 SUBSISTEMAS EN ESTADO 'GO'\nAUTORIZACIÓN DE DESPEGUE CONCEDIDA</b></color>", bodyStyle);
            }
            else
            {
                int passed = 0;
                for (int i = 0; i < 5; i++) if (preFlightChecks[i]) passed++;
                GUILayout.Label($"<color=orange><b>⚠️ ESTADO: {passed}/5 REVISADOS\nComplete la lista para habilitar ignición</b></color>", bodyStyle);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(12);

            GUILayout.BeginHorizontal();
            if (allChecked)
            {
                GUI.color = new Color(0.2f, 1f, 0.5f);
                if (GUILayout.Button("🚀 INICIAR SECUENCIA DE DESPEGUE (CUENTA REGRESIVA 3, 2, 1...)", buttonPrimaryStyle, GUILayout.Height(52)))
                {
                    StartLaunchCountdown();
                }
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Box("⚠️ VERIFIQUE TODOS LOS SUBSISTEMAS PARA DESPEGAR", buttonSecondaryStyle, GUILayout.Height(52));
                GUI.color = Color.white;
            }

            GUILayout.Space(16);
            if (GUILayout.Button("✖ Volver / Cancelar", buttonSecondaryStyle, GUILayout.Width(180), GUILayout.Height(52)))
            {
                currentUIState = SimulationUIState.MainMenu;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        /// <summary>
        /// Superposición cinematográfica de cuenta regresiva (T-3, T-2, T-1, ¡IGNICIÓN/DESPEGUE!).
        /// </summary>
        private void DrawLaunchCountdownOverlay()
        {
            float bannerW = 860f;
            float bannerH = 190f;
            float bannerX = (1920f - bannerW) * 0.5f;
            float bannerY = 280f;

            GUI.Box(new Rect(bannerX, bannerY, bannerW, bannerH), "", panelBoxStyle);

            GUILayout.BeginArea(new Rect(bannerX + 20, bannerY + 15, bannerW - 40, bannerH - 30));

            if (launchCountdownTimer > 0f)
            {
                int count = Mathf.CeilToInt(launchCountdownTimer);
                string phaseMsg = "";
                string colorTag = "#00FFFF";

                if (launchCountdownTimer > 2.0f)
                {
                    phaseMsg = (selectedPropulsion == PropulsionMode.Chemical)
                        ? "SECUENCIA DE IGNICIÓN INICIADA... PURGA DE VÁLVULAS Y CHILLDOWN OK"
                        : (selectedPropulsion == PropulsionMode.Antimatter)
                            ? "INYECCIÓN DE HAZ DE ANTIPROTONES... CONFINAMIENTO PENNING 12.4 T"
                            : "ENERGIZACIÓN DE CAVIDAD CASIMIR... FORMACIÓN DE TENSORES MÉTRICOS";
                    colorTag = "#00FFCC";
                }
                else if (launchCountdownTimer > 1.0f)
                {
                    phaseMsg = (selectedPropulsion == PropulsionMode.Chemical)
                        ? "PRESURIZACIÓN DE TANQUES LOX/LH2 AL 100%... ACTUADORES GIMBAL ARMADOS"
                        : (selectedPropulsion == PropulsionMode.Antimatter)
                            ? "SINCRONIZACIÓN DE BOBINAS DE CONFINAMIENTO... VACÍO ULTRA-ALTO NOMINAL"
                            : "RESONANCIA CUÁNTICA EN 4.85 GHz... BURBUJA DE ALCUBIERRE ESTABILIZADA";
                    colorTag = "#FFDD00";
                }
                else
                {
                    phaseMsg = (selectedPropulsion == PropulsionMode.Chemical)
                        ? "¡MÁXIMA PRESIÓN EN CÁMARA! DESENCLAVE DE PLATAFORMA DE LANZAMIENTO..."
                        : (selectedPropulsion == PropulsionMode.Antimatter)
                            ? "¡DESBLOQUEO DE TOBERA MAGNÉTICA! GENERACIÓN DE FLUJO PIÓNICO..."
                            : "¡CONTRACCIÓN ESPACIOTEMPORAL MÁXIMA! SALTO DE CURVATURA INMINENTE...";
                    colorTag = "#FF5533";
                }

                GUIStyle countStyle = new GUIStyle(titleStyle)
                {
                    fontSize = 58,
                    alignment = TextAnchor.MiddleCenter
                };

                GUILayout.Label($"<color={colorTag}><b>T - {count}</b></color>", countStyle);
                GUILayout.Space(4);
                GUILayout.Label($"<b>{phaseMsg}</b>", subtitleStyle);
            }
            else if (launchBannerFadeTimer > 0f)
            {
                GUIStyle igniteStyle = new GUIStyle(titleStyle)
                {
                    fontSize = 44,
                    alignment = TextAnchor.MiddleCenter
                };

                string igniteTitle = (selectedPropulsion == PropulsionMode.WarpDrive)
                    ? "<color=#00FFFF>🌌 ¡WARP ENGAGE! BURBUJA ESTABILIZADA (10.0c)</color>"
                    : "<color=#00FF99>🚀 ¡IGNICIÓN & DESPEGUE NOMINAL!</color>";

                string igniteSub = (selectedPropulsion == PropulsionMode.WarpDrive)
                    ? "MÉTRICA EN EXPANSIÓN TRASERA Y CONTRACCIÓN FRONTAL — 0.00 g SENTIDO EN CABINA"
                    : "PROPULSIÓN A PLENA POTENCIA — TRAYECTORIA EN CURSO HACIA EL DESTINO";

                GUILayout.Label(igniteTitle, igniteStyle);
                GUILayout.Space(4);
                GUILayout.Label($"<b>{igniteSub}</b>", subtitleStyle);
            }

            GUILayout.EndArea();
        }

        /// <summary>
        /// Consola central de inspección holográfica para diales, tanques y estado detallado de subsistemas de cabina.
        /// Se abre dinámicamente al pulsar '🎛️ Diales y Tanques' o la tecla 'D'.
        /// </summary>
        private void DrawDashboardInspectionHUD()
        {
            float dashW = 1320f;
            float dashH = 580f;
            float dashX = 80f;
            float dashY = 410f;

            GUI.Box(new Rect(dashX, dashY, dashW, dashH), "", panelBoxStyle);

            GUILayout.BeginArea(new Rect(dashX + 25, dashY + 16, dashW - 50, dashH - 32));

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("🎛️ CONSOLA CENTRAL DE DIALES, TANQUES & INSTRUMENTACIÓN DEL SISTEMA", titleStyle);
            string subHeader = selectedPropulsion switch
            {
                PropulsionMode.Antimatter => "TELEMETRÍA EN TIEMPO REAL: TRAMPA DE PENNING, VACÍO ULTRA-ALTO & CONFINAMIENTO MAGNÉTICO DE PIONES",
                PropulsionMode.WarpDrive => "TELEMETRÍA EN TIEMPO REAL: CAVIDADES CASIMIR, FLUCTUACIONES DE VACÍO (ρ < 0) & TENSORES DE CURVATURA",
                _ => "TELEMETRÍA EN TIEMPO REAL: DINÁMICA DE TANQUES CRIOGÉNICOS, CÁMARA DE COMBUSTIÓN & TURBOBOMBAS"
            };
            GUILayout.Label(subHeader, subtitleStyle);
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUI.color = new Color(0.3f, 0.8f, 1f);
            if (GUILayout.Button("🔭 CERRAR INSPECCIÓN (Esc / D)", buttonSecondaryStyle, GUILayout.Width(240), GUILayout.Height(40)))
            {
                isInspectingDashboard = false;
                if (cockpitManager != null) cockpitManager.LookForward();
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(12);

            FlightCalculation calc = (targetBody != null)
                ? PropulsionPhysics.CalculateTrip(selectedPropulsion, targetBody.distanceFromEarthKm, shipConfig.GetTotalDryMassKg(), shipConfig.fuelLoadingPercentage)
                : default;
            float fuelFraction = (calc.fuelMassKg > 0.0) ? Mathf.Clamp01((float)(currentRemainingFuelKg / calc.fuelMassKg)) : 1.0f;

            float colW = (dashW - 50 - 45) / 4f;
            GUILayout.BeginHorizontal();

            // COLUMNA 1: TANQUES Y DEPÓSITOS
            GUILayout.BeginVertical(presetCardStyle, GUILayout.Width(colW), GUILayout.Height(450));
            GUILayout.Label("<b>1. TANQUES & DEPÓSITOS</b>", headerStyle);
            GUILayout.Space(4);

            if (selectedPropulsion == PropulsionMode.Chemical)
            {
                double loxKg = currentRemainingFuelKg * 0.857;
                double lh2Kg = currentRemainingFuelKg * 0.143;

                GUILayout.Label("<b>Tanque Criogénico LH2 (Hidrógeno):</b>", smallStyle);
                GUILayout.Label($"• Masa: <b>{lh2Kg:N0} kg</b> ({(fuelFraction * 100f):F1}%)", smallStyle);
                GUILayout.Label("• Presión: <b>3.22 bar</b> | Temp: <b>20.3 K</b>", smallStyle);
                Rect b1 = GUILayoutUtility.GetRect(colW - 20, 10);
                GUI.DrawTexture(b1, texPanelAccent);
                GUI.DrawTexture(new Rect(b1.x, b1.y, b1.width * fuelFraction, b1.height), texBarCyan);

                GUILayout.Space(8);
                GUILayout.Label("<b>Tanque Criogénico LOX (Oxígeno):</b>", smallStyle);
                GUILayout.Label($"• Masa: <b>{loxKg:N0} kg</b> ({(fuelFraction * 100f):F1}%)", smallStyle);
                GUILayout.Label("• Presión: <b>2.95 bar</b> | Temp: <b>90.2 K</b>", smallStyle);
                Rect b2 = GUILayoutUtility.GetRect(colW - 20, 10);
                GUI.DrawTexture(b2, texPanelAccent);
                GUI.DrawTexture(new Rect(b2.x, b2.y, b2.width * fuelFraction, b2.height), texBarGreen);

                GUILayout.Space(10);
                GUILayout.Label("• Mezcla Estequiométrica: <b>O/F 6.0:1</b>", smallStyle);
                GUILayout.Label("• Líneas de Purga: <b>Presurizadas (He)</b>", smallStyle);
            }
            else if (selectedPropulsion == PropulsionMode.Antimatter)
            {
                GUILayout.Label("<b>Trampa Superconductora Penning:</b>", smallStyle);
                GUILayout.Label($"• Antiprotones (p̄): <b>{currentRemainingFuelKg:F3} kg</b>", smallStyle);
                GUILayout.Label($"• Carga Cuántica: <b>{(fuelFraction * 100f):F1}%</b>", smallStyle);
                Rect b1 = GUILayoutUtility.GetRect(colW - 20, 10);
                GUI.DrawTexture(b1, texPanelAccent);
                GUI.DrawTexture(new Rect(b1.x, b1.y, b1.width * fuelFraction, b1.height), texBarCyan);

                GUILayout.Space(8);
                GUILayout.Label("• Campo Magnético: <b>12.40 Tesla</b>", smallStyle);
                GUILayout.Label("• Criostato Helio 4: <b>4.20 K</b> (Superfluido)", smallStyle);
                GUILayout.Label("• Cámara Ultra-Vacío: <b>1.12×10⁻¹² Torr</b>", smallStyle);

                GUILayout.Space(10);
                GUILayout.Label("<b>Depósito de Hidrógeno Bariónico:</b>", smallStyle);
                GUILayout.Label("• Conexión Inyector: <b>Presión 4.5 bar</b>", smallStyle);
            }
            else
            {
                GUILayout.Label("<b>Banco Cuántico Casimir (ρ < 0):</b>", smallStyle);
                GUILayout.Label($"• Reserva Exótica: <b>{calc.exoticEnergyJoules:E2} J</b>", smallStyle);
                GUILayout.Label($"• Nivel de Carga: <b>{(fuelFraction * 100f):F1}%</b>", smallStyle);
                Rect b1 = GUILayoutUtility.GetRect(colW - 20, 10);
                GUI.DrawTexture(b1, texPanelAccent);
                GUI.DrawTexture(new Rect(b1.x, b1.y, b1.width * fuelFraction, b1.height), texBarCyan);

                GUILayout.Space(8);
                GUILayout.Label("<color=#7FD5FF><i>* Masa de reacción expulsada: 0 kg (Propulsión no newtoniana)</i></color>", smallStyle);
                GUILayout.Space(8);
                GUILayout.Label("• Separación de Placas: <b>d = 10.0 nm</b>", smallStyle);
                GUILayout.Label("• Fuerza Atractiva Casimir: <b>-1.30 mN/m²</b>", smallStyle);
                GUILayout.Label("• Fluctuación de Vacío: <b>Regulada</b>", smallStyle);
            }

            GUILayout.EndVertical();
            GUILayout.Space(15);

            // COLUMNA 2: CÁMARA & TOBERA
            GUILayout.BeginVertical(presetCardStyle, GUILayout.Width(colW), GUILayout.Height(450));
            GUILayout.Label("<b>2. CÁMARA & TOBERA</b>", headerStyle);
            GUILayout.Space(4);

            if (selectedPropulsion == PropulsionMode.Chemical)
            {
                GUILayout.Label("<b>Turbobombas Principales:</b>", smallStyle);
                GUILayout.Label("• Turbobomba LH2: <b>34,800 RPM</b>", smallStyle);
                GUILayout.Label("• Turbobomba LOX: <b>14,200 RPM</b>", smallStyle);
                GUILayout.Label("• Presión Descarga: <b>275 bar</b>", smallStyle);

                GUILayout.Space(8);
                GUILayout.Label("<b>Cámara de Combustión:</b>", smallStyle);
                GUILayout.Label("• Presión Pc: <b>206.4 bar</b>", smallStyle);
                GUILayout.Label("• Temperatura Gases: <b>3,580 K</b>", smallStyle);
                GUILayout.Label("• Impulso Específico: <b>452 s (Vacío)</b>", smallStyle);
                GUILayout.Label("• Expansión Tobera: <b>ε = 77.5 : 1</b>", smallStyle);
            }
            else if (selectedPropulsion == PropulsionMode.Antimatter)
            {
                GUILayout.Label("<b>Anillo de Reacción p-p̄:</b>", smallStyle);
                GUILayout.Label("• Energía Inyección: <b>187.3 MeV</b>", smallStyle);
                GUILayout.Label("• Tasa Aniquilación: <b>1.45×10¹⁶ /s</b>", smallStyle);
                GUILayout.Label("• Piones Cargados π±: <b>40.0%</b>", smallStyle);
                GUILayout.Label("• Rayos Gamma π0: <b>60.0%</b>", smallStyle);

                GUILayout.Space(8);
                GUILayout.Label("<b>Tobera Magnética Superconductora:</b>", smallStyle);
                GUILayout.Label("• Campo Confinamiento: <b>45.0 Tesla</b>", smallStyle);
                GUILayout.Label("• Velocidad Escape: <b>0.33c - 0.50c</b>", smallStyle);
                GUILayout.Label("• Eficiencia Magnética: <b>99.4%</b>", smallStyle);
            }
            else
            {
                GUILayout.Label("<b>Anillo Toroidal de Curvatura:</b>", smallStyle);
                GUILayout.Label("• Frecuencia Resonancia: <b>4.85 GHz</b>", smallStyle);
                GUILayout.Label("• Tensor Alcubierre-White: <b>Conforme</b>", smallStyle);
                GUILayout.Label("• Coherencia Métrica: <b>99.98%</b>", smallStyle);

                GUILayout.Space(8);
                GUILayout.Label("<b>Deformación del Espaciotiempo:</b>", smallStyle);
                GUILayout.Label("• Contracción Frontal θ: <b>-10.0c</b>", smallStyle);
                GUILayout.Label("• Expansión Trasera θ: <b>+10.0c</b>", smallStyle);
                GUILayout.Label("• Velocidad Coordinada: <b>10.0c (Aparente)</b>", smallStyle);
                GUILayout.Label("• Dilatación dτ: <b>dτ = dt (Sin desfase)</b>", smallStyle);
            }

            GUILayout.EndVertical();
            GUILayout.Space(15);

            // COLUMNA 3: ACCIONES & CONTROL INTERACTIVO
            GUILayout.BeginVertical(presetCardStyle, GUILayout.Width(colW), GUILayout.Height(450));
            GUILayout.Label("<b>3. MANDOS ACTIVOS</b>", headerStyle);
            GUILayout.Space(4);

            if (selectedPropulsion == PropulsionMode.Chemical)
            {
                GUILayout.Label("<b>Control de Válvulas y Cardán:</b>", smallStyle);
                if (GUILayout.Button("💨 Purgar Sobrepresión Criogénica", buttonSecondaryStyle, GUILayout.Height(36)))
                {
                    survivalSystem.PurgeCryoValves();
                }
                GUILayout.Space(6);
                if (GUILayout.Button("🔧 Centrar Gimbal Hidráulico (0°)", buttonSecondaryStyle, GUILayout.Height(36)))
                {
                    // Gimbal centrado nominal
                }
                GUILayout.Space(6);
                GUILayout.Label($"• Estrés Térmico: <b>{survivalSystem.engineThermalStressPct:F1}%</b>", smallStyle);
                GUILayout.Label("• Sistema Gimbal: <b>Actuadores Hidráulicos GO</b>", smallStyle);
            }
            else if (selectedPropulsion == PropulsionMode.Antimatter)
            {
                GUILayout.Label("<b>Deflexión y Confinamiento:</b>", smallStyle);
                string ismLabel = survivalSystem.isISMDeflectorActive ? "🛡️ Escudo ISM: [ACTIVO]" : "🛡️ Escudo ISM: [DESACTIVADO]";
                GUI.color = survivalSystem.isISMDeflectorActive ? Color.cyan : Color.white;
                if (GUILayout.Button(ismLabel, buttonSecondaryStyle, GUILayout.Height(36)))
                {
                    survivalSystem.ToggleISMDeflector();
                }
                GUI.color = Color.white;
                GUILayout.Space(6);
                if (GUILayout.Button("🔄 Recalibrar Bobinas HTS", buttonSecondaryStyle, GUILayout.Height(36)))
                {
                    survivalSystem.engineThermalStressPct = Mathf.Max(0f, survivalSystem.engineThermalStressPct - 15f);
                }
                GUILayout.Space(6);
                GUILayout.Label($"• Estrés Térmico Tobera: <b>{survivalSystem.engineThermalStressPct:F1}%</b>", smallStyle);
                GUILayout.Label($"• Escudo Deflector: <b>{(survivalSystem.isISMDeflectorActive ? "2.5 MW Potencia" : "Standby")}</b>", smallStyle);
            }
            else
            {
                GUILayout.Label("<b>Estabilidad Geométrica Burbuja:</b>", smallStyle);
                if (GUILayout.Button("⚡ Armonizar Frecuencia Casimir", buttonSecondaryStyle, GUILayout.Height(36)))
                {
                    survivalSystem.warpBubbleStabilityPct = 100f;
                }
                GUILayout.Space(6);
                if (GUILayout.Button("🌐 Purgar Radiación de Horizonte", buttonSecondaryStyle, GUILayout.Height(36)))
                {
                    survivalSystem.accumulatedRadiation_mSv = System.Math.Max(0.0, survivalSystem.accumulatedRadiation_mSv - 15.0);
                }
                GUILayout.Space(6);
                GUILayout.Label($"• Estabilidad Burbuja: <b>{survivalSystem.warpBubbleStabilityPct:F1}%</b>", smallStyle);
                GUILayout.Label("• Horizonte Métrica: <b>Blindaje Cuántico Nominal</b>", smallStyle);
            }

            GUILayout.Space(12);
            GUI.color = new Color(0.2f, 1f, 0.6f);
            if (GUILayout.Button("🔭 Mirar al Frente (Vista Exterior)", buttonPrimaryStyle, GUILayout.Height(44)))
            {
                isInspectingDashboard = false;
                if (cockpitManager != null) cockpitManager.LookForward();
            }
            GUI.color = Color.white;

            GUILayout.EndVertical();
            GUILayout.Space(15);

            // COLUMNA 4: SENSORES INERCIALES Y CABINA
            GUILayout.BeginVertical(presetCardStyle, GUILayout.Width(colW), GUILayout.Height(450));
            GUILayout.Label("<b>4. CABINA & GRAVEDAD</b>", headerStyle);
            GUILayout.Space(4);

            GUILayout.Label("<b>Acelerómetro Inercial:</b>", smallStyle);
            string gDesc = (selectedPropulsion == PropulsionMode.WarpDrive)
                ? "<color=#00FFFF><b>0.00 g</b> (Caída Libre Geodésica Pura)</color>"
                : (selectedPropulsion == PropulsionMode.Antimatter)
                    ? "<color=yellow><b>1.50 g</b> (Aceleración Relativista)</color>"
                    : "<color=orange><b>3.50 g</b> (Empuje Físico Químico)</color>";
            GUILayout.Label($"• Fuerza G: {gDesc}", smallStyle);

            if (selectedPropulsion == PropulsionMode.WarpDrive)
            {
                GUILayout.Label("<size=11><i>La tripulación no siente aceleración porque el espacio se contrae alrededor; interior estrictamente plano (a^μ = 0).</i></size>", smallStyle);
            }
            else
            {
                GUILayout.Label("<size=11><i>Fuerza transmitida por compresión mecánica de los motores al chasis y asientos.</i></size>", smallStyle);
            }

            GUILayout.Space(10);
            GUILayout.Label("<b>Soporte Vital & Habitáculo:</b>", smallStyle);
            GUILayout.Label("• Presión Interna: <b>101.3 kPa (1.00 atm)</b>", smallStyle);
            GUILayout.Label("• Atmósfera: <b>20.9% O₂, 78.1% N₂, 0.038% CO₂</b>", smallStyle);
            GUILayout.Label("• Temperatura de Cabina: <b>21.4 °C</b>", smallStyle);
            GUILayout.Label($"• Radiación Cabina: <b>{survivalSystem.accumulatedRadiation_mSv:F2} mSv</b>", smallStyle);

            GUILayout.Space(10);
            GUILayout.Label("<b>Estado Estructural Casco:</b>", smallStyle);
            GUILayout.Label($"• Integridad Casco: <b>{survivalSystem.hullIntegrityPct:F1}%</b>", smallStyle);
            Rect bHull = GUILayoutUtility.GetRect(colW - 20, 10);
            GUI.DrawTexture(bHull, texPanelAccent);
            GUI.DrawTexture(new Rect(bHull.x, bHull.y, bHull.width * (survivalSystem.hullIntegrityPct / 100f), bHull.height), (survivalSystem.hullIntegrityPct > 50f ? texBarGreen : texBarRed));

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        /// <summary>
        /// Dibuja etiquetas proyectadas en pantalla sobre cada planeta, hito espacial y zonas de peligro de radiación en el mapa 2D.
        /// </summary>
        private void DrawInteractiveBodyLabels()
        {
            if (currentPerspective != ViewPerspective.TacticalMap2D) return;
            Camera cam = Camera.main;
            if (cam == null) return;

            for (int i = 0; i < celestialBodies.Count; i++)
            {
                var body = celestialBodies[i];
                if (body == null) continue;

                Vector3 screenP = cam.WorldToScreenPoint(body.transform.position);
                if (screenP.z > 0 && screenP.x >= 0 && screenP.x <= Screen.width && screenP.y >= 0 && screenP.y <= Screen.height)
                {
                    // Conversión a coordenadas virtuales 1920x1080
                    float vx = screenP.x * (1920f / Screen.width);
                    float vy = (Screen.height - screenP.y) * (1080f / Screen.height);

                    bool isTarget = (body == targetBody);
                    string labelText = isTarget ? $"▶ {body.bodyName}" : body.bodyName;
                    int labelWidth = Mathf.Max(100, labelText.Length * 9 + 18);
                    Rect labelRect = new Rect(vx - (labelWidth / 2f), vy + 16, labelWidth, 26);

                    GUI.color = isTarget ? Color.cyan : new Color(1f, 1f, 1f, 0.90f);
                    if (GUI.Button(labelRect, labelText, buttonSecondaryStyle))
                    {
                        SetDestination(i);
                    }
                    GUI.color = Color.white;

                    // Megagrafía de Zonas de Peligro Ambiental y Radiación (Triángulos y Círculos de Alerta)
                    if (body.bodyName.Contains("Tierra"))
                    {
                        Rect vanAllenRect = new Rect(vx - 145, vy + 46, 290, 24);
                        GUI.color = new Color(1f, 0.85f, 0.2f, 0.95f);
                        GUI.Box(vanAllenRect, "⚠️ ZONA DE RADIACIÓN: Cinturón Van Allen (20 mSv/h)", smallStyle);
                        GUI.color = Color.white;
                    }
                    else if (body.bodyName.Contains("Sol"))
                    {
                        Rect sunRadRect = new Rect(vx - 165, vy + 46, 330, 24);
                        GUI.color = new Color(1f, 0.70f, 0.1f, 0.95f);
                        GUI.Box(sunRadRect, "⚠️ RADIACIÓN: Viento Cósmico & Fulguraciones (1/r²)", smallStyle);
                        GUI.color = Color.white;

                        Rect sunThermRect = new Rect(vx - 165, vy + 72, 330, 24);
                        GUI.color = new Color(1f, 0.25f, 0.15f, 0.95f);
                        GUI.Box(sunThermRect, "🔴 PELIGRO TÉRMICO: Corona Solar (T > 2.500 K - Fusión)", smallStyle);
                        GUI.color = Color.white;
                    }
                    else if (body.bodyName.Contains("Júpiter"))
                    {
                        Rect jupHazardRect = new Rect(vx - 160, vy + 46, 320, 24);
                        GUI.color = new Color(1f, 0.35f, 0.1f, 0.95f);
                        GUI.Box(jupHazardRect, "⚠️ RADIACIÓN: Magnetosfera Sincrotrón (10-50 Sv/d)", smallStyle);
                        GUI.color = Color.white;
                    }
                    else if (body.bodyName.Contains("Ceres") || body.bodyName.Contains("Asteroide"))
                    {
                        Rect astHazardRect = new Rect(vx - 160, vy + 46, 320, 24);
                        GUI.color = new Color(1f, 0.3f, 0.3f, 0.95f);
                        GUI.Box(astHazardRect, "🔴 EROSIÓN ISM: Polvo de Asteroides (>35 km/s)", smallStyle);
                        GUI.color = Color.white;
                    }
                    else if (body.bodyName.Contains("Púlsar") || body.bodyName.Contains("PSR"))
                    {
                        Rect pulsarRect = new Rect(vx - 160, vy + 46, 320, 24);
                        GUI.color = new Color(0.95f, 0.3f, 1f, 0.95f);
                        GUI.Box(pulsarRect, "⚠️ RADIACIÓN: Haz Gamma Relativista (0.95 Sv/s)", smallStyle);
                        GUI.color = Color.white;
                    }
                    else if (body.bodyName.Contains("Agujero Negro") || body.bodyName.Contains("Cygnus"))
                    {
                        Rect bhRect = new Rect(vx - 160, vy + 46, 320, 24);
                        GUI.color = new Color(1f, 0.15f, 0.15f, 0.95f);
                        GUI.Box(bhRect, "🔴 DISCO DE ACRECIÓN: Mareas Gravitacionales 22g", smallStyle);
                        GUI.color = Color.white;
                    }
                }
            }
        }
    }
}
