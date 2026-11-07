using UnityEngine;
using UnityEngine.InputSystem;

namespace Alcubierre.Cockpit
{
    /// <summary>
    /// Administrador maestro del ecosistema de Cabinas 3D (Cockpit POV).
    /// Controla la instanciación de las 3 cabinas (Química, Antimateria, Warp),
    /// la rotación suave de cabeza en primera persona (Mouselook), la actualización en tiempo real
    /// de los indicadores de combustible y diales físicos, y la integración de la pantalla táctica.
    /// </summary>
    public class CockpitManager : MonoBehaviour
    {
        [Header("Propulsión y Cabina Activa")]
        public PropulsionMode activeMode = PropulsionMode.WarpDrive;

        [Header("Contenedores de Cabinas")]
        public ChemicalCockpit chemicalCockpit;
        public AntimatterCockpit antimatterCockpit;
        public WarpCockpit warpCockpit;
        public CockpitExteriorView exteriorView;

        [Header("Control de Cámara POV en Primera Persona")]
        public Camera povCamera;
        public float mouseSensitivity = 2.0f;
        public float pitchLimit = 50.0f;
        public float yawLimit = 75.0f;
        public bool isHeadLookActive = true;

        private float currentPitch = 14.0f;
        private float currentYaw = 0f;
        public float targetRestingPitch = 14.0f;
        private Vector3 baseCameraPosition = new Vector3(0f, 0.12f, 0.38f);

        private void Awake()
        {
            if (povCamera == null) povCamera = Camera.main;

            // Instanciar y construir las 3 cabinas procedurales
            BuildAllCockpits();
        }

        private void Start()
        {
            // Activar la cabina según el modo inicial
            SetCockpitMode(activeMode);

            if (povCamera != null)
            {
                povCamera.transform.SetParent(transform, false);
                povCamera.transform.localPosition = baseCameraPosition;
                povCamera.transform.localRotation = Quaternion.Euler(targetRestingPitch, 0f, 0f);
                povCamera.fieldOfView = 65.0f;
            }
        }

        [Header("Efecto de Explosión / Sacudida Catastrófica")]
        public bool isCatastropheShaking = false;
        private float catastropheIntensity = 0f;

        [Header("Dinámica Relativista de FOV y Óptica Observacional")]
        public float targetFOV = 65.0f;

        private void Update()
        {
            if (povCamera != null)
            {
                if (povCamera.transform.parent != transform)
                {
                    povCamera.transform.SetParent(transform, false);
                    povCamera.transform.localPosition = baseCameraPosition;
                    povCamera.transform.localRotation = Quaternion.Euler(targetRestingPitch, 0f, 0f);
                }
                if (povCamera.orthographic)
                {
                    povCamera.orthographic = false;
                    povCamera.fieldOfView = targetFOV;
                    povCamera.nearClipPlane = 0.02f;
                    povCamera.farClipPlane = 1200f;
                }
                else
                {
                    // Transición suave de campo de visión (FOV) según la óptica física activa
                    povCamera.fieldOfView = Mathf.Lerp(povCamera.fieldOfView, targetFOV, Time.deltaTime * 3.5f);
                }
            }

            HandleMouselook();
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            if (povCamera == null) return;
            Vector3 shake = exteriorView != null ? exteriorView.CurrentShakeOffset : Vector3.zero;
            if (isCatastropheShaking)
            {
                shake += Random.insideUnitSphere * (0.09f * Mathf.Max(0.5f, catastropheIntensity));
            }
            povCamera.transform.localPosition = baseCameraPosition + shake;
        }

        public void TriggerCatastropheShake(bool active, float intensity = 1.0f)
        {
            isCatastropheShaking = active;
            catastropheIntensity = intensity;
        }

        public void LookAtDashboard()
        {
            targetRestingPitch = 30.0f;
        }

        public void LookForward()
        {
            targetRestingPitch = 14.0f;
        }

