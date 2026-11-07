using UnityEngine;
using Alcubierre;

namespace Alcubierre.Cockpit
{
    /// <summary>
    /// Gestiona la visualización del entorno exterior visible desde el parabrisas de las cabinas 3D:
    /// 1. Secuencia realista de despegue y transición atmosférica:
    ///    - En superficie: Cielo azul diurno Rayleigh con nubes y horizonte terrestre visible.
    ///    - Ascenso transónico/supersónico: Condensación y estelas de nubes pasando por la ventana.
    ///    - Estratosfera y Mesosfera: Azul celeste -> Cobalto profundo -> Índigo Kármán (100 km) -> Negro cósmico absoluto.
    /// 2. Horizonte terrestre curvo que se aleja conforme la nave gana altitud orbital.
    /// 3. Campo estelar permanente de alta fidelidad (450 estrellas) con polvo cósmico luminoso sin colapso recursivo.
    /// 4. Facsímil planetario continuo que crece desde disco distante hasta orbe planetario iluminado por el Sol.
    /// 5. Efectos relativistas de haz frontal (Headlight Beaming), corrimiento Doppler y halo toroidal Warp Casimir.
    /// </summary>
    public class CockpitExteriorView : MonoBehaviour
    {
        [Header("Referencias")]
        public Transform cockpitRoot;
        public Camera povCamera;

        [Header("Sol Exterior y Luz Celestial")]
        public Light ambientSunLight;
        public GameObject sunDiskObj;

        [Header("Horizonte Terrestre (Despegue y Ascenso)")]
        public GameObject earthHorizonObj;
        public MeshRenderer earthHorizonRenderer;

        [Header("Facsímil Planetario de Destino")]
        public GameObject proximityPlanetObj;
        public MeshRenderer planetRenderer;
        public Material planetMaterial;

        [Header("Halo Toroidal Warp (Alcubierre)")]
        public GameObject warpTorusHaloObj;
        public MeshRenderer warpHaloRenderer;

        // Partículas del campo estelar y polvo cósmico
        private ParticleSystem starFieldPS;
        private ParticleSystem.Particle[] stars;
        private Vector3[] starBaseCoords;       // Coordenadas base inmutables (evita colapso a 0,0)
        private float[] starZDepths;            // Profundidad z individual
        private Color[] starBaseColors;         // Colores espectrales astronómicos (O, B, A, G, M)
        private const int MaxStars = 450;

        // Polvo cósmico interestelar
        private ParticleSystem cosmicDustPS;
        private const int MaxDust = 120;

        // Nubes atmosféricas de baja cota durante el ascenso
        private ParticleSystem cloudWispsPS;

        // Estado del vuelo
        private bool isFlying = false;
        private float flightProgress = 0f;
        private double velocityFractionC = 0.0;
        private double feltGForce = 0.0;
        private PropulsionMode activeMode = PropulsionMode.Chemical;
        private Color currentTargetColor = Color.white;
        private string currentTargetName = "";

        // Shaking
        public Vector3 CurrentShakeOffset { get; private set; } = Vector3.zero;
        private float shakeIntensity = 0f;

        public void Initialize(Transform rootTransform, Camera cameraRef)
        {
            cockpitRoot = rootTransform;
            povCamera = cameraRef;

            BuildAtmosphereAndLight();
            BuildEarthHorizon();
            BuildStarfield();
            BuildCosmicDust();
            BuildAtmosphericClouds();
            BuildWarpTorusHalo();
            BuildProximityPlanet();
        }

