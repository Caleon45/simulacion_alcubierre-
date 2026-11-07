using UnityEngine;

namespace Alcubierre.Cockpit
{
    /// <summary>
    /// Modelado 3D procedural de la Cabina de Propulsión por Antimateria (Orientación Horizontal / Viga Tensora Relativista).
    /// Inspirada en el diseño del Project Valkyrie (Robert Forward / Pellegrino) con blindaje de tungsteno denso (W),
    /// parabrisas angular con filtro polarizado de oro/ámbar y trampa magnética Penning-Malmberg central con núcleo de plasma púrpura.
    /// </summary>
    public class AntimatterCockpit : MonoBehaviour
    {
        [Header("Referencias de Indicadores Dinámicos")]
        public Transform antimatterPlasmaCore;
        public Light plasmaCoreLight;
        public Transform relativisticSpeedNeedle; // Aguja 3D del velocímetro relativista (%c)
        public Transform gMeterNeedle;            // Aguja 3D del acelerómetro de Fuerzas G
        public Transform navScreenMesh;
        public Transform magneticCoilRings;
        public Renderer radiationGeigerLed;

        private Vector3 initialPlasmaScale;

        public void BuildCockpit()
        {
            gameObject.name = "Cockpit_Antimateria_Relativista";
            CockpitMaterialPalette.InitializePalette();

            Material matHull = CockpitMaterialPalette.MatTungstenCarbon;
            Material matConsole = CockpitMaterialPalette.MatConsoleDark;
            Material matAccent = CockpitMaterialPalette.MatInteriorAccent;
            Material matGlass = CockpitMaterialPalette.MatGoldAmberGlass; // Polarizado dorado antirradiación
            Material matPlasma = CockpitMaterialPalette.MatAntimatterPlasma;

            // 1. Mamparos Exteriores Angulares en Cuña (Wedge Shape de Alta Velocidad)
            // Suelo de la cabina
            CreatePrimitive(PrimitiveType.Cube, "Floor", transform, new Vector3(0f, -1.05f, 1.4f), new Vector3(2.8f, 0.15f, 3.4f), matAccent);

            // Mamparo trasero pesado de Tungsteno (Shadow Shield)
            CreatePrimitive(PrimitiveType.Cube, "TungstenShadowBulkhead", transform, new Vector3(0f, 0.35f, -0.3f), new Vector3(3.0f, 2.6f, 0.35f), matHull);

            // Paredes laterales convergentes en cuña aerodinámica
            GameObject leftWall = CreatePrimitive(PrimitiveType.Cube, "LeftWedgeWall", transform, new Vector3(-1.35f, 0.3f, 1.3f), new Vector3(0.18f, 2.5f, 3.2f), matHull);
            leftWall.transform.localRotation = Quaternion.Euler(0f, 12f, 0f);

            GameObject rightWall = CreatePrimitive(PrimitiveType.Cube, "RightWedgeWall", transform, new Vector3(1.35f, 0.3f, 1.3f), new Vector3(0.18f, 2.5f, 3.2f), matHull);
            rightWall.transform.localRotation = Quaternion.Euler(0f, -12f, 0f);

            // Nervaduras de refuerzo de tungsteno y grafito
            CreatePrimitive(PrimitiveType.Cube, "Rib_Wedge_L1", transform, new Vector3(-1.28f, 0.3f, 0.6f), new Vector3(0.12f, 2.3f, 0.12f), matAccent).transform.localRotation = Quaternion.Euler(0f, 12f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "Rib_Wedge_L2", transform, new Vector3(-1.24f, 0.3f, 1.8f), new Vector3(0.12f, 2.3f, 0.12f), matAccent).transform.localRotation = Quaternion.Euler(0f, 12f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "Rib_Wedge_R1", transform, new Vector3(1.28f, 0.3f, 0.6f), new Vector3(0.12f, 2.3f, 0.12f), matAccent).transform.localRotation = Quaternion.Euler(0f, -12f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "Rib_Wedge_R2", transform, new Vector3(1.24f, 0.3f, 1.8f), new Vector3(0.12f, 2.3f, 0.12f), matAccent).transform.localRotation = Quaternion.Euler(0f, -12f, 0f);

            // Conductos superconductores de plasma (tubos curvados a lo largo de las paredes con brillo púrpura)
            CreatePrimitive(PrimitiveType.Cylinder, "Plasma_Conduit_L", transform, new Vector3(-1.18f, 0.95f, 1.4f), new Vector3(0.06f, 1.5f, 0.06f), matPlasma).transform.localRotation = Quaternion.Euler(90f, 12f, 0f);
            CreatePrimitive(PrimitiveType.Cylinder, "Plasma_Conduit_R", transform, new Vector3(1.18f, 0.95f, 1.4f), new Vector3(0.06f, 1.5f, 0.06f), matPlasma).transform.localRotation = Quaternion.Euler(90f, -12f, 0f);

            // Techo bajo y angulado con refuerzos
            GameObject ceiling = CreatePrimitive(PrimitiveType.Cube, "Ceiling", transform, new Vector3(0f, 1.25f, 1.4f), new Vector3(2.4f, 0.15f, 3.0f), matHull);
            ceiling.transform.localRotation = Quaternion.Euler(10f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "Ceiling_Spine", transform, new Vector3(0f, 1.18f, 1.4f), new Vector3(0.20f, 0.08f, 2.8f), matAccent).transform.localRotation = Quaternion.Euler(10f, 0f, 0f);

            // 2. Marcos Frontales Angulados en Cuña (Bloqueo Estructural Gamma sin cristales opacos)
            // Marco frontal angulado con bisel
            CreatePrimitive(PrimitiveType.Cube, "FrontCanopyArch", transform, new Vector3(0f, 0.95f, 2.7f), new Vector3(1.9f, 0.16f, 0.25f), matHull);
            CreatePrimitive(PrimitiveType.Cube, "FrontBottomArch", transform, new Vector3(0f, -0.35f, 2.85f), new Vector3(2.1f, 0.16f, 0.25f), matHull);

            // Visera parasol protectora superior
            GameObject canopyVisor = CreatePrimitive(PrimitiveType.Cube, "Canopy_Visor_Top", transform, new Vector3(0f, 1.05f, 2.75f), new Vector3(2.0f, 0.06f, 0.25f), matConsole);
            canopyVisor.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);

