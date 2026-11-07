using UnityEngine;

namespace Alcubierre
{
    /// <summary>
    /// Representa una entidad individual dentro del ecosistema de simulación.
    /// Posee estado de ejecución, energía, velocidad y comportamiento de movimiento.
    /// </summary>
    public class SimulationEntity : MonoBehaviour
    {
        [Header("Identificación")]
        public string entityId;
        public string entityType = "Agente";

        [Header("Estado de Ejecución")]
        public bool isAlive = true;
        public float energy = 100f;
        public Vector3 currentVelocity = Vector3.zero;

        private Renderer entityRenderer;

        private void Awake()
        {
            entityRenderer = GetComponent<Renderer>();
        }

        /// <summary>
        /// Inicializa los parámetros visuales y de estado de la entidad.
        /// </summary>
        public void Initialize(string id, string type, Color color)
        {
            entityId = id;
            entityType = type;
            isAlive = true;
            energy = 100f;
            currentVelocity = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0f);

            if (entityRenderer != null)
            {
                entityRenderer.material.color = color;
            }
        }

        private void Update()
        {
            if (!isAlive) return;

            // Movimiento dinámico en el área 2D
            transform.position += currentVelocity * Time.deltaTime;

            // Rebote suave en los límites del ecosistema visible
            Vector3 pos = transform.position;
            if (Mathf.Abs(pos.x) > 6.5f)
            {
                currentVelocity.x = -currentVelocity.x;
                pos.x = Mathf.Clamp(pos.x, -6.5f, 6.5f);
            }
            if (Mathf.Abs(pos.y) > 3.8f)
            {
                currentVelocity.y = -currentVelocity.y;
                pos.y = Mathf.Clamp(pos.y, -3.8f, 3.8f);
            }
            transform.position = pos;
        }

        /// <summary>
        /// Actualiza la visualización según el estado actual (ej: color por energía o vida).
        /// </summary>
        public void UpdateVisuals()
        {
            if (!isAlive)
            {
                if (entityRenderer != null)
                {
                    entityRenderer.material.color = Color.gray;
                }
            }
        }
    }
}