        private void BuildAtmosphereAndLight()
        {
            // Luz solar direccional suave que entra oblicua por el parabrisas
            GameObject sunLightObj = new GameObject("Cockpit_ExternalSunLight");
            sunLightObj.transform.SetParent(transform, false);
            sunLightObj.transform.localPosition = new Vector3(8f, 10f, 6f);
            sunLightObj.transform.localRotation = Quaternion.Euler(38f, -28f, 0f);

            ambientSunLight = sunLightObj.AddComponent<Light>();
            ambientSunLight.type = LightType.Directional;
            ambientSunLight.color = new Color(1f, 0.98f, 0.93f);
            ambientSunLight.intensity = 1.35f;

            // Disco solar visible en el firmamento distante
            sunDiskObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sunDiskObj.name = "Celestial_SunDisk";
            sunDiskObj.transform.SetParent(transform, false);
            sunDiskObj.transform.localPosition = new Vector3(18f, 14f, 42f);
            sunDiskObj.transform.localScale = Vector3.one * 3.2f;

            Collider col = sunDiskObj.GetComponent<Collider>();
            if (col != null) Destroy(col);

            MeshRenderer sunRen = sunDiskObj.GetComponent<MeshRenderer>();
            Shader unlit = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color") ?? Shader.Find("Sprites/Default");
            Material sunMat = new Material(unlit);
            sunMat.color = new Color(1f, 0.98f, 0.85f);
            if (sunMat.HasProperty("_BaseColor")) sunMat.SetColor("_BaseColor", new Color(1f, 0.98f, 0.85f));
            sunRen.material = sunMat;
        }

        private void BuildEarthHorizon()
        {
            // Orbe gigante curvo colocado debajo del morro para simular la curvatura de la Tierra durante el despegue
            earthHorizonObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            earthHorizonObj.name = "Earth_LimbHorizon";
            earthHorizonObj.transform.SetParent(transform, false);
            earthHorizonObj.transform.localPosition = new Vector3(0f, -14f, 12f);
            earthHorizonObj.transform.localScale = new Vector3(36f, 36f, 36f);

            Collider col = earthHorizonObj.GetComponent<Collider>();
            if (col != null) Destroy(col);

            earthHorizonRenderer = earthHorizonObj.GetComponent<MeshRenderer>();
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material earthMat = new Material(litShader);
            earthMat.color = new Color(0.18f, 0.42f, 0.75f); // Azul oceánico con relieve
            if (earthMat.HasProperty("_BaseColor")) earthMat.SetColor("_BaseColor", new Color(0.18f, 0.42f, 0.75f));
            if (earthMat.HasProperty("_Smoothness")) earthMat.SetFloat("_Smoothness", 0.55f);
            earthHorizonRenderer.material = earthMat;
        }

        private void BuildStarfield()
        {
            GameObject psObj = new GameObject("Cockpit_StarfieldSystem");
            psObj.transform.SetParent(transform, false);
            psObj.transform.localPosition = Vector3.zero;

            starFieldPS = psObj.AddComponent<ParticleSystem>();
            var main = starFieldPS.main;
            main.loop = true;
            main.playOnAwake = true;
            main.maxParticles = MaxStars;
            main.startLifetime = 10000f;
            main.startSpeed = 0f;
            main.startSize = 0.22f;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;

            var emission = starFieldPS.emission;
            emission.rateOverTime = 0f;

            var shape = starFieldPS.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(36f, 26f, 50f);

            var rend = psObj.GetComponent<ParticleSystemRenderer>();
            Shader particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit") ??
                                   Shader.Find("Particles/Standard Unlit") ??
                                   Shader.Find("Mobile/Particles/Additive") ??
                                   Shader.Find("Sprites/Default");
            Material starMat = new Material(particleShader);
            Texture2D glowTex = CreateSoftGlowTexture(32);
            starMat.mainTexture = glowTex;
            if (starMat.HasProperty("_BaseMap")) starMat.SetTexture("_BaseMap", glowTex);
            rend.material = starMat;

            // Arrays de estrellas con coordenadas base inmutables para evitar cualquier colapso numérico
            stars = new ParticleSystem.Particle[MaxStars];
            starBaseCoords = new Vector3[MaxStars];
            starZDepths = new float[MaxStars];
            starBaseColors = new Color[MaxStars];

            // Paleta espectral estelar: Blanco azulado (O/B), Blanco (A), Amarillo solar (G) y Ámbar/Rojo (K/M)
            Color[] spectralTints = new Color[]
            {
                new Color(0.85f, 0.95f, 1.0f, 1.0f),  // Blanco azulado (Clase B/A)
                new Color(1.0f, 1.0f, 1.0f, 1.0f),    // Blanco puro
                new Color(1.0f, 0.96f, 0.88f, 1.0f),  // Blanco cálido / Solar (Clase G)
                new Color(1.0f, 0.85f, 0.65f, 1.0f),  // Naranja suave (Clase K)
                new Color(0.65f, 0.88f, 1.0f, 1.0f)   // Azul brillante
            };

            for (int i = 0; i < MaxStars; i++)
            {
                starBaseCoords[i] = new Vector3(
                    Random.Range(-20f, 20f),
                    Random.Range(-13f, 13f),
                    0f
                );
                starZDepths[i] = Random.Range(4.5f, 54f);
                starBaseColors[i] = spectralTints[Random.Range(0, spectralTints.Length)];

                stars[i].position = new Vector3(starBaseCoords[i].x, starBaseCoords[i].y, starZDepths[i]);
                stars[i].startSize = Random.Range(0.16f, 0.34f);
                stars[i].startColor = starBaseColors[i];
                stars[i].remainingLifetime = 10000f;
            }

            starFieldPS.SetParticles(stars, MaxStars);
        }