            // Mamparo inferior frontal (cierra el hueco debajo del salpicadero hacia el suelo)
            CreatePrimitive(PrimitiveType.Cube, "FrontLowerBulkhead", transform, new Vector3(0f, -0.78f, 1.95f), new Vector3(2.4f, 0.60f, 0.15f), matHull);

            // 3. Salpicadero Digital Relativista
            GameObject dashBase = CreatePrimitive(PrimitiveType.Cube, "Dashboard_Base", transform, new Vector3(0f, -0.48f, 1.85f), new Vector3(2.2f, 0.55f, 0.95f), matConsole);
            dashBase.transform.localRotation = Quaternion.Euler(22f, 0f, 0f);

            // Pantalla central de Navegación Táctica (HUD Relativista)
            GameObject navScreen = CreatePrimitive(PrimitiveType.Cube, "NavScreen_Display", transform, new Vector3(0f, -0.25f, 1.95f), new Vector3(1.05f, 0.55f, 0.05f), CockpitMaterialPalette.MatScreenEmissive);
            navScreen.transform.localRotation = Quaternion.Euler(22f, 0f, 0f);
            navScreenMesh = navScreen.transform;

            // 4. Trampa de Confinamiento de Antimateria (Penning-Malmberg Trap en la consola izquierda)
            // Pedestal cilíndrico en consola izquierda
            GameObject trapStand = CreatePrimitive(PrimitiveType.Cylinder, "PenningTrap_Stand", transform, new Vector3(-0.95f, -0.55f, 1.55f), new Vector3(0.38f, 0.25f, 0.38f), matConsole);

            // Cilindro de vacío de contención
            GameObject vacuumChamber = CreatePrimitive(PrimitiveType.Cylinder, "Trap_VacuumChamber", trapStand.transform, new Vector3(0f, 1.05f, 0f), new Vector3(0.70f, 0.80f, 0.70f), CockpitMaterialPalette.MatReinforcedGlass);

            // Anillos magnéticos superconductores (YBCO Gold Coils)
            GameObject coilParent = new GameObject("SuperconductingCoils");
            coilParent.transform.SetParent(trapStand.transform, false);
            coilParent.transform.localPosition = new Vector3(0f, 1.05f, 0f);
            magneticCoilRings = coilParent.transform;

            CreatePrimitive(PrimitiveType.Cylinder, "Coil_Top", coilParent.transform, new Vector3(0f, 0.55f, 0f), new Vector3(0.85f, 0.08f, 0.85f), matHull);
            CreatePrimitive(PrimitiveType.Cylinder, "Coil_Mid", coilParent.transform, new Vector3(0f, 0f, 0f), new Vector3(0.90f, 0.08f, 0.90f), matHull);
            CreatePrimitive(PrimitiveType.Cylinder, "Coil_Bot", coilParent.transform, new Vector3(0f, -0.55f, 0f), new Vector3(0.85f, 0.08f, 0.85f), matHull);

            // Esfera de Plasma de Antiprotones (Levitando en el centro magnético)
            GameObject plasmaSphere = CreatePrimitive(PrimitiveType.Sphere, "Antimatter_Plasma_Core", coilParent.transform, Vector3.zero, new Vector3(0.38f, 0.38f, 0.38f), matPlasma);
            antimatterPlasmaCore = plasmaSphere.transform;
            initialPlasmaScale = antimatterPlasmaCore.localScale;

