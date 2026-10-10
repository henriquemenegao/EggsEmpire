using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera
{
    public class IsometricCameraController : MonoBehaviour
    {
        [Header("Movimentação")]
        [SerializeField] private float dragSpeed = 0.05f;

        [Header("Zoom")]
        [SerializeField] private float zoomSpeed = 1f;
        [SerializeField] private float minZoom = 3f;
        [SerializeField] private float maxZoom = 35f;
        
        [Header("Limites do Mapa")]
        [SerializeField] private float minX = -65f;
        [SerializeField] private float maxX = 25f;
        [SerializeField] private float minZ = -65f;
        [SerializeField] private float maxZ = 15f;

        private UnityEngine.Camera _cam;
        private Vector2 _lastMousePosition;

        private void Awake()
        {
            _cam = GetComponent<UnityEngine.Camera>();
        }

        private void Update()
        {
            HandleMovement();
            HandleZoom();
        }

        private void HandleMovement()
        {
            if (Mouse.current == null)
                return;

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                _lastMousePosition =
                    Mouse.current.position.ReadValue();
            }

            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 currentPosition =
                    Mouse.current.position.ReadValue();

                Vector2 delta =
                    currentPosition - _lastMousePosition;

                Vector3 right = transform.right;

                Vector3 forward = Vector3.ProjectOnPlane(
                    transform.forward,
                    Vector3.up
                ).normalized;

                Vector3 movement =
                    (-right * delta.x - forward * delta.y)
                    * dragSpeed;

                Vector3 newPosition = transform.position + movement;

                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

                transform.position = newPosition;

                _lastMousePosition = currentPosition;
            }
        }

        private void HandleZoom()
        {
            if (Mouse.current == null)
                return;

            float scroll =
                Mouse.current.scroll.ReadValue().y;

            if (Mathf.Abs(scroll) < 0.01f)
                return;

            _cam.orthographicSize = Mathf.Clamp(
                _cam.orthographicSize - scroll * zoomSpeed,
                minZoom,
                maxZoom
            );
        }
    }
}