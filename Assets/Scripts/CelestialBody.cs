using UnityEngine;

namespace Alcubierre
{
    public enum BodyCategory
    {
        Star,
        Planet,
        Moon,
        DeepSpaceRegion,
        InterstellarStar,
        Galaxy
    }

    /// <summary>
    /// Representa un cuerpo o hito astronómico dentro del escenario con representación visual normalizada y órbitas coherentes.
    /// Incorpora dinámica orbital Kepleriana en tiempo real (traslación orbital y rotación axial).
    /// </summary>
    public class CelestialBody : MonoBehaviour
    {
        [Header("Datos Astronómicos Reales")]
        public string bodyName = "Tierra";
        public BodyCategory category = BodyCategory.Planet;
        public double realDistanceAU = 1.0;            // Distancia orbital media al Sol en UA
        public double distanceFromEarthKm = 0.0;       // Distancia de referencia desde la Tierra en km
        public double realRadiusKm = 6371.0;           // Radio físico del cuerpo
        public float orbitalPeriodDays = 365.25f;      // Periodo orbital
        public float rotationSpeed = 0.85f;            // Velocidad de rotación visual serena sobre su eje (0.85°/s)

        [Header("Propiedades Atmosféricas y de Superficie")]
        public bool hasAtmosphere = false;             // ¿Posee atmósfera que genere fricción/vibración de reentrada?
        public bool canLandOnSurface = true;           // ¿Tiene superficie sólida de posado o requiere inserción orbital?

        [Header("Representación Visual & Órbita Kepleriana")]
        public Sprite bodySprite;
        public Color orbitColor = new Color(0.2f, 0.6f, 1.0f, 0.35f);
        public float visualRadius = 1.0f;              // Diámetro visual normalizado en unidades Unity
        public bool isReferenceOrigin = false;        // ¿Es el planeta Tierra de origen?
        public Vector3 orbitCenter = new Vector3(-14f, 0f, 0f); // Centro orbital (Sol por defecto)
        public CelestialBody parentBody = null;       // Cuerpo primario (ej. Tierra para la Luna)
        public float orbitRadius = 0f;                 // Radio orbital visual
        public float currentOrbitalAngle = 0f;         // Ángulo orbital actual en grados
        public float orbitalAngularSpeed = 0f;         // Velocidad angular en grados por segundo
        public bool enableOrbitalMotion = true;

        private SpriteRenderer spriteRenderer;
        private LineRenderer orbitLine;

        public Vector2 VisualPosition => transform.position;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        private void Start()
        {
            if (bodySprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = bodySprite;
            }

            // Escala normalizada del sprite: visualRadius determina el tamaño exacto en pantalla
            transform.localScale = Vector3.one * visualRadius;

            ConfigurePhysicalProperties();
            InitializeOrbit(orbitCenter, parentBody);
            SetupOrbitLine();
        }

        private void Update()
        {
            // La traslación orbital se gestiona de forma centralizada por SimulationManager con timeWarpMultiplier,
            // pero si corre de forma aislada rota suavemente sobre su eje.
            if (orbitalAngularSpeed == 0f)
            {
                transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            }
        }

        public void ConfigurePhysicalProperties()
        {
            hasAtmosphere = (bodyName.Contains("Tierra") || bodyName.Contains("Marte") || bodyName.Contains("Venus"));

            canLandOnSurface = !(bodyName.Contains("Sol") || bodyName.Contains("Júpiter") || 
                                 bodyName.Contains("Saturno") || bodyName.Contains("Urano") || 
                                 bodyName.Contains("Neptuno") || bodyName.Contains("Púlsar") || 
                                 bodyName.Contains("PSR") || bodyName.Contains("Agujero") || 
                                 bodyName.Contains("Cygnus") || category == BodyCategory.Galaxy || 
                                 category == BodyCategory.DeepSpaceRegion);
        }

        public void InitializeOrbit(Vector3 center, CelestialBody parent = null)
        {
            orbitCenter = center;
            parentBody = parent;

            Vector3 pivot = (parentBody != null) ? parentBody.transform.position : orbitCenter;
            orbitRadius = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), 
                                           new Vector2(pivot.x, pivot.y));

