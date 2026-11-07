using UnityEngine;

namespace Alcubierre.Cockpit
{
    /// <summary>
    /// Modelado 3D procedural de la Cabina de Curvatura Warp (Orientación Horizontal / Plataforma Inercial Cero-G).
    /// Inspirada en el diseño de la IXS Enterprise (Dr. Harold White / Mark Rademaker) con aleación de titanio blanco perla,
    /// ventanal panorámico semicircular continuo, resonador central de energía exótica (Efecto Casimir Dinámico)
    /// y monitor inercial fijado permanentemente en 0.00g (caída libre geodésica pura).
    /// </summary>
    public class WarpCockpit : MonoBehaviour
    {
        [Header("Referencias de Indicadores Dinámicos")]
        public Transform casimirResonatorCore;
        public Light warpCoreLight;
        public Transform metricHoloProjector;
        public Transform navScreenMesh;
        public Transform gZeroDialMesh;

        private Vector3 initialResonatorScale;

        public void BuildCockpit()
        {
            gameObject.name = "Cockpit_Alcubierre_Warp";
            CockpitMaterialPalette.InitializePalette();

            Material matHull = CockpitMaterialPalette.MatPearlTitanium;
            Material matConsole = CockpitMaterialPalette.MatConsoleDark;
            Material matAccent = CockpitMaterialPalette.MatInteriorAccent;
            Material matGlass = CockpitMaterialPalette.MatPanoramicGlass; // Cristal antirreflectante ultra-claro
            Material matWarpCore = CockpitMaterialPalette.MatCasimirWarpCore;

            // 1. Mamparos Curvos y Plataforma Inercial (Estilo Futurista Ergonómico NASA)
            // Suelo circular / elíptico con anillo de resonancia cuántica
            CreatePrimitive(PrimitiveType.Cylinder, "Floor_Platform", transform, new Vector3(0f, -1.05f, 1.35f), new Vector3(3.2f, 0.12f, 3.2f), matAccent);
            CreatePrimitive(PrimitiveType.Cylinder, "Floor_QuantumResonanceRing", transform, new Vector3(0f, -0.98f, 1.35f), new Vector3(2.8f, 0.02f, 2.8f), matWarpCore);

            // Mamparo trasero curvado con molduras perla
            CreatePrimitive(PrimitiveType.Cylinder, "RearCurvedWall", transform, new Vector3(0f, 0.4f, -0.4f), new Vector3(3.3f, 1.25f, 1.5f), matHull);

            // Pilares de arco aerodinámicos tridimensionales
            CreatePrimitive(PrimitiveType.Cube, "LeftArchPillar", transform, new Vector3(-1.6f, 0.35f, 1.4f), new Vector3(0.18f, 2.4f, 2.4f), matHull);
            CreatePrimitive(PrimitiveType.Cube, "RightArchPillar", transform, new Vector3(1.6f, 0.35f, 1.4f), new Vector3(0.18f, 2.4f, 2.4f), matHull);
            CreatePrimitive(PrimitiveType.Cube, "LeftRib_Inner", transform, new Vector3(-1.45f, 0.4f, 1.4f), new Vector3(0.10f, 2.2f, 0.16f), matAccent);
            CreatePrimitive(PrimitiveType.Cube, "RightRib_Inner", transform, new Vector3(1.45f, 0.4f, 1.4f), new Vector3(0.10f, 2.2f, 0.16f), matAccent);

            // Techo de cúpula abierta con halo superior
            GameObject roof = CreatePrimitive(PrimitiveType.Cylinder, "Roof_Dome", transform, new Vector3(0f, 1.4f, 1.35f), new Vector3(3.1f, 0.08f, 3.1f), matHull);
            roof.transform.localRotation = Quaternion.Euler(5f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cylinder, "Roof_QuantumHalo", transform, new Vector3(0f, 1.34f, 1.35f), new Vector3(2.2f, 0.02f, 2.2f), matWarpCore).transform.localRotation = Quaternion.Euler(5f, 0f, 0f);

            // 2. Ventanal Panorámico Semicircular Continuo (Máxima Visibilidad de Curvatura sin cristales opacos)
            // Marco inferior curvo
            CreatePrimitive(PrimitiveType.Cylinder, "Canopy_BaseRing", transform, new Vector3(0f, -0.42f, 2.5f), new Vector3(2.8f, 0.12f, 1.2f), matHull);
            // Marco superior curvo
            CreatePrimitive(PrimitiveType.Cylinder, "Canopy_TopRing", transform, new Vector3(0f, 1.15f, 2.4f), new Vector3(2.7f, 0.12f, 1.1f), matHull);

            // Arcos de refuerzo del dosel panorámico
            CreatePrimitive(PrimitiveType.Cube, "Canopy_Rib_Left", transform, new Vector3(-0.85f, 0.40f, 2.55f), new Vector3(0.08f, 1.4f, 0.10f), matHull).transform.localRotation = Quaternion.Euler(-14f, 15f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "Canopy_Rib_Right", transform, new Vector3(0.85f, 0.40f, 2.55f), new Vector3(0.08f, 1.4f, 0.10f), matHull).transform.localRotation = Quaternion.Euler(-14f, -15f, 0f);

            // Mamparo inferior frontal (cierra el hueco debajo de la consola hacia la plataforma)
            CreatePrimitive(PrimitiveType.Cube, "FrontLowerBulkhead", transform, new Vector3(0f, -0.80f, 1.85f), new Vector3(2.6f, 0.60f, 0.15f), matHull);

            // 3. Consola Central Flotante y Holográfica
            GameObject consoleBase = CreatePrimitive(PrimitiveType.Cube, "FloatingConsole_Base", transform, new Vector3(0f, -0.55f, 1.6f), new Vector3(2.4f, 0.45f, 0.85f), matConsole);
            consoleBase.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);

