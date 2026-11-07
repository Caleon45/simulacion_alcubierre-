using UnityEngine;

namespace Alcubierre.Cockpit
{
    /// <summary>
    /// Generador y repositorio en memoria de materiales PBR optimizados para las cabinas 3D
    /// según la ciencia de materiales de la carpeta contexto (Al-Li, Tungsteno, Grafeno, Vidrios y Combustibles).
    /// </summary>
    public static class CockpitMaterialPalette
    {
        private static Shader standardShader;
        private static Shader unlitShader;

        // Materiales Estructurales
        public static Material MatAluminumLithium { get; private set; }   // Química: Al-Li 2195 plateado/metálico
        public static Material MatTungstenCarbon { get; private set; }    // Antimateria: Tungsteno mate y fibra de carbono
        public static Material MatPearlTitanium { get; private set; }     // Warp: Titanio satinado blanco perla futurista
        public static Material MatConsoleDark { get; private set; }       // Salpicadero y consolas interiores
        public static Material MatInteriorAccent { get; private set; }    // Molduras y soportes mecánicos

        // Cristales de Parabrisas
        public static Material MatReinforcedGlass { get; private set; }   // Química: Cristal multicapa transparente
        public static Material MatGoldAmberGlass { get; private set; }    // Antimateria: Filtro dorado/ámbar antirradiación
        public static Material MatPanoramicGlass { get; private set; }    // Warp: Cristal panorámico antirreflectante

        // Materiales de Combustible e Indicadores Emisivos
        public static Material MatLOXFuel { get; private set; }           // Oxígeno Líquido (Turquesa criogénico emisivo)
        public static Material MatLH2Fuel { get; private set; }           // Hidrógeno Líquido (Azul eléctrico profundo)
        public static Material MatAntimatterPlasma { get; private set; }  // Plasma de antiprotones (Púrpura/violeta pulsante)
        public static Material MatCasimirWarpCore { get; private set; }   // Resonador Casimir negativo (Cian radiante)
        public static Material MatScreenEmissive { get; private set; }    // Monitores tácticos digitales
        public static Material MatIndicatorGreen { get; private set; }    // LEDs estado nominal
        public static Material MatIndicatorYellow { get; private set; }   // LEDs advertencia
        public static Material MatIndicatorRed { get; private set; }      // LEDs alarma crítica

        static CockpitMaterialPalette()
        {
            InitializePalette();
        }

        public static void InitializePalette()
        {
            // Búsqueda de shaders de URP con fallback a Standard/Mobile
            standardShader = Shader.Find("Universal Render Pipeline/Lit") ?? 
                             Shader.Find("Standard") ?? 
                             Shader.Find("Specular");

            unlitShader = Shader.Find("Universal Render Pipeline/Unlit") ?? 
                          Shader.Find("Unlit/Color") ?? 
                          Shader.Find("Sprites/Default");

            // 1. Estructura Química: Aleación Aluminio-Litio (Plateado industrial mate nítido)
            MatAluminumLithium = CreatePBRMaterial(new Color(0.85f, 0.88f, 0.90f), 0.50f, 0.55f);

            // 2. Estructura Antimateria: Compuesto de Tungsteno y Carbono (Grafito y tungsteno visible)
            MatTungstenCarbon = CreatePBRMaterial(new Color(0.32f, 0.35f, 0.38f), 0.65f, 0.40f);

            // 3. Estructura Warp: Titanio Satinado Blanco Perla (Estilo NASA-Future IXS Enterprise)
            MatPearlTitanium = CreatePBRMaterial(new Color(0.94f, 0.95f, 0.98f), 0.25f, 0.75f);

            // 4. Consola y salpicadero (Gris carbón balanceado)
            MatConsoleDark = CreatePBRMaterial(new Color(0.22f, 0.24f, 0.27f), 0.15f, 0.35f);
            MatInteriorAccent = CreatePBRMaterial(new Color(0.38f, 0.42f, 0.46f), 0.40f, 0.45f);

            // 5. Cristales de Parabrisas
            MatReinforcedGlass = CreateGlassMaterial(new Color(0.85f, 0.95f, 1.0f, 0.20f));
            MatGoldAmberGlass = CreateGlassMaterial(new Color(0.95f, 0.75f, 0.25f, 0.32f)); // Polarizado dorado
            MatPanoramicGlass = CreateGlassMaterial(new Color(0.75f, 0.90f, 1.0f, 0.12f));

            // 6. Combustibles Criogénicos y Plasma
            MatLOXFuel = CreateEmissiveMaterial(new Color(0.1f, 0.75f, 0.85f), 2.2f); // Turquesa LOX
            MatLH2Fuel = CreateEmissiveMaterial(new Color(0.15f, 0.40f, 0.95f), 2.5f); // Azul LH2
            MatAntimatterPlasma = CreateEmissiveMaterial(new Color(0.75f, 0.20f, 1.0f), 3.0f); // Púrpura Antimateria
            MatCasimirWarpCore = CreateEmissiveMaterial(new Color(0.1f, 0.85f, 1.0f), 2.8f); // Cian Warp

            // 7. Pantallas y LEDs
            MatScreenEmissive = CreateScreenMaterial();
            MatIndicatorGreen = CreateEmissiveMaterial(new Color(0.1f, 1.0f, 0.2f), 2.0f);
            MatIndicatorYellow = CreateEmissiveMaterial(new Color(1.0f, 0.85f, 0.1f), 2.0f);
            MatIndicatorRed = CreateEmissiveMaterial(new Color(1.0f, 0.15f, 0.15f), 2.5f);
        }

