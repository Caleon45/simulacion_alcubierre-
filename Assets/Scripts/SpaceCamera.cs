using UnityEngine;
using UnityEngine.InputSystem;

namespace Alcubierre
{
    public enum CameraFollowMode
    {
        Free,           // Cámara libre para explorar el espacio
        FollowShip,     // Fijar y seguir a la nave espacial
        FocusEarth,     // Fijar en la Tierra
        FocusSun,       // Fijar en el Sol
        FocusTarget     // Fijar en el cuerpo celeste destino
    }

    /// <summary>
    /// Controlador de cámara 2D multiescala con zoom libre (0.5 a 250 unidades)
    /// y navegación fluida por ratón (arrastre, rueda) y teclado (WASD / flechas).
    /// Compatible con el Nuevo Input System y el sistema clásico.
    /// </summary>
    public class SpaceCamera : MonoBehaviour
    {
        [Header("Zoom Multiescala")]
        public float minOrthoSize = 0.8f;      // Escala de nave y superficie planetaria
        public float maxOrthoSize = 250.0f;    // Escala interestelar / Vía Láctea / Andrómeda
        public float zoomSensitivity = 1.25f;
        public float zoomSmoothTime = 0.08f;

        [Header("Paneo y Desplazamiento")]
        public float basePanSpeed = 18.0f;

        [Header("Modo de Cámara")]
        public CameraFollowMode currentMode = CameraFollowMode.Free;
        public Transform targetTransform;

        private Camera cam;
        private float targetOrthoSize = 35.0f;
        private float zoomVelocity;
        private Vector2 lastMouseScreenPos;
        private bool isDragging = false;

        // Propiedad para evitar arrastre de cámara si el clic ocurre sobre la interfaz IMGUI
        public static bool IsMouseOverUI = false;