            // Pantalla Holográfica Central de Navegación Táctica
            GameObject navScreen = CreatePrimitive(PrimitiveType.Cube, "NavScreen_Display", transform, new Vector3(0f, -0.28f, 1.72f), new Vector3(1.15f, 0.65f, 0.04f), CockpitMaterialPalette.MatScreenEmissive);
            navScreen.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
            navScreenMesh = navScreen.transform;

            // 4. Resonador de Energía Exótica (Efecto Casimir Dinámico - rho < 0 en la consola izquierda)
            // Pedestal toroidal flotante en consola izquierda
            GameObject coreStand = CreatePrimitive(PrimitiveType.Cylinder, "CasimirStand", transform, new Vector3(-0.95f, -0.55f, 1.50f), new Vector3(0.42f, 0.20f, 0.42f), matConsole);

            // Anillo exterior de resonancia armónica
            CreatePrimitive(PrimitiveType.Cylinder, "ResonatorRing_Outer", coreStand.transform, new Vector3(0f, 0.95f, 0f), new Vector3(0.75f, 0.08f, 0.75f), matHull);

            // Núcleo Esférico de Energía Exótica (Flotando y emitiendo luz cian)
            GameObject coreSphere = CreatePrimitive(PrimitiveType.Sphere, "Casimir_Energy_Core", coreStand.transform, new Vector3(0f, 0.95f, 0f), new Vector3(0.38f, 0.38f, 0.38f), matWarpCore);
            casimirResonatorCore = coreSphere.transform;
            initialResonatorScale = casimirResonatorCore.localScale;

            // Luz cian puntual de ambientación interior
            GameObject warpLightObj = new GameObject("WarpCore_PointLight");
            warpLightObj.transform.SetParent(coreSphere.transform, false);
            warpCoreLight = warpLightObj.AddComponent<Light>();
            warpCoreLight.type = LightType.Point;
            warpCoreLight.color = new Color(0.15f, 0.85f, 1.0f);
            warpCoreLight.range = 3.5f;
            warpCoreLight.intensity = 2.2f;

            // 5. Proyector Holográfico Volumétrico de la Métrica de Alcubierre f(rs)
            GameObject holoBase = CreatePrimitive(PrimitiveType.Cylinder, "HoloProjector_Base", transform, new Vector3(-0.48f, -0.26f, 1.62f), new Vector3(0.24f, 0.05f, 0.24f), matConsole);
            holoBase.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);

            GameObject holoMesh = CreatePrimitive(PrimitiveType.Sphere, "Holo_Metric_Bubble", holoBase.transform, new Vector3(0f, 1.4f, 0f), new Vector3(0.26f, 0.16f, 0.26f), matWarpCore);
            metricHoloProjector = holoMesh.transform;

            // 6. Monitor Inercial Cero-G (0.00g Absoluto) y Reloj Dual Cuántico en consola derecha
            GameObject gZeroDisplay = CreatePrimitive(PrimitiveType.Cube, "GZero_Dial", transform, new Vector3(0.85f, -0.26f, 1.62f), new Vector3(0.35f, 0.22f, 0.04f), matConsole);
            gZeroDisplay.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
            gZeroDialMesh = gZeroDisplay.transform;

            // LED verde de confirmación geodésica
            CreatePrimitive(PrimitiveType.Sphere, "Geodesic_Confirmation_LED", gZeroDisplay.transform, new Vector3(0f, 0.05f, 0.03f), new Vector3(0.06f, 0.06f, 0.06f), CockpitMaterialPalette.MatIndicatorGreen);