        private void BuildCosmicDust()
        {
            GameObject dustObj = new GameObject("Cockpit_CosmicDustSystem");
            dustObj.transform.SetParent(transform, false);
            dustObj.transform.localPosition = Vector3.zero;

            cosmicDustPS = dustObj.AddComponent<ParticleSystem>();
            var main = cosmicDustPS.main;
            main.loop = true;
            main.playOnAwake = true;
            main.maxParticles = MaxDust;
            main.startLifetime = 3.5f;
            main.startSpeed = 8.0f;
            main.startSize = 0.045f;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;

            var emission = cosmicDustPS.emission;
            emission.rateOverTime = 30f;

            var shape = cosmicDustPS.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.position = new Vector3(0f, 0f, 18f);
            shape.scale = new Vector3(16f, 10f, 2f);
            shape.rotation = new Vector3(0f, 180f, 0f); // Se mueven hacia la cabina

            var rend = dustObj.GetComponent<ParticleSystemRenderer>();
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit") ??
                           Shader.Find("Particles/Standard Unlit") ??
                           Shader.Find("Sprites/Default");
            Material dustMat = new Material(shader);
            dustMat.color = new Color(0.9f, 0.95f, 1f, 0.75f);
            rend.material = dustMat;

            cosmicDustPS.gameObject.SetActive(false);
        }

        private void BuildAtmosphericClouds()
        {
            GameObject cloudObj = new GameObject("Cockpit_AtmosphericVaporSystem");
            cloudObj.transform.SetParent(transform, false);
            cloudObj.transform.localPosition = new Vector3(0f, 0f, 4.5f);

            cloudWispsPS = cloudObj.AddComponent<ParticleSystem>();
            var main = cloudWispsPS.main;
            main.loop = true;
            main.playOnAwake = true;
            main.maxParticles = 60;
            main.startLifetime = 1.4f;
            main.startSpeed = 16.0f;
            main.startSize = 1.8f;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;

            var emission = cloudWispsPS.emission;
            emission.rateOverTime = 22f;

            var shape = cloudWispsPS.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.position = new Vector3(0f, 4f, 0f);
            shape.scale = new Vector3(12f, 1f, 8f);
            shape.rotation = new Vector3(90f, 0f, 0f); // Estelas bajando verticalmente en el ascenso

            var rend = cloudObj.GetComponent<ParticleSystemRenderer>();
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit") ??
                           Shader.Find("Particles/Standard Unlit") ??
                           Shader.Find("Sprites/Default");
            Material cloudMat = new Material(shader);
            cloudMat.color = new Color(1f, 1f, 1f, 0.35f);
            Texture2D tex = CreateSoftGlowTexture(64);
            cloudMat.mainTexture = tex;
            rend.material = cloudMat;

            cloudWispsPS.gameObject.SetActive(false);
        }

