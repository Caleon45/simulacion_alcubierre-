using UnityEngine;

namespace Alcubierre.Cockpit
{
    /// <summary>
    /// Modelado 3D procedural de la Cabina de Propulsión Química (Orientación Vertical / Cápsula Industrial).
    /// Inspirada en la arquitectura de las cápsulas Apollo, Orion y Starship con aleación de Aluminio-Litio (Al-Li 2195),
    /// dos ventanillas reforzadas, diales analógicos y tanques criogénicos visibles de LOX y LH2.
    /// </summary>
    public class ChemicalCockpit : MonoBehaviour
    {
        [Header("Referencias de Indicadores Dinámicos")]
        public Transform loxFuelLevelBar;
        public Transform lh2FuelLevelBar;
        public Transform velocimeterNeedle;      // Aguja 3D del velocímetro orbital (0 a 40 km/s)
        public Transform gMeterNeedle;            // Aguja 3D del acelerómetro de Fuerzas G
        public Transform o2PressureNeedle;        // Aguja 3D de presión de cabina y soporte vital O2
        public Transform throttleLever;
        public Transform navScreenMesh;
        public Renderer[] statusLeds;

        private Vector3 initialLoxScale;
        private Vector3 initialLh2Scale;

        public void BuildCockpit()
        {
            gameObject.name = "Cockpit_Quimica_Industrial";
            CockpitMaterialPalette.InitializePalette();

            Material matHull = CockpitMaterialPalette.MatAluminumLithium;
            Material matConsole = CockpitMaterialPalette.MatConsoleDark;
            Material matAccent = CockpitMaterialPalette.MatInteriorAccent;
            Material matGlass = CockpitMaterialPalette.MatReinforcedGlass;

            // 1. Mamparos Exteriores y Techo Cónico de la Cápsula
            // Suelo de la cabina
            CreatePrimitive(PrimitiveType.Cube, "Floor", transform, new Vector3(0f, -1.1f, 1.2f), new Vector3(3.2f, 0.15f, 3.0f), matAccent);

            // Pared trasera
            CreatePrimitive(PrimitiveType.Cube, "RearBulkhead", transform, new Vector3(0f, 0.2f, -0.2f), new Vector3(3.2f, 2.5f, 0.15f), matHull);

            // Paredes laterales anguladas (cápsula trapezoidal)
            GameObject leftWall = CreatePrimitive(PrimitiveType.Cube, "LeftWall", transform, new Vector3(-1.55f, 0.2f, 1.2f), new Vector3(0.15f, 2.5f, 2.9f), matHull);
            leftWall.transform.localRotation = Quaternion.Euler(0f, 6f, 0f);

            GameObject rightWall = CreatePrimitive(PrimitiveType.Cube, "RightWall", transform, new Vector3(1.55f, 0.2f, 1.2f), new Vector3(0.15f, 2.5f, 2.9f), matHull);
            rightWall.transform.localRotation = Quaternion.Euler(0f, -6f, 0f);

            // Nervaduras estructurales tridimensionales (Truss Ribs) en paredes y techo para romper el aspecto plano
            CreatePrimitive(PrimitiveType.Cube, "Rib_Left_01", transform, new Vector3(-1.48f, 0.25f, 0.5f), new Vector3(0.12f, 2.3f, 0.14f), matAccent).transform.localRotation = Quaternion.Euler(0f, 6f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "Rib_Left_02", transform, new Vector3(-1.46f, 0.25f, 1.7f), new Vector3(0.12f, 2.3f, 0.14f), matAccent).transform.localRotation = Quaternion.Euler(0f, 6f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "Rib_Right_01", transform, new Vector3(1.48f, 0.25f, 0.5f), new Vector3(0.12f, 2.3f, 0.14f), matAccent).transform.localRotation = Quaternion.Euler(0f, -6f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "Rib_Right_02", transform, new Vector3(1.46f, 0.25f, 1.7f), new Vector3(0.12f, 2.3f, 0.14f), matAccent).transform.localRotation = Quaternion.Euler(0f, -6f, 0f);

            // Techo inclinado con vigas longitudinales
            GameObject ceiling = CreatePrimitive(PrimitiveType.Cube, "Ceiling", transform, new Vector3(0f, 1.35f, 1.3f), new Vector3(3.0f, 0.15f, 2.7f), matHull);
            ceiling.transform.localRotation = Quaternion.Euler(8f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "Ceiling_Rib_Front", transform, new Vector3(0f, 1.28f, 1.8f), new Vector3(2.6f, 0.10f, 0.12f), matAccent).transform.localRotation = Quaternion.Euler(8f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "Ceiling_Rib_Rear", transform, new Vector3(0f, 1.31f, 0.8f), new Vector3(2.8f, 0.10f, 0.12f), matAccent).transform.localRotation = Quaternion.Euler(8f, 0f, 0f);

            // Panel Cenital de Disyuntores e Interruptores (Overhead Switch Console)
            GameObject overheadConsole = CreatePrimitive(PrimitiveType.Cube, "Overhead_Switch_Panel", transform, new Vector3(0f, 1.22f, 1.3f), new Vector3(1.10f, 0.12f, 0.95f), matConsole);
            overheadConsole.transform.localRotation = Quaternion.Euler(8f, 0f, 0f);
            for (int sw = -2; sw <= 2; sw++)
            {
                CreatePrimitive(PrimitiveType.Cube, $"Toggle_Sw_{sw}", overheadConsole.transform, new Vector3(sw * 0.18f, -0.07f, 0.15f), new Vector3(0.04f, 0.08f, 0.04f), CockpitMaterialPalette.MatIndicatorYellow).transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
                CreatePrimitive(PrimitiveType.Cube, $"Breaker_{sw}", overheadConsole.transform, new Vector3(sw * 0.18f, -0.07f, -0.15f), new Vector3(0.04f, 0.05f, 0.04f), CockpitMaterialPalette.MatIndicatorGreen);
            }

            // 2. Marcos Estructurales del Parabrisas Frontal (Heavy Framing sin cristales opacos que bloqueen el cosmos)
            // Poste central con remaches y relieve
            CreatePrimitive(PrimitiveType.Cube, "CenterPost", transform, new Vector3(0f, 0.35f, 2.5f), new Vector3(0.18f, 1.6f, 0.2f), matHull);
            CreatePrimitive(PrimitiveType.Cube, "CenterPost_GripTrim", transform, new Vector3(0f, 0.35f, 2.42f), new Vector3(0.08f, 1.5f, 0.06f), matAccent);
            // Marco inferior
            CreatePrimitive(PrimitiveType.Cube, "BottomFrame", transform, new Vector3(0f, -0.45f, 2.5f), new Vector3(2.8f, 0.15f, 0.25f), matHull);
            // Marco superior
            CreatePrimitive(PrimitiveType.Cube, "TopFrame", transform, new Vector3(0f, 1.15f, 2.45f), new Vector3(2.6f, 0.18f, 0.25f), matHull);

            // Visera / Capucha Antirreflejos sobre el Salpicadero
            GameObject glareShield = CreatePrimitive(PrimitiveType.Cube, "GlareShield_Hood", transform, new Vector3(0f, -0.15f, 2.15f), new Vector3(2.65f, 0.06f, 0.35f), matConsole);
            glareShield.transform.localRotation = Quaternion.Euler(14f, 0f, 0f);

            // Mamparo inferior frontal (cierra el hueco debajo del salpicadero hacia el suelo)
            CreatePrimitive(PrimitiveType.Cube, "FrontLowerBulkhead", transform, new Vector3(0f, -0.82f, 1.85f), new Vector3(2.8f, 0.65f, 0.15f), matHull);

            // 3. Salpicadero y Consola Central Inclinada
            GameObject dashBase = CreatePrimitive(PrimitiveType.Cube, "Dashboard_Base", transform, new Vector3(0f, -0.52f, 1.7f), new Vector3(2.6f, 0.65f, 0.85f), matConsole);
            dashBase.transform.localRotation = Quaternion.Euler(18f, 0f, 0f);

            // Monitor central de Navegación Táctica (Superficie con HUD digital procedural y bisel protector)
            CreatePrimitive(PrimitiveType.Cube, "NavScreen_Bezel", transform, new Vector3(0f, -0.32f, 1.73f), new Vector3(1.18f, 0.72f, 0.04f), matAccent).transform.localRotation = Quaternion.Euler(18f, 0f, 0f);
            GameObject navScreen = CreatePrimitive(PrimitiveType.Cube, "NavScreen_Display", transform, new Vector3(0f, -0.32f, 1.76f), new Vector3(1.1f, 0.65f, 0.02f), CockpitMaterialPalette.MatScreenEmissive);
            navScreen.transform.localRotation = Quaternion.Euler(18f, 0f, 0f);
            navScreenMesh = navScreen.transform;

            // 4. Tanques Criogénicos de Combustible (LOX y LH2)
            // Tanque Izquierdo: Hidrógeno Líquido (LH2 - Azul eléctrico)
            GameObject lh2Housing = CreatePrimitive(PrimitiveType.Cylinder, "Tank_LH2_Housing", transform, new Vector3(-1.05f, -0.35f, 1.6f), new Vector3(0.24f, 0.40f, 0.24f), matAccent);
            GameObject lh2Liquid = CreatePrimitive(PrimitiveType.Cylinder, "Tank_LH2_LiquidLevel", lh2Housing.transform, new Vector3(0f, 0f, 0f), new Vector3(0.85f, 0.85f, 0.85f), CockpitMaterialPalette.MatLH2Fuel);
            lh2FuelLevelBar = lh2Liquid.transform;
            initialLh2Scale = lh2FuelLevelBar.localScale;

            // Tanque Derecho: Oxígeno Líquido (LOX - Turquesa criogénico)
            GameObject loxHousing = CreatePrimitive(PrimitiveType.Cylinder, "Tank_LOX_Housing", transform, new Vector3(1.05f, -0.35f, 1.6f), new Vector3(0.24f, 0.40f, 0.24f), matAccent);
            GameObject loxLiquid = CreatePrimitive(PrimitiveType.Cylinder, "Tank_LOX_LiquidLevel", loxHousing.transform, new Vector3(0f, 0f, 0f), new Vector3(0.85f, 0.85f, 0.85f), CockpitMaterialPalette.MatLOXFuel);
            loxFuelLevelBar = loxLiquid.transform;
            initialLoxScale = loxFuelLevelBar.localScale;

            // Manómetros analógicos redondos sobre los tanques
            CreatePrimitive(PrimitiveType.Cylinder, "Gauge_LH2_Pressure", transform, new Vector3(-1.05f, 0.12f, 1.55f), new Vector3(0.18f, 0.04f, 0.18f), matConsole).transform.localRotation = Quaternion.Euler(75f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cylinder, "Gauge_LOX_Pressure", transform, new Vector3(1.05f, 0.12f, 1.55f), new Vector3(0.18f, 0.04f, 0.18f), matConsole).transform.localRotation = Quaternion.Euler(75f, 0f, 0f);

            // 5. Cluster de Instrumentación Analógica 3D (Velocímetro, Acelerómetro G-Meter y Manómetro O2)
            // A) Velocímetro Orbital Físico (0 - 40 km/s con aguja 3D)
            GameObject spdHousing = CreatePrimitive(PrimitiveType.Cylinder, "Velocimeter_Housing", transform, new Vector3(-0.75f, 0.05f, 1.65f), new Vector3(0.24f, 0.04f, 0.24f), matConsole);
            spdHousing.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cylinder, "Velocimeter_Face", spdHousing.transform, new Vector3(0f, 0.02f, 0f), new Vector3(0.90f, 0.01f, 0.90f), matAccent);
            GameObject spdNeedleObj = CreatePrimitive(PrimitiveType.Cube, "Needle_Speed", spdHousing.transform, new Vector3(0f, 0.04f, 0.05f), new Vector3(0.016f, 0.015f, 0.13f), CockpitMaterialPalette.MatIndicatorYellow);
            velocimeterNeedle = spdNeedleObj.transform;