            // Cronómetro Dual Cuántico (Tierra vs Nave: d_tau = dt)
            GameObject clockDisplay = CreatePrimitive(PrimitiveType.Cube, "DualQuantumClock_Monitor", transform, new Vector3(0.48f, -0.26f, 1.62f), new Vector3(0.26f, 0.18f, 0.04f), matConsole);
            clockDisplay.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);

            // 7. Sillón de Mando Ergonómico Futurista (Ubicado bajo la cámara POV)
            CreatePrimitive(PrimitiveType.Cube, "PilotSeat_Cushion", transform, new Vector3(0f, -0.42f, 0.38f), new Vector3(0.64f, 0.14f, 0.65f), matHull);
            GameObject backrest = CreatePrimitive(PrimitiveType.Cube, "PilotSeat_Backrest", transform, new Vector3(0f, -0.05f, 0.12f), new Vector3(0.56f, 0.72f, 0.14f), matHull);
            backrest.transform.localRotation = Quaternion.Euler(-14f, 0f, 0f);
            GameObject headrest = CreatePrimitive(PrimitiveType.Cube, "PilotSeat_Headrest", transform, new Vector3(0f, 0.38f, 0.03f), new Vector3(0.34f, 0.22f, 0.12f), matHull);
            headrest.transform.localRotation = Quaternion.Euler(-14f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "PilotSeat_ArmrestLeft", transform, new Vector3(-0.36f, -0.22f, 0.36f), new Vector3(0.12f, 0.08f, 0.48f), matConsole);
            CreatePrimitive(PrimitiveType.Cube, "PilotSeat_ArmrestRight", transform, new Vector3(0.36f, -0.22f, 0.36f), new Vector3(0.12f, 0.08f, 0.48f), matConsole);
            CreatePrimitive(PrimitiveType.Cylinder, "PilotSeat_Pedestal", transform, new Vector3(0f, -0.75f, 0.38f), new Vector3(0.24f, 0.30f, 0.24f), matConsole);

            // Controles de curvatura táctiles en los reposabrazos
            CreatePrimitive(PrimitiveType.Cube, "LeftArmrest_WarpSlider", transform, new Vector3(-0.36f, -0.17f, 0.42f), new Vector3(0.06f, 0.03f, 0.14f), CockpitMaterialPalette.MatIndicatorGreen);
            CreatePrimitive(PrimitiveType.Cube, "RightArmrest_VectorTrackball", transform, new Vector3(0.36f, -0.17f, 0.42f), new Vector3(0.06f, 0.03f, 0.14f), CockpitMaterialPalette.MatIndicatorYellow);

            // 8. Iluminación Interior de Cabina (Luz blanca perla futurista)
            GameObject cabinLightObj = new GameObject("Cabin_Interior_Light");
            cabinLightObj.transform.SetParent(transform, false);
            cabinLightObj.transform.localPosition = new Vector3(0f, 0.95f, 1.1f);
            Light cabinLight = cabinLightObj.AddComponent<Light>();
            cabinLight.type = LightType.Point;
            cabinLight.color = new Color(0.92f, 0.96f, 1.0f);
            cabinLight.range = 6.0f;
            cabinLight.intensity = 2.8f;
        }

        public void UpdateTelemetryVisuals(float warpFactor, bool isWarpActive, bool isWarning = false, bool isCritical = false)
        {
            // El núcleo de energía exótica pulsa en frecuencia armónica con el factor Warp W (10c - 100c)
            if (casimirResonatorCore != null)
            {
                float freq = isWarpActive ? Mathf.Max(6f, warpFactor * 0.8f) : 2.5f;
                if (isCritical) freq = 24.0f; // Pulso caótico en estado crítico
                float pulse = 1.0f + 0.15f * Mathf.Sin(Time.time * freq);
                casimirResonatorCore.localScale = initialResonatorScale * pulse;
            }

            // Luz ambiental cian reactiva al campo de curvatura
            if (warpCoreLight != null)
            {
                if (isCritical)
                {
                    bool blink = Mathf.Sin(Time.time * 12f) > 0f;
                    warpCoreLight.color = blink ? Color.red : new Color(0.15f, 0.85f, 1.0f);
                    warpCoreLight.intensity = blink ? 3.5f : 0.8f;
                }
                else
                {
                    warpCoreLight.color = new Color(0.15f, 0.85f, 1.0f);
                    warpCoreLight.intensity = isWarpActive ? (2.2f + Mathf.PingPong(Time.time * 4f, 1.2f)) : 1.2f;
                }
            }

            // El holograma de la burbuja rota suavemente sobre su eje de curvatura
            if (metricHoloProjector != null)
            {
                float rotSpeed = isCritical ? 180f : 30f;
                metricHoloProjector.Rotate(0f, rotSpeed * Time.deltaTime, 0f);
                float deformPulse = 1.0f + 0.10f * Mathf.Sin(Time.time * 5f);
                metricHoloProjector.localScale = new Vector3(0.30f * deformPulse, 0.18f, 0.30f * (2f - deformPulse));
            }
        }

        private static GameObject CreatePrimitive(PrimitiveType type, string name, Transform parent, Vector3 localPos, Vector3 localScale, Material mat)
        {
            GameObject obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = localScale;

            Collider col = obj.GetComponent<Collider>();
            if (col != null) DestroyImmediate(col);

            Renderer ren = obj.GetComponent<Renderer>();
            if (ren != null && mat != null)
            {
                ren.material = mat;
            }
            return obj;
        }
    }
}