        private void BuildWarpTorusHalo()
        {
            warpTorusHaloObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            warpTorusHaloObj.name = "Warp_TorusCanopyHalo";
            warpTorusHaloObj.transform.SetParent(transform, false);
            warpTorusHaloObj.transform.localPosition = new Vector3(0f, 0.35f, 2.75f);
            warpTorusHaloObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            warpTorusHaloObj.transform.localScale = new Vector3(2.85f, 0.02f, 1.8f);

            Destroy(warpTorusHaloObj.GetComponent<Collider>());

            warpHaloRenderer = warpTorusHaloObj.GetComponent<MeshRenderer>();
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
            Material haloMat = new Material(shader);
            haloMat.color = new Color(0.1f, 0.8f, 1f, 0.25f);
            warpHaloRenderer.material = haloMat;

            warpTorusHaloObj.SetActive(false);
        }

        private void BuildProximityPlanet()
        {
            proximityPlanetObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            proximityPlanetObj.name = "ProximityPlanet_Facsimile";
            proximityPlanetObj.transform.SetParent(transform, false);
            proximityPlanetObj.transform.localPosition = new Vector3(0f, 0.35f, 32f);
            proximityPlanetObj.transform.localScale = Vector3.zero;

            Destroy(proximityPlanetObj.GetComponent<Collider>());

            planetRenderer = proximityPlanetObj.GetComponent<MeshRenderer>();
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            planetMaterial = new Material(litShader);
            planetMaterial.color = new Color(0.85f, 0.85f, 0.88f); // Tono lunar ceniciento por defecto
            if (planetMaterial.HasProperty("_Smoothness")) planetMaterial.SetFloat("_Smoothness", 0.35f);
            planetRenderer.material = planetMaterial;

            proximityPlanetObj.SetActive(false);
        }

        public void SetTargetDestination(string name, Color color)
        {
            currentTargetName = name;
            currentTargetColor = color;

            if (planetMaterial != null)
            {
                if (name.Contains("Luna"))
                {
                    planetMaterial.color = new Color(0.82f, 0.82f, 0.86f);
                }
                else if (name.Contains("Marte"))
                {
                    planetMaterial.color = new Color(0.88f, 0.42f, 0.24f);
                }
                else if (name.Contains("Júpiter") || name.Contains("Saturno"))
                {
                    planetMaterial.color = new Color(0.85f, 0.65f, 0.45f);
                }
                else
                {
                    planetMaterial.color = color;
                }
            }
        }

        public void UpdateFlightTelemetry(bool flying, float progress, double vFractionC, double gForce, PropulsionMode mode)
        {
            isFlying = flying;
            flightProgress = progress;
            velocityFractionC = vFractionC;
            feltGForce = gForce;
            activeMode = mode;
        }

        private void Update()
        {
            UpdateSkyColorAndAtmosphere();
            UpdateEarthHorizon();
            UpdateStarfieldOptics();
            UpdateCosmicDust();
            UpdateWarpHaloVisual();
            UpdateProximityPlanetFacsimile();
            UpdateCockpitShake();
        }