        public float CurrentOrthoSize => cam != null ? cam.orthographicSize : targetOrthoSize;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam == null) cam = Camera.main;
            if (cam != null)
            {
                targetOrthoSize = cam.orthographicSize;
            }
        }

        private void Start()
        {
            if (!enabled) return;

            // Posición inicial panorámica centrada en el Sol y la Tierra
            if (cam != null && currentMode == CameraFollowMode.Free)
            {
                transform.position = new Vector3(0f, 0f, -10f);
                targetOrthoSize = 35f;
                cam.orthographicSize = 35f;
            }
        }

        private void LateUpdate()
        {
            if (!enabled || cam == null) return;

            HandleZoom();
            HandlePan();
            HandleFollowMode();
        }

        private void HandleZoom()
        {
            float scrollDelta = 0f;

            // Leer rueda del ratón desde Input System o Legacy
            if (Mouse.current != null)
            {
                scrollDelta = Mouse.current.scroll.ReadValue().y * 0.0015f;
            }
            else
            {
                try { scrollDelta = Input.GetAxis("Mouse ScrollWheel"); } catch { }
            }

            if (Mathf.Abs(scrollDelta) > 0.0001f)
            {
                // Zoom exponencial proporcional al tamaño actual para suavidad uniforme en cualquier escala
                float zoomFactor = Mathf.Pow(zoomSensitivity, -scrollDelta * 5f);
                targetOrthoSize = Mathf.Clamp(targetOrthoSize * zoomFactor, minOrthoSize, maxOrthoSize);
            }

            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetOrthoSize, ref zoomVelocity, zoomSmoothTime);
        }

        private void HandlePan()
        {
            Vector2 currentMousePos = Vector2.zero;
            bool isRightDown = false;
            bool isRightPressed = false;
            bool isMiddlePressed = false;
            bool isLeftDown = false;
            bool isLeftPressed = false;

            // Lectura de ratón
            if (Mouse.current != null)
            {
                currentMousePos = Mouse.current.position.ReadValue();
                isRightDown = Mouse.current.rightButton.wasPressedThisFrame;
                isRightPressed = Mouse.current.rightButton.isPressed;
                isMiddlePressed = Mouse.current.middleButton.isPressed;
                isLeftDown = Mouse.current.leftButton.wasPressedThisFrame;
                isLeftPressed = Mouse.current.leftButton.isPressed;
            }
            else
            {
                try
                {
                    currentMousePos = Input.mousePosition;
                    isRightDown = Input.GetMouseButtonDown(1);
                    isRightPressed = Input.GetMouseButton(1);
                    isMiddlePressed = Input.GetMouseButton(2);
                    isLeftDown = Input.GetMouseButtonDown(0);
                    isLeftPressed = Input.GetMouseButton(0);
                }
                catch { }
            }

            // Inicio de arrastre:
            // Clic derecho o medio en cualquier parte, o clic izquierdo en espacio abierto (fuera de UI)
            if (isRightDown || isMiddlePressed || (isLeftDown && !IsMouseOverUI))
            {
                isDragging = true;
                lastMouseScreenPos = currentMousePos;
                currentMode = CameraFollowMode.Free;
            }

            // Continuación del arrastre
            if (isDragging)
            {
                bool stillPressing = isRightPressed || isMiddlePressed || (isLeftPressed && !IsMouseOverUI);
                if (stillPressing)
                {
                    // Convertir delta de pantalla a mundo con la cámara en reposo durante el cálculo
                    float zDistance = -transform.position.z;
                    Vector3 prevWorld = cam.ScreenToWorldPoint(new Vector3(lastMouseScreenPos.x, lastMouseScreenPos.y, zDistance));
                    Vector3 currWorld = cam.ScreenToWorldPoint(new Vector3(currentMousePos.x, currentMousePos.y, zDistance));
                    Vector3 worldDelta = currWorld - prevWorld;

                    transform.position -= new Vector3(worldDelta.x, worldDelta.y, 0f);
                    lastMouseScreenPos = currentMousePos;
                }
                else
                {
                    isDragging = false;
                }
            }

            // Paneo con teclado (WASD / Flechas)
            float moveX = 0f;
            float moveY = 0f;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY -= 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
            }
            else
            {
                try
                {
                    moveX = Input.GetAxisRaw("Horizontal");
                    moveY = Input.GetAxisRaw("Vertical");
                }
                catch { }
            }

            if (Mathf.Abs(moveX) > 0.05f || Mathf.Abs(moveY) > 0.05f)
            {
                currentMode = CameraFollowMode.Free;
                float moveSpeed = (cam.orthographicSize / 8.0f) * basePanSpeed * Time.deltaTime;
                transform.position += new Vector3(moveX * moveSpeed, moveY * moveSpeed, 0f);
            }
        }

        private void HandleFollowMode()
        {
            if (currentMode != CameraFollowMode.Free && targetTransform != null)
            {
                Vector3 targetPos = new Vector3(targetTransform.position.x, targetTransform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 6.0f);
            }
        }

        public void SetMode(CameraFollowMode mode, Transform target = null, float desiredOrthoSize = -1f)
        {
            currentMode = mode;
            if (target != null) targetTransform = target;

            if (desiredOrthoSize > 0f)
            {
                targetOrthoSize = Mathf.Clamp(desiredOrthoSize, minOrthoSize, maxOrthoSize);
            }
        }

        public void ZoomIn(float factor = 1.35f)
        {
            targetOrthoSize = Mathf.Clamp(targetOrthoSize / factor, minOrthoSize, maxOrthoSize);
        }

        public void ZoomOut(float factor = 1.35f)
        {
            targetOrthoSize = Mathf.Clamp(targetOrthoSize * factor, minOrthoSize, maxOrthoSize);
        }

        public void SetDirectZoom(float size)
        {
            targetOrthoSize = Mathf.Clamp(size, minOrthoSize, maxOrthoSize);
        }

        public void CenterOnPosition(Vector3 worldPos, float desiredOrthoSize = -1f)
        {
            currentMode = CameraFollowMode.Free;
            transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
            if (desiredOrthoSize > 0f)
            {
                SetDirectZoom(desiredOrthoSize);
            }
        }
    }
}
