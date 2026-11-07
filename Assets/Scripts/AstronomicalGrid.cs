using System.Collections.Generic;
using UnityEngine;

namespace Alcubierre
{
    /// <summary>
    /// Genera y dibuja una cuadrícula astronómica semitransparente con círculos concéntricos de distancia
    /// y coordenadas espaciales para dimensionar magnitudes físicas (UA, km, años luz).
    /// </summary>
    public class AstronomicalGrid : MonoBehaviour
    {
        [Header("Opacidad y Apariencia")]
        [Range(0.02f, 0.5f)] public float gridOpacity = 0.18f;
        public Color gridColor = new Color(0.2f, 0.5f, 0.9f);
        public Color ringColor = new Color(0f, 0.85f, 1f);

        [Header("Configuración de Cuadrícula")]
        public float gridSpacing = 10.0f;
        public float maxGridExtent = 280.0f;
        public Vector3 sunCenter = new Vector3(-14f, 0f, 0f);

        private Material lineMaterial;
        private List<LineRenderer> ringRenderers = new List<LineRenderer>();
        private GameObject gridHolder;

        private void Start()
        {
            gridHolder = new GameObject("AstronomicalGridHolder");
            gridHolder.transform.SetParent(transform);

            lineMaterial = new Material(Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/Unlit"));

            CreateDistanceRings();
        }

        private void CreateDistanceRings()
        {
            // Radios visuales calibrados correspondientes a órbitas astronómicas del Sistema Solar
            float[] solarRadii = new float[] {
                5.4f,   // Mercurio (~0.39 UA)
                9.8f,   // Venus (~0.72 UA)
                14.0f,  // Tierra (1.00 UA - Origen de Referencia)
                21.0f,  // Marte (~1.52 UA)
                32.0f,  // Cinturón de Asteroides / Ceres (~2.8 UA)
                46.0f,  // Júpiter (~5.2 UA)
                62.0f,  // Saturno (~9.58 UA)
                80.0f,  // Urano (~19.2 UA)
                98.0f,  // Neptuno (~30.0 UA)
                116.0f  // Plutón / Kuiper (~40 UA)
            };

            foreach (float r in solarRadii)
            {
                GameObject ringObj = new GameObject($"DistanceRing_UA_{r:F0}");
                ringObj.transform.SetParent(gridHolder.transform);

                LineRenderer lr = ringObj.AddComponent<LineRenderer>();
                lr.useWorldSpace = true;
                lr.loop = true;
                lr.positionCount = 80;
                lr.startWidth = 0.04f;
                lr.endWidth = 0.04f;
                lr.material = lineMaterial;

                Color c = new Color(ringColor.r, ringColor.g, ringColor.b, gridOpacity * 0.75f);
                lr.startColor = c;
                lr.endColor = c;

                for (int i = 0; i < 80; i++)
                {
                    float angle = (i / 80f) * Mathf.PI * 2f;
                    Vector3 pt = sunCenter + new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0.2f);
                    lr.SetPosition(i, pt);
                }

                ringRenderers.Add(lr);
            }

            // Anillo orbital local de la Luna alrededor de la Tierra (Origen 0,0,0)
            GameObject moonRingObj = new GameObject("DistanceRing_Moon_Orbit");
            moonRingObj.transform.SetParent(gridHolder.transform);
            LineRenderer moonLr = moonRingObj.AddComponent<LineRenderer>();
            moonLr.useWorldSpace = true;
            moonLr.loop = true;
            moonLr.positionCount = 48;
            moonLr.startWidth = 0.03f;
            moonLr.endWidth = 0.03f;
            moonLr.material = lineMaterial;
            Color moonC = new Color(0.8f, 0.8f, 0.8f, gridOpacity * 0.9f);
            moonLr.startColor = moonC;
            moonLr.endColor = moonC;

            float moonR = 2.16f; // Distancia visual Tierra - Luna
            for (int i = 0; i < 48; i++)
            {
                float angle = (i / 48f) * Mathf.PI * 2f;
                Vector3 pt = new Vector3(Mathf.Cos(angle) * moonR, Mathf.Sin(angle) * moonR, 0.2f);
                moonLr.SetPosition(i, pt);
            }
            ringRenderers.Add(moonLr);
        }

        public void SetOpacity(float newOpacity)
        {
            gridOpacity = Mathf.Clamp(newOpacity, 0.01f, 0.8f);
            Color c = new Color(ringColor.r, ringColor.g, ringColor.b, gridOpacity * 0.75f);
            foreach (var lr in ringRenderers)
            {
                if (lr != null)
                {
                    lr.startColor = c;
                    lr.endColor = c;
                }
            }
        }
    }
}