        /// <summary>
        /// Transición fotométricamente coherente del cielo diurno de despegue a negro profundo interestelar.
        /// - 0% a 3%: Azul celeste diurno Rayleigh (Tierra al nivel del mar).
        /// - 3% a 7%: Azul cobalto estratosférico (20 km a 50 km).
        /// - 7% a 11%: Índigo violáceo / Línea de Kármán (100 km).
        /// - >11%: Negro espacial profundo (0.003, 0.003, 0.008) donde brillan las estrellas.
        /// </summary>
        private void UpdateSkyColorAndAtmosphere()
        {
            if (povCamera == null) return;

            // Fase 1: Ascenso atmosférico terrestre (primer 11% de la trayectoria)
            if (flightProgress < 0.11f)
            {
                float t = Mathf.Clamp01(flightProgress / 0.11f);

                Color skySeaLevel = new Color(0.36f, 0.68f, 0.98f);
                Color skyStrato = new Color(0.10f, 0.24f, 0.62f);
                Color skyKarman = new Color(0.025f, 0.035f, 0.11f);
                Color spaceBlack = new Color(0.003f, 0.003f, 0.008f);

                Color currentSky;
                if (t < 0.45f)
                {
                    currentSky = Color.Lerp(skySeaLevel, skyStrato, t / 0.45f);
                }
                else if (t < 0.85f)
                {
                    currentSky = Color.Lerp(skyStrato, skyKarman, (t - 0.45f) / 0.40f);
                }
                else
                {
                    currentSky = Color.Lerp(skyKarman, spaceBlack, (t - 0.85f) / 0.15f);
                }

                povCamera.backgroundColor = currentSky;
                povCamera.clearFlags = CameraClearFlags.SolidColor;

                // Nubes y condensación activas durante el ascenso denso (0% a 7%)
                if (cloudWispsPS != null)
                {
                    bool showClouds = isFlying && (flightProgress < 0.07f);
                    if (cloudWispsPS.gameObject.activeSelf != showClouds)
                    {
                        cloudWispsPS.gameObject.SetActive(showClouds);
                    }
                }
            }
            // Fase 3: Reentrada o aproximación atmosférica en destino
            else if (flightProgress >= 0.88f && (currentTargetName.Contains("Marte") || currentTargetName.Contains("Venus") || currentTargetName.Contains("Tierra")))
            {
                float tRe = (flightProgress - 0.88f) / 0.12f;
                Color targetSky = currentTargetName.Contains("Marte") ? new Color(0.72f, 0.42f, 0.28f) : (currentTargetName.Contains("Venus") ? new Color(0.85f, 0.72f, 0.32f) : new Color(0.36f, 0.68f, 0.98f));
                povCamera.backgroundColor = Color.Lerp(new Color(0.003f, 0.003f, 0.008f), targetSky, tRe);
                povCamera.clearFlags = CameraClearFlags.SolidColor;

                if (cloudWispsPS != null && cloudWispsPS.gameObject.activeSelf)
                {
                    cloudWispsPS.gameObject.SetActive(false);
                }
            }
            // Fase 2: Vacío cósmico puro (espacio exterior)
            else
            {
                povCamera.backgroundColor = new Color(0.003f, 0.003f, 0.008f);
                povCamera.clearFlags = CameraClearFlags.SolidColor;

                if (cloudWispsPS != null && cloudWispsPS.gameObject.activeSelf)
                {
                    cloudWispsPS.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// El horizonte curvo de la Tierra retrocede hacia abajo y atrás durante el ascenso a órbita.
        /// </summary>
        private void UpdateEarthHorizon()
        {
            if (earthHorizonObj == null) return;

            // Visible durante el despegue y alejamiento inicial (0% a 28%)
            bool showEarth = (flightProgress < 0.28f);
            if (earthHorizonObj.activeSelf != showEarth)
            {
                earthHorizonObj.SetActive(showEarth);
            }

            if (showEarth)
            {
                float t = flightProgress / 0.28f;
                // Se aleja verticalmente hacia abajo y hacia atrás
                float targetY = Mathf.Lerp(-14f, -48f, t);
                float targetZ = Mathf.Lerp(12f, 2f, t);
                earthHorizonObj.transform.localPosition = new Vector3(0f, targetY, targetZ);
            }
        }

        /// <summary>
        /// Óptica estelar cinemática de alta fidelidad:
        /// - En atmósfera (<10%): estrellas atenuadas por dispersión solar.
        /// - En espacio (>10%): brillo pleno, blancas y policromáticas sin colapsar.
        /// - Aceleración relativista: efecto headlight (beaming) y corrimiento Doppler cian/ámbar.
        /// </summary>
        private void UpdateStarfieldOptics()
        {
            if (starFieldPS == null || stars == null) return;

            float beta = Mathf.Clamp01((float)velocityFractionC);
            float streamSpeed = isFlying ? (beta > 0.01f ? (5.0f + beta * 40.0f) : 3.2f) : 0f;

            if (activeMode == PropulsionMode.WarpDrive && isFlying)
            {
                streamSpeed = 38.0f;
            }

            // Factor de atenuación atmosférica: estrellas invisibles a nivel del suelo, emergen al salir al espacio
            float atmosphereVisibility = (flightProgress < 0.11f) ? Mathf.Clamp01((flightProgress - 0.035f) / 0.075f) : 1.0f;
            float aberrationConcentration = (beta > 0.02f) ? Mathf.Max(0.38f, 1.0f - (beta * 0.68f)) : 1.0f;

            for (int i = 0; i < MaxStars; i++)
            {
                // Avance continuo en Z con retorno circular suave al fondo
                if (isFlying)
                {
                    starZDepths[i] -= streamSpeed * Time.deltaTime;
                    if (starZDepths[i] < 3.2f)
                    {
                        starZDepths[i] = 52.0f;
                        starBaseCoords[i] = new Vector3(Random.Range(-20f, 20f), Random.Range(-13f, 13f), 0f);
                    }
                }

                // Posición proyectada: calculada de forma pura sin mutar las coordenadas base
                float px = starBaseCoords[i].x * aberrationConcentration;
                float py = starBaseCoords[i].y * aberrationConcentration;
                stars[i].position = new Vector3(px, py, starZDepths[i]);

                // Tamaño y estiramiento relativista
                float streak = (activeMode == PropulsionMode.WarpDrive && isFlying) ? 2.8f : (1.0f + beta * 3.2f);
                stars[i].startSize = 0.22f * streak;

                // Color espectral con atenuación atmosférica y corrimiento Doppler
                Color c = starBaseColors[i];
                if (beta > 0.05f)
                {
                    float dist = Mathf.Sqrt(px * px + py * py);
                    if (dist < 5.5f)
                    {
                        // Blueshift frontal extremo hacia el cian/ultravioleta
                        c = Color.Lerp(c, new Color(0.25f, 0.90f, 1.0f, 1f), beta * 1.5f);
                    }
                    else
                    {
                        // Redshift periférico hacia el ámbar/naranja
                        c = Color.Lerp(c, new Color(1f, 0.35f, 0.12f, 0.75f), beta * 1.3f);
                    }
                }

                // Atenuar con la atmósfera terrestre
                c.a = atmosphereVisibility;
                stars[i].startColor = c;
            }

            starFieldPS.SetParticles(stars, MaxStars);
        }

        private void UpdateCosmicDust()
        {
            if (cosmicDustPS == null) return;

            // Activo sólo en el vacío espacial fuera de la atmósfera
            bool inSpace = isFlying && (flightProgress >= 0.10f);
            if (cosmicDustPS.gameObject.activeSelf != inSpace)
            {
                cosmicDustPS.gameObject.SetActive(inSpace);
            }
        }

        private void UpdateWarpHaloVisual()
        {
            if (warpTorusHaloObj == null) return;

            bool isWarpActive = (activeMode == PropulsionMode.WarpDrive && isFlying);
            if (warpTorusHaloObj.activeSelf != isWarpActive)
            {
                warpTorusHaloObj.SetActive(isWarpActive);
            }

            if (isWarpActive && warpHaloRenderer != null)
            {
                float pulse = 0.22f + 0.14f * Mathf.Sin(Time.time * 6.5f);
                Color c = new Color(0.1f, 0.85f, 1.0f, pulse);
                warpHaloRenderer.material.color = c;
            }
        }

        /// <summary>
        /// El cuerpo astronómico de destino es visible desde el 15% del vuelo y crece majestuosamente
        /// hasta convertirse en un orbe planetario detallado iluminado por la luz del Sol.
        /// </summary>
        private void UpdateProximityPlanetFacsimile()
        {
            if (proximityPlanetObj == null) return;

            // Visible a partir del 15% del trayecto en adelante
            bool shouldShow = (flightProgress >= 0.15f);

            if (proximityPlanetObj.activeSelf != shouldShow)
            {
                proximityPlanetObj.SetActive(shouldShow);
            }

            if (shouldShow)
            {
                float t = Mathf.InverseLerp(0.15f, 1.0f, flightProgress);
                // Crece de un disco visible en el firmamento (0.35) a un orbe dominante (7.2)
                float targetRadius = Mathf.Lerp(0.35f, 7.2f, Mathf.Pow(t, 1.6f));

                proximityPlanetObj.transform.localScale = Vector3.one * targetRadius;
                // Rotación axial suave
                proximityPlanetObj.transform.Rotate(Vector3.up, 3.5f * Time.deltaTime, Space.Self);

                // Posición frontal en el cono visual de la cabina
                float targetZ = Mathf.Lerp(42f, 15.5f, t);
                proximityPlanetObj.transform.localPosition = new Vector3(0f, 0.40f, targetZ);
            }
        }

        private void UpdateCockpitShake()
        {
            if (activeMode == PropulsionMode.WarpDrive)
            {
                CurrentShakeOffset = Vector3.zero;
                return;
            }

            if (!isFlying)
            {
                CurrentShakeOffset = Vector3.Lerp(CurrentShakeOffset, Vector3.zero, Time.deltaTime * 8f);
                return;
            }

            bool targetHasAtmosphere = (currentTargetName.Contains("Tierra") || currentTargetName.Contains("Marte") || currentTargetName.Contains("Venus"));
            float currentShakeLevel = 0f;

            if (flightProgress < 0.11f)
            {
                // FASE 1: Ascenso atmosférico terrestre (Max-Q y turbulencia de aire)
                float tAtm = 1.0f - (flightProgress / 0.11f);
                currentShakeLevel = 0.003f + tAtm * 0.016f;
            }
            else if (flightProgress >= 0.88f && targetHasAtmosphere)
            {
                // FASE 3: Reentrada hipersónica en atmósfera de destino
                float tReentry = (flightProgress - 0.88f) / 0.12f;
                currentShakeLevel = 0.002f + tReentry * 0.014f;
            }
            else
            {
                // FASE 2: Crucero en el vacío espacial (Sin resistencia aerodinámica)
                if (activeMode == PropulsionMode.Chemical)
                {
                    // Leve zumbido mecánico de turbobomba a 34.000 RPM
                    currentShakeLevel = 0.0012f;
                }
                else
                {
                    // Viga de antimateria: silencio inercial suave
                    currentShakeLevel = 0.0f;
                }
            }

            if (currentShakeLevel > 0.0001f)
            {
                shakeIntensity = currentShakeLevel;
                float freq = (flightProgress < 0.11f || (flightProgress >= 0.88f && targetHasAtmosphere)) ? 26f : 45f;
                CurrentShakeOffset = new Vector3(
                    (Mathf.PerlinNoise(Time.time * freq, 0f) - 0.5f) * shakeIntensity,
                    (Mathf.PerlinNoise(0f, Time.time * freq) - 0.5f) * shakeIntensity,
                    0f
                );
            }
            else
            {
                CurrentShakeOffset = Vector3.Lerp(CurrentShakeOffset, Vector3.zero, Time.deltaTime * 6f);
            }
        }

        private static Texture2D CreateSoftGlowTexture(int size)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            Color[] cols = new Color[size * size];
            float center = (size - 1) * 0.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center)) / center;
                    float alpha = Mathf.Clamp01(1.0f - dist);
                    alpha = Mathf.Pow(alpha, 1.8f);
                    cols[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
            }
            tex.SetPixels(cols);
            tex.Apply();
            return tex;
        }
    }
}
