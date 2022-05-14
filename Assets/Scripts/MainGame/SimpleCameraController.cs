using UnityEngine;

namespace MainGame
{
    public class SimpleCameraController : MonoBehaviour
    {
        private bool isGameDone;

        public void ChangeEndGameFlag() => isGameDone = !isGameDone;

        private class CameraState
        {
            public float yaw;
            public float pitch;
            public float roll;
            private float x;
            private float y;
            private float z;

            public void SetFromTransform(Transform t)
            {
                pitch = t.eulerAngles.x;
                yaw = t.eulerAngles.y;
                roll = t.eulerAngles.z;
                x = t.position.x;
                y = t.position.y;
                z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                var rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

                x += rotatedTranslation.x;
                y += rotatedTranslation.y;
                z += rotatedTranslation.z;
            }

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
                roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

                x = Mathf.Lerp(x, target.x, positionLerpPct);
                y = Mathf.Lerp(y, target.y, positionLerpPct);
                z = Mathf.Lerp(z, target.z, positionLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(pitch, yaw, roll);
                t.position = new Vector3(x, y, z);
            }
        }

        readonly CameraState m_TargetCameraState = new CameraState();
        readonly CameraState m_InterpolatingCameraState = new CameraState();

        [Header("Movement Settings")] [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        [SerializeField] private float baseSpeed = 3.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        [SerializeField] private float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        [SerializeField] private AnimationCurve mouseSensitivityCurve =
            new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        [SerializeField] private float rotationLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        [SerializeField] private bool invertY = false;

        [SerializeField] private float shiftBoostSpeed;

        private void OnEnable()
        {
            m_TargetCameraState.SetFromTransform(transform);
            m_InterpolatingCameraState.SetFromTransform(transform);
        }

        private Vector3 GetInputTranslationDirection()
        {
            var direction = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }

            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }

            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }

            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.down;
            }

            if (Input.GetKey(KeyCode.E))
            {
                direction += Vector3.up;
            }

            return direction;
        }

        private void FixedUpdate()
        {
        
            // Hide and lock cursor when right mouse button pressed
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            // Unlock and show cursor when right mouse button released
            if (Input.GetMouseButtonUp(1))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            // Rotation
            if (Input.GetMouseButton(1))
            {
                var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));

                var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            }
        
        

            // Translation
            var translation = GetInputTranslationDirection() * Time.deltaTime;

            // Speed up movement when shift key held
            if (Input.GetKey(KeyCode.LeftShift))
            {
                translation *= shiftBoostSpeed;
            }

            translation *= Mathf.Pow(2.0f, baseSpeed);

            m_TargetCameraState.Translate(translation);

            // Framerate-independent interpolation
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

            m_InterpolatingCameraState.UpdateTransform(transform);
        }
    }
}