            // B) Acelerómetro Mecánico de Fuerzas G (0 - 6g con aguja 3D)
            GameObject gMeterHousing = CreatePrimitive(PrimitiveType.Cylinder, "GMeter_Housing", transform, new Vector3(-0.45f, 0.05f, 1.65f), new Vector3(0.22f, 0.04f, 0.22f), matConsole);
            gMeterHousing.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cylinder, "GMeter_Face", gMeterHousing.transform, new Vector3(0f, 0.02f, 0f), new Vector3(0.88f, 0.01f, 0.88f), matAccent);
            GameObject needleObj = CreatePrimitive(PrimitiveType.Cube, "Needle_G", gMeterHousing.transform, new Vector3(0f, 0.04f, 0.05f), new Vector3(0.018f, 0.015f, 0.12f), CockpitMaterialPalette.MatIndicatorRed);
            gMeterNeedle = needleObj.transform;

            // C) Manómetro de Soporte Vital y Atmósfera de Cabina (Presión O2 a 101.3 kPa)
            GameObject o2Housing = CreatePrimitive(PrimitiveType.Cylinder, "O2_Pressure_Housing", transform, new Vector3(-0.16f, 0.05f, 1.65f), new Vector3(0.20f, 0.04f, 0.20f), matConsole);
            o2Housing.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cylinder, "O2_Face", o2Housing.transform, new Vector3(0f, 0.02f, 0f), new Vector3(0.88f, 0.01f, 0.88f), matAccent);
            GameObject o2NeedleObj = CreatePrimitive(PrimitiveType.Cube, "Needle_O2", o2Housing.transform, new Vector3(0f, 0.04f, 0.05f), new Vector3(0.016f, 0.015f, 0.11f), CockpitMaterialPalette.MatIndicatorGreen);
            o2PressureNeedle = o2NeedleObj.transform;

            // D) Manómetro de temperatura de cámara de combustión (3.500 K)
            CreatePrimitive(PrimitiveType.Cylinder, "Gauge_ChamberTemp", transform, new Vector3(0.14f, 0.05f, 1.65f), new Vector3(0.18f, 0.04f, 0.18f), matConsole).transform.localRotation = Quaternion.Euler(65f, 0f, 0f);

            // 6. Palanca de Empuje / Throttle e Interruptores Físicos
            GameObject throttlePedestal = CreatePrimitive(PrimitiveType.Cube, "Throttle_Pedestal", transform, new Vector3(0.45f, -0.65f, 1.35f), new Vector3(0.18f, 0.25f, 0.35f), matConsole);
            GameObject leverHandle = CreatePrimitive(PrimitiveType.Cylinder, "Throttle_Lever", throttlePedestal.transform, new Vector3(0f, 0.18f, 0f), new Vector3(0.04f, 0.18f, 0.04f), CockpitMaterialPalette.MatIndicatorYellow);
            throttleLever = leverHandle.transform;

            CreatePrimitive(PrimitiveType.Cube, "Switch_IgnitionArm", transform, new Vector3(-0.30f, -0.28f, 1.45f), new Vector3(0.04f, 0.08f, 0.04f), CockpitMaterialPalette.MatIndicatorYellow).transform.localRotation = Quaternion.Euler(25f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "Switch_RCSArm", transform, new Vector3(-0.20f, -0.28f, 1.45f), new Vector3(0.04f, 0.08f, 0.04f), CockpitMaterialPalette.MatIndicatorGreen).transform.localRotation = Quaternion.Euler(25f, 0f, 0f);

            // 7. Luces LED de Estado
            statusLeds = new Renderer[3];
            statusLeds[0] = CreatePrimitive(PrimitiveType.Sphere, "LED_Nominal", transform, new Vector3(0.35f, 0.05f, 1.68f), new Vector3(0.04f, 0.04f, 0.04f), CockpitMaterialPalette.MatIndicatorGreen).GetComponent<Renderer>();
            statusLeds[1] = CreatePrimitive(PrimitiveType.Sphere, "LED_Warning", transform, new Vector3(0.42f, 0.05f, 1.68f), new Vector3(0.04f, 0.04f, 0.04f), CockpitMaterialPalette.MatIndicatorYellow).GetComponent<Renderer>();
            statusLeds[2] = CreatePrimitive(PrimitiveType.Sphere, "LED_Critical", transform, new Vector3(0.49f, 0.05f, 1.68f), new Vector3(0.04f, 0.04f, 0.04f), CockpitMaterialPalette.MatIndicatorRed).GetComponent<Renderer>();

            // 8. Asiento Ergonómico del Piloto (Ubicado justo bajo la cámara POV)
            CreatePrimitive(PrimitiveType.Cube, "PilotSeat_Cushion", transform, new Vector3(0f, -0.42f, 0.38f), new Vector3(0.65f, 0.14f, 0.65f), matAccent);
            GameObject backrest = CreatePrimitive(PrimitiveType.Cube, "PilotSeat_Backrest", transform, new Vector3(0f, -0.05f, 0.12f), new Vector3(0.58f, 0.72f, 0.14f), matAccent);
            backrest.transform.localRotation = Quaternion.Euler(-12f, 0f, 0f);
            GameObject headrest = CreatePrimitive(PrimitiveType.Cube, "PilotSeat_Headrest", transform, new Vector3(0f, 0.38f, 0.04f), new Vector3(0.35f, 0.22f, 0.12f), matAccent);
            headrest.transform.localRotation = Quaternion.Euler(-12f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "PilotSeat_ArmrestLeft", transform, new Vector3(-0.38f, -0.22f, 0.36f), new Vector3(0.12f, 0.08f, 0.48f), matConsole);
            CreatePrimitive(PrimitiveType.Cube, "PilotSeat_ArmrestRight", transform, new Vector3(0.38f, -0.22f, 0.36f), new Vector3(0.12f, 0.08f, 0.48f), matConsole);
            CreatePrimitive(PrimitiveType.Cylinder, "PilotSeat_Pedestal", transform, new Vector3(0f, -0.78f, 0.38f), new Vector3(0.24f, 0.30f, 0.24f), matConsole);

            // Palanca de mando / Yoke central
            GameObject stickBase = CreatePrimitive(PrimitiveType.Cylinder, "FlightYoke_Column", transform, new Vector3(0f, -0.32f, 0.78f), new Vector3(0.05f, 0.22f, 0.05f), matConsole);
            stickBase.transform.localRotation = Quaternion.Euler(18f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cube, "FlightYoke_Grip", stickBase.transform, new Vector3(0f, 0.9f, 0f), new Vector3(0.22f, 0.06f, 0.06f), matAccent);

            // 9. Iluminación Interior de Cabina (Luz cenital cálida que baña los paneles de Al-Li)
            GameObject cabinLightObj = new GameObject("Cabin_Interior_Light");
            cabinLightObj.transform.SetParent(transform, false);
            cabinLightObj.transform.localPosition = new Vector3(0f, 0.85f, 1.1f);
            Light cabinLight = cabinLightObj.AddComponent<Light>();
            cabinLight.type = LightType.Point;
            cabinLight.color = new Color(1.0f, 0.95f, 0.88f);
            cabinLight.range = 5.5f;
            cabinLight.intensity = 2.8f;
        }

        public void UpdateTelemetryVisuals(float fuelRatio, float currentVelocityKms, float feltGForce, bool isWarning, bool isCritical)
        {
            // Actualizar nivel de los tanques cilíndricos criogénicos (LOX y LH2)
            fuelRatio = Mathf.Clamp01(fuelRatio);
            if (loxFuelLevelBar != null)
            {
                loxFuelLevelBar.localScale = new Vector3(initialLoxScale.x, initialLoxScale.y * fuelRatio, initialLoxScale.z);
            }
            if (lh2FuelLevelBar != null)
            {
                lh2FuelLevelBar.localScale = new Vector3(initialLh2Scale.x, initialLh2Scale.y * fuelRatio, initialLh2Scale.z);
            }

            // Aguja física del velocímetro orbital: Rota de 0° (0 km/s) a -220° (40 km/s escape hiperbólico)
            if (velocimeterNeedle != null)
            {
                float spdAngle = Mathf.Lerp(0f, -220f, Mathf.Clamp01(currentVelocityKms / 40.0f));
                velocimeterNeedle.localRotation = Quaternion.Euler(0f, spdAngle, 0f);
            }

            // Aguja de aceleración (G-Meter): Rota proporcionalmente entre 0g (0°) y 6g (-140°)
            if (gMeterNeedle != null)
            {
                float angle = Mathf.Lerp(0f, -140f, Mathf.Clamp01(feltGForce / 6.0f));
                gMeterNeedle.localRotation = Quaternion.Euler(0f, angle, 0f);
            }

            // Aguja de presión de cabina y atmósfera de soporte vital (O2 nominal a -25°, caída si alerta)
            if (o2PressureNeedle != null)
            {
                float o2Angle = isCritical ? -95f : (isWarning ? -60f : -25f);
                o2PressureNeedle.localRotation = Quaternion.Euler(0f, o2Angle, 0f);
            }

            // Inclinación de la palanca de aceleración
            if (throttleLever != null)
            {
                float leverAngle = feltGForce > 0.1f ? 28f : 0f;
                throttleLever.localRotation = Quaternion.Euler(leverAngle, 0f, 0f);
            }

            // Actualización dinámica de LEDs de Estado en salpicadero
            if (statusLeds != null && statusLeds.Length >= 3)
            {
                bool blink = (Mathf.Sin(Time.time * 8.0f) > 0f);
                if (statusLeds[0] != null) statusLeds[0].material = (!isWarning && !isCritical) ? CockpitMaterialPalette.MatIndicatorGreen : CockpitMaterialPalette.MatConsoleDark;
                if (statusLeds[1] != null) statusLeds[1].material = (isWarning && blink) ? CockpitMaterialPalette.MatIndicatorYellow : CockpitMaterialPalette.MatConsoleDark;
                if (statusLeds[2] != null) statusLeds[2].material = (isCritical && blink) ? CockpitMaterialPalette.MatIndicatorRed : CockpitMaterialPalette.MatConsoleDark;
            }
        }

        private static GameObject CreatePrimitive(PrimitiveType type, string name, Transform parent, Vector3 localPos, Vector3 localScale, Material mat)
        {
            GameObject obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = localScale;

            // Retirar colisionadores para máximo rendimiento
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