        public void BuildAllCockpits()
        {
            // 1. Construir Cabina Química
            if (chemicalCockpit == null)
            {
                GameObject chemObj = new GameObject("Cockpit_Quimica");
                chemObj.transform.SetParent(transform, false);
                chemicalCockpit = chemObj.AddComponent<ChemicalCockpit>();
                chemicalCockpit.BuildCockpit();
            }

            // 2. Construir Cabina de Antimateria
            if (antimatterCockpit == null)
            {
                GameObject antiObj = new GameObject("Cockpit_Antimateria");
                antiObj.transform.SetParent(transform, false);
                antimatterCockpit = antiObj.AddComponent<AntimatterCockpit>();
                antimatterCockpit.BuildCockpit();
            }

            // 3. Construir Cabina de Curvatura Warp
            if (warpCockpit == null)
            {
                GameObject warpObj = new GameObject("Cockpit_Warp");
                warpObj.transform.SetParent(transform, false);
                warpCockpit = warpObj.AddComponent<WarpCockpit>();
                warpCockpit.BuildCockpit();
            }

            // 4. Construir Vista Exterior y Campo Estelar Relativista
            if (exteriorView == null)
            {
                GameObject extObj = new GameObject("Cockpit_ExteriorView");
                extObj.transform.SetParent(transform, false);
                exteriorView = extObj.AddComponent<CockpitExteriorView>();
                exteriorView.Initialize(transform, povCamera);
            }

            // 5. Luces Interiores de la Cabina (Garantiza visibilidad nítida de controles, mamparos y diales)
            GameObject keyLightObj = new GameObject("Cockpit_KeyLight");
            keyLightObj.transform.SetParent(transform, false);
            keyLightObj.transform.localPosition = new Vector3(0f, 2.5f, 0.5f);
            keyLightObj.transform.localRotation = Quaternion.Euler(35f, 20f, 0f);
            Light keyLight = keyLightObj.AddComponent<Light>();
            keyLight.type = LightType.Directional;
            keyLight.color = new Color(1f, 0.98f, 0.95f);
            keyLight.intensity = 1.15f;

            // Luz directa a la consola (Asegura iluminación nítida de diales, agujas y tanques)
            GameObject consoleKeyLightObj = new GameObject("Cockpit_ConsoleKeyLight");
            consoleKeyLightObj.transform.SetParent(transform, false);
            consoleKeyLightObj.transform.localPosition = new Vector3(0f, 1.8f, -0.1f);
            consoleKeyLightObj.transform.localRotation = Quaternion.Euler(45f, 0f, 0f);
            Light conKeyLight = consoleKeyLightObj.AddComponent<Light>();
            conKeyLight.type = LightType.Directional;
            conKeyLight.color = new Color(0.96f, 0.98f, 1.0f);
            conKeyLight.intensity = 1.35f;

            GameObject fillLightObj = new GameObject("Cockpit_DashboardFillLight");
            fillLightObj.transform.SetParent(transform, false);
            fillLightObj.transform.localPosition = new Vector3(0f, 0.35f, 1.15f);
            Light fillLight = fillLightObj.AddComponent<Light>();
            fillLight.type = LightType.Point;
            fillLight.color = new Color(0.92f, 0.96f, 1.0f);
            fillLight.range = 6.0f;
            fillLight.intensity = 2.4f;

            GameObject domeLightObj = new GameObject("Cockpit_OverheadDomeLight");
            domeLightObj.transform.SetParent(transform, false);
            domeLightObj.transform.localPosition = new Vector3(0f, 1.1f, 1.2f);
            Light domeLight = domeLightObj.AddComponent<Light>();
            domeLight.type = LightType.Point;
            domeLight.color = new Color(0.95f, 0.95f, 1.0f);
            domeLight.range = 6.0f;
            domeLight.intensity = 2.0f;
        }

        public void SetCockpitMode(PropulsionMode mode)
        {
            activeMode = mode;

            if (chemicalCockpit != null) chemicalCockpit.gameObject.SetActive(mode == PropulsionMode.Chemical);
            if (antimatterCockpit != null) antimatterCockpit.gameObject.SetActive(mode == PropulsionMode.Antimatter);
            if (warpCockpit != null) warpCockpit.gameObject.SetActive(mode == PropulsionMode.WarpDrive);
        }

        private void HandleMouselook()
        {
            if (povCamera == null) return;

            // Rotar cabeza con clic derecho sostenido o libre si se activa
            bool shouldRotate = false;
            Vector2 mouseDelta = Vector2.zero;

            if (Mouse.current != null)
            {
                shouldRotate = Mouse.current.rightButton.isPressed;
                mouseDelta = Mouse.current.delta.ReadValue() * 0.12f;
            }
            else
            {
                try
                {
                    shouldRotate = Input.GetMouseButton(1);
                    mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 2.0f;
                }
                catch { }
            }

            if (shouldRotate && mouseDelta.sqrMagnitude > 0.0001f)
            {
                currentYaw += mouseDelta.x * mouseSensitivity;
                currentPitch -= mouseDelta.y * mouseSensitivity;

                currentYaw = Mathf.Clamp(currentYaw, -yawLimit, yawLimit);
                currentPitch = Mathf.Clamp(currentPitch, -pitchLimit * 0.8f, pitchLimit);

                povCamera.transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            }
            else if (!shouldRotate)
            {
                // Retorno suave a la posición neutra activa (mirando al frente o inspeccionando consola)
                currentYaw = Mathf.Lerp(currentYaw, 0f, Time.deltaTime * 3.5f);
                currentPitch = Mathf.Lerp(currentPitch, targetRestingPitch, Time.deltaTime * 3.5f);
                povCamera.transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            }
        }