            currentOrbitalAngle = Mathf.Atan2(transform.position.y - pivot.y, transform.position.x - pivot.x) * Mathf.Rad2Deg;

            // Velocidad orbital calibrada según la astrodinámica Kepleriana y la duración de la simulación
            if (category == BodyCategory.Star || category == BodyCategory.Galaxy || category == BodyCategory.DeepSpaceRegion)
            {
                orbitalAngularSpeed = 0f;
            }
            else if (bodyName.Contains("Luna"))
            {
                // La Luna orbita la Tierra a ~0.32°/s para que un viaje de 120s cubra ~38° (equivalente exacto a los 3 días físicos de Apolo)
                orbitalAngularSpeed = 0.32f;
            }
            else
            {
                float a = Mathf.Max(0.25f, (float)realDistanceAU);
                float keplerFactor = 1.0f / Mathf.Pow(a, 1.5f);
                // Velocidad base de la Tierra (1 UA) calibrada a 0.024° por segundo (~13.3 veces más lenta que la Luna, proporción física real)
                orbitalAngularSpeed = 0.024f * keplerFactor;
            }
        }

        public void UpdateOrbitalMotion(float timeMultiplier, float dt)
        {
            if (!enableOrbitalMotion) return;

            // 1. Rotación axial propia del cuerpo
            transform.Rotate(0f, 0f, rotationSpeed * Mathf.Max(0.2f, timeMultiplier) * dt);

            if (orbitalAngularSpeed <= 0.001f) return;

            // 2. Traslación orbital Kepleriana
            float deltaAngle = orbitalAngularSpeed * timeMultiplier * dt;
            currentOrbitalAngle = (currentOrbitalAngle + deltaAngle) % 360f;

            Vector3 pivot = (parentBody != null) ? parentBody.transform.position : orbitCenter;
            float rad = currentOrbitalAngle * Mathf.Deg2Rad;
            transform.position = new Vector3(
                pivot.x + Mathf.Cos(rad) * orbitRadius,
                pivot.y + Mathf.Sin(rad) * orbitRadius,
                0f
            );

            // 3. Si es un satélite secundario (como la Luna), actualizar la línea de órbita alrededor del planeta móvil
            if (parentBody != null && orbitLine != null)
            {
                Vector3 c = parentBody.transform.position;
                for (int i = 0; i < 80; i++)
                {
                    float angle = (i / 80f) * Mathf.PI * 2f;
                    Vector3 pt = c + new Vector3(Mathf.Cos(angle) * orbitRadius, Mathf.Sin(angle) * orbitRadius, 0.1f);
                    orbitLine.SetPosition(i, pt);
                }
            }
        }

        public void SetupOrbitLine()
        {
            // Solo los planetas del sistema solar y la Luna tienen órbitas cerradas
            if (category == BodyCategory.Star || category == BodyCategory.InterstellarStar || 
                category == BodyCategory.Galaxy || category == BodyCategory.DeepSpaceRegion || isReferenceOrigin)
            {
                return;
            }

            if (orbitLine == null)
            {
                orbitLine = gameObject.AddComponent<LineRenderer>();
            }

            orbitLine.useWorldSpace = true;
            orbitLine.loop = true;
            orbitLine.positionCount = 80;
            orbitLine.startWidth = 0.04f;
            orbitLine.endWidth = 0.04f;

            var mat = new Material(Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/Unlit"));
            orbitLine.material = mat;
            orbitLine.startColor = orbitColor;
            orbitLine.endColor = orbitColor;

            // Trazar el círculo orbital centrado exactamente en su centro orbital (Sol o Tierra)
            float r = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), 
                                       new Vector2(orbitCenter.x, orbitCenter.y));
            for (int i = 0; i < 80; i++)
            {
                float angle = (i / 80f) * Mathf.PI * 2f;
                Vector3 pt = orbitCenter + new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0.1f);
                orbitLine.SetPosition(i, pt);
            }
        }
    }
}