            // Luz puntual interior que emite brillo violeta en la cabina
            GameObject plasmaLightObj = new GameObject("PlasmaCore_PointLight");
            plasmaLightObj.transform.SetParent(plasmaSphere.transform, false);
            plasmaCoreLight = plasmaLightObj.AddComponent<Light>();
            plasmaCoreLight.type = LightType.Point;
            plasmaCoreLight.color = new Color(0.85f, 0.25f, 1.0f);
            plasmaCoreLight.range = 3.2f;
            plasmaCoreLight.intensity = 2.0f;

            // 5. Cluster de Instrumentación Relativista 3D (Velocímetro %c, Acelerómetro G-Meter y Contador Geiger)
            // A) Contador Geiger / Radiómetro Digital
            GameObject geigerHousing = CreatePrimitive(PrimitiveType.Cube, "Geiger_Monitor", transform, new Vector3(-0.45f, -0.22f, 1.85f), new Vector3(0.30f, 0.22f, 0.08f), matConsole);
            geigerHousing.transform.localRotation = Quaternion.Euler(22f, 0f, 0f);
            radiationGeigerLed = CreatePrimitive(PrimitiveType.Sphere, "Geiger_LED", geigerHousing.transform, new Vector3(0f, 0f, 0.05f), new Vector3(0.08f, 0.08f, 0.08f), CockpitMaterialPalette.MatIndicatorGreen).GetComponent<Renderer>();

            // B) Velocímetro Relativista Analógico-Digital (0.0c a 0.60c con aguja luminiscente)
            GameObject relHousing = CreatePrimitive(PrimitiveType.Cylinder, "Rel_Velocimeter_Housing", transform, new Vector3(0.42f, -0.06f, 1.88f), new Vector3(0.24f, 0.04f, 0.24f), matConsole);
            relHousing.transform.localRotation = Quaternion.Euler(68f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cylinder, "Rel_Velocimeter_Face", relHousing.transform, new Vector3(0f, 0.02f, 0f), new Vector3(0.90f, 0.01f, 0.90f), matAccent);
            GameObject relNeedleObj = CreatePrimitive(PrimitiveType.Cube, "Needle_RelSpeed", relHousing.transform, new Vector3(0f, 0.04f, 0.05f), new Vector3(0.016f, 0.015f, 0.13f), CockpitMaterialPalette.MatCasimirWarpCore);
            relativisticSpeedNeedle = relNeedleObj.transform;

            // C) Acelerómetro de Inercia Relativista (0 - 4g con aguja)
            GameObject gHousing = CreatePrimitive(PrimitiveType.Cylinder, "Antimatter_GMeter_Housing", transform, new Vector3(0.72f, -0.06f, 1.88f), new Vector3(0.22f, 0.04f, 0.22f), matConsole);
            gHousing.transform.localRotation = Quaternion.Euler(68f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cylinder, "GMeter_Face", gHousing.transform, new Vector3(0f, 0.02f, 0f), new Vector3(0.88f, 0.01f, 0.88f), matAccent);
            GameObject gNeedleObj = CreatePrimitive(PrimitiveType.Cube, "Needle_G", gHousing.transform, new Vector3(0f, 0.04f, 0.05f), new Vector3(0.018f, 0.015f, 0.12f), CockpitMaterialPalette.MatIndicatorRed);
            gMeterNeedle = gNeedleObj.transform;

            // 6. Monitor de Escudo Deflector ISM de Berilio (Consola derecha)
            GameObject ismDisplay = CreatePrimitive(PrimitiveType.Cube, "ISM_Deflector_Monitor", transform, new Vector3(1.02f, -0.22f, 1.85f), new Vector3(0.32f, 0.25f, 0.08f), matConsole);
            ismDisplay.transform.localRotation = Quaternion.Euler(22f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "ISM_ShieldStatus_LED", ismDisplay.transform, new Vector3(0f, 0.06f, 0.05f), new Vector3(0.12f, 0.04f, 0.04f), CockpitMaterialPalette.MatIndicatorGreen);