        private static Material CreateScreenMaterial()
        {
            Material mat = new Material(unlitShader);
            Color screenTint = new Color(0.7f, 0.9f, 1.0f, 1.0f);
            mat.color = screenTint;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", screenTint);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", screenTint);

            Texture2D hudTex = CreateNavScreenTexture();
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", hudTex);
            if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", hudTex);
            mat.mainTexture = hudTex;

            return mat;
        }

        private static Texture2D CreateNavScreenTexture()
        {
            int size = 256;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            Color bg = new Color(0.02f, 0.05f, 0.10f, 1.0f);
            Color grid = new Color(0.06f, 0.18f, 0.28f, 1.0f);
            Color reticle = new Color(0.15f, 0.85f, 0.95f, 1.0f);
            Color target = new Color(0.2f, 1.0f, 0.4f, 1.0f);

            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int idx = y * size + x;
                    pixels[idx] = bg;

                    // Cuadrícula táctica cada 32 píxeles
                    if (x % 32 == 0 || y % 32 == 0)
                    {
                        pixels[idx] = grid;
                    }

                    // Anillos concéntricos de radar astronómico
                    float dx = x - 128;
                    float dy = y - 128;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (Mathf.Abs(dist - 35) < 1.2f || Mathf.Abs(dist - 70) < 1.2f || Mathf.Abs(dist - 105) < 1.4f)
                    {
                        pixels[idx] = reticle;
                    }

                    // Cruz de puntería central
                    if ((Mathf.Abs(dx) < 18 && Mathf.Abs(dy) < 1.5f) || (Mathf.Abs(dy) < 18 && Mathf.Abs(dx) < 1.5f))
                    {
                        pixels[idx] = reticle;
                    }

                    // Vector de vuelo hacia el objetivo
                    if (Mathf.Abs(dx - 35) < 3.5f && Mathf.Abs(dy - 25) < 3.5f)
                    {
                        pixels[idx] = target;
                    }

                    // Borde perimetral del HUD
                    if (x < 3 || x >= size - 3 || y < 3 || y >= size - 3)
                    {
                        pixels[idx] = reticle;
                    }
                }
            }
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static Material CreatePBRMaterial(Color baseColor, float metallic, float smoothness)
        {
            Material mat = new Material(standardShader);
            mat.color = baseColor;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", baseColor);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            return mat;
        }

        private static Material CreateGlassMaterial(Color tint)
        {
            // Shader transparente para parabrisas de cabinas (máxima visibilidad hacia el cosmos exterior)
            Shader glassShader = Shader.Find("Universal Render Pipeline/Unlit") ?? 
                                 Shader.Find("Universal Render Pipeline/Lit") ?? 
                                 Shader.Find("Standard");

            Material mat = new Material(glassShader);
            Color subtleTint = new Color(tint.r, tint.g, tint.b, 0.08f); // 92% transparente para óptima visión exterior
            mat.color = subtleTint;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", subtleTint);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", subtleTint);

            // Configuración exhaustiva de transparencia en URP y Built-in
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetFloat("_Surface", 1.0f); // 1 = Transparent en URP
            mat.SetFloat("_Blend", 0.0f);   // 0 = Alpha blend
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            mat.SetShaderPassEnabled("DepthOnly", false);
            mat.SetShaderPassEnabled("SHADOWCASTER", false);

            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0.95f);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", 0.05f);
            return mat;
        }

        private static Material CreateEmissiveMaterial(Color color, float intensity)
        {
            Material mat = new Material(unlitShader);
            Color finalColor = color * intensity;
            mat.color = finalColor;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", finalColor);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", finalColor);
            return mat;
        }
    }
}