        public void SetTargetDestination(string name, Color color)
        {
            if (exteriorView != null)
            {
                exteriorView.SetTargetDestination(name, color);
            }
        }

        /// <summary>
        /// Configura la cámara principal para la vista en primera persona dentro de la cabina 3D.
        /// </summary>
        public void EnableCockpitPOV()
        {
            gameObject.SetActive(true);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.62f, 0.65f, 0.72f);
            if (povCamera != null)
            {
                povCamera.transform.SetParent(transform, false);
                povCamera.transform.localPosition = baseCameraPosition;
                povCamera.transform.localRotation = Quaternion.Euler(targetRestingPitch, 0f, 0f);
                povCamera.orthographic = false;
                povCamera.fieldOfView = 65.0f;
                povCamera.nearClipPlane = 0.02f;
                povCamera.farClipPlane = 1200f;
            }
        }

        /// <summary>
        /// Desvincula la cámara al cambiar a la vista de Mapa Táctico 2D.
        /// </summary>
        public void DisableCockpitPOV()
        {
            RenderSettings.ambientLight = Color.white;
            if (povCamera != null)
            {
                povCamera.transform.SetParent(null);
                povCamera.orthographic = true;
                povCamera.nearClipPlane = 0.1f;
                povCamera.farClipPlane = 500f;
                povCamera.transform.position = new Vector3(0f, 0f, -10f);
                povCamera.transform.rotation = Quaternion.identity;
            }
            currentPitch = 0f;
            currentYaw = 0f;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Actualiza los instrumentos, diales y animación de combustibles en tiempo real en la cabina activa.
        /// </summary>
        public void UpdateActiveCockpitTelemetry(float fuelRatio, float flightProgress, float currentVelocityKms, float feltGForce, float velocityFractionC, 
                                                double radiationDose_mSv, bool isFlying, bool isWarning, bool isCritical)
        {
            // 1. Modulación de FOV según la física observacional activa
            if (isFlying)
            {
                if (activeMode == PropulsionMode.WarpDrive)
                {
                    // Lente Gravitatoria / Contracción Métrica Alcubierre: FOV ampliado y estirado a los lados (78°)
                    targetFOV = Mathf.Lerp(65.0f, 78.0f, Mathf.Clamp01(flightProgress * 1.8f));
                }
                else if (activeMode == PropulsionMode.Antimatter)
                {
                    // Aberración Óptica Relativista (Headlight Beaming): estrellas convergen al centro (57°)
                    float beta = Mathf.Clamp01(velocityFractionC);
                    targetFOV = Mathf.Lerp(65.0f, 55.0f, beta);
                }
                else
                {
                    // Cohete Químico: Ángulo de visión humano estándar fijo
                    targetFOV = 65.0f;
                }
            }
            else
            {
                targetFOV = 65.0f;
            }

            if (exteriorView != null)
            {
                exteriorView.UpdateFlightTelemetry(isFlying, flightProgress, velocityFractionC, feltGForce, activeMode);
            }

            switch (activeMode)
            {
                case PropulsionMode.Chemical:
                    if (chemicalCockpit != null && chemicalCockpit.gameObject.activeSelf)
                    {
                        chemicalCockpit.UpdateTelemetryVisuals(fuelRatio, currentVelocityKms, feltGForce, isWarning, isCritical);
                    }
                    break;

                case PropulsionMode.Antimatter:
                    if (antimatterCockpit != null && antimatterCockpit.gameObject.activeSelf)
                    {
                        antimatterCockpit.UpdateTelemetryVisuals(fuelRatio, velocityFractionC, feltGForce, radiationDose_mSv, isWarning, isCritical);
                    }
                    break;

                case PropulsionMode.WarpDrive:
                    if (warpCockpit != null && warpCockpit.gameObject.activeSelf)
                    {
                        warpCockpit.UpdateTelemetryVisuals((float)velocityFractionC, isFlying, isWarning, isCritical);
                    }
                    break;
            }
        }
    }
}