            // 7. Asiento Ergonómico de Alta-G (Ubicado bajo la cámara POV)
            CreatePrimitive(PrimitiveType.Cube, "PilotSeat_Cushion", transform, new Vector3(0f, -0.42f, 0.38f), new Vector3(0.62f, 0.14f, 0.65f), matAccent);
            GameObject backrest = CreatePrimitive(PrimitiveType.Cube, "PilotSeat_Backrest", transform, new Vector3(0f, -0.05f, 0.12f), new Vector3(0.55f, 0.72f, 0.14f), matAccent);
            backrest.transform.localRotation = Quaternion.Euler(-15f, 0f, 0f);
            GameObject headrest = CreatePrimitive(PrimitiveType.Cube, "PilotSeat_Headrest", transform, new Vector3(0f, 0.38f, 0.02f), new Vector3(0.34f, 0.22f, 0.12f), matAccent);
            headrest.transform.localRotation = Quaternion.Euler(-15f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "PilotSeat_ArmrestLeft", transform, new Vector3(-0.36f, -0.22f, 0.36f), new Vector3(0.12f, 0.08f, 0.48f), matConsole);
            CreatePrimitive(PrimitiveType.Cube, "PilotSeat_ArmrestRight", transform, new Vector3(0.36f, -0.22f, 0.36f), new Vector3(0.12f, 0.08f, 0.48f), matConsole);
            CreatePrimitive(PrimitiveType.Cylinder, "PilotSeat_Pedestal", transform, new Vector3(0f, -0.75f, 0.38f), new Vector3(0.24f, 0.30f, 0.24f), matConsole);

            // Controles integrados en reposabrazos (HOTAS)
            CreatePrimitive(PrimitiveType.Cylinder, "LeftArmrest_ThrottleStick", transform, new Vector3(-0.36f, -0.14f, 0.42f), new Vector3(0.04f, 0.12f, 0.04f), matAccent);
            CreatePrimitive(PrimitiveType.Cylinder, "RightArmrest_FlightStick", transform, new Vector3(0.36f, -0.14f, 0.42f), new Vector3(0.04f, 0.12f, 0.04f), matAccent);

            // 8. Iluminación Interior de Cabina (Luz técnica azulada-blanca)
            GameObject cabinLightObj = new GameObject("Cabin_Interior_Light");
            cabinLightObj.transform.SetParent(transform, false);
            cabinLightObj.transform.localPosition = new Vector3(0f, 0.90f, 1.1f);
            Light cabinLight = cabinLightObj.AddComponent<Light>();
            cabinLight.type = LightType.Point;
            cabinLight.color = new Color(0.85f, 0.92f, 1.0f);
            cabinLight.range = 5.5f;
            cabinLight.intensity = 2.6f;
        }

        public void UpdateTelemetryVisuals(float fuelRatio, float velocityFractionC, float feltGForce, double radiationDose_mSv, bool isWarning, bool isCritical)
        {
            fuelRatio = Mathf.Clamp01(fuelRatio);

            // El núcleo de plasma de antimateria pulsa suavemente y reduce su volumen conforme se gasta el combustible
            if (antimatterPlasmaCore != null)
            {
                float pulse = 1.0f + 0.12f * Mathf.Sin(Time.time * 6.0f);
                antimatterPlasmaCore.localScale = initialPlasmaScale * (fuelRatio * pulse);
            }

            // La luz interior del reactor reacciona a la potencia
            if (plasmaCoreLight != null)
            {
                float baseIntensity = fuelRatio > 0.01f ? (1.5f + velocityFractionC * 2.0f) : 0f;
                plasmaCoreLight.intensity = baseIntensity;
            }

            // Aguja física del velocímetro relativista: de 0° (0.0c) a -210° (0.60c)
            if (relativisticSpeedNeedle != null)
            {
                float spdAngle = Mathf.Lerp(0f, -210f, Mathf.Clamp01(velocityFractionC / 0.60f));
                relativisticSpeedNeedle.localRotation = Quaternion.Euler(0f, spdAngle, 0f);
            }

            // Aguja física del acelerómetro G-Meter: Rota de 0° a -135° (0 a 3.5g)
            if (gMeterNeedle != null)
            {
                float gAngle = Mathf.Lerp(0f, -135f, Mathf.Clamp01(feltGForce / 3.5f));
                gMeterNeedle.localRotation = Quaternion.Euler(0f, gAngle, 0f);
            }

            // Los anillos magnéticos giran sutilmente simulando confinamiento toroidal
            if (magneticCoilRings != null && fuelRatio > 0f)
            {
                magneticCoilRings.Rotate(0f, 40f * Time.deltaTime, 0f);
            }

            // LED del contador Geiger: Verde nominal, amarillo si > 0.25 Sv, rojo crítico si > 1.0 Sv
            if (radiationGeigerLed != null)
            {
                bool blink = (Mathf.Sin(Time.time * 8.0f) > 0f);
                if (isCritical || radiationDose_mSv > 1000.0)
                {
                    radiationGeigerLed.material = blink ? CockpitMaterialPalette.MatIndicatorRed : CockpitMaterialPalette.MatConsoleDark;
                }
                else if (isWarning || radiationDose_mSv > 250.0)
                {
                    radiationGeigerLed.material = blink ? CockpitMaterialPalette.MatIndicatorYellow : CockpitMaterialPalette.MatIndicatorGreen;
                }
                else
                {
                    radiationGeigerLed.material = CockpitMaterialPalette.MatIndicatorGreen;
                }
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

