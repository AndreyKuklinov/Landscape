using UnityEngine;

namespace MainGame
{
    public class SimpleCameraController : MonoBehaviour
    {
        private class CameraState
        {
            public float Yaw;
            public float Pitch;
            public float Roll;
            private float X;
            private float Y;
            private float Z;
            private float minYPosition = 0.5f;

            public void SetFromTransform(Transform t)
            {
                Pitch = t.eulerAngles.x;
                Yaw = t.eulerAngles.y;
                Roll = t.eulerAngles.z;
                X = t.position.x;
                Y = t.position.y;
                Z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                var rotatedTranslation = Quaternion.Euler(Pitch, Yaw, Roll) * translation;

                X += rotatedTranslation.x;
                if (Y + rotatedTranslation.y > minYPosition || rotatedTranslation.y > 0)
                    Y += rotatedTranslation.y;
                Z += rotatedTranslation.z;
            }

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                Yaw = Mathf.Lerp(Yaw, target.Yaw, rotationLerpPct);
                Pitch = Mathf.Lerp(Pitch, target.Pitch, rotationLerpPct);
                Roll = Mathf.Lerp(Roll, target.Roll, rotationLerpPct);

                X = Mathf.Lerp(X, target.X, positionLerpPct);
                Y = Mathf.Lerp(Y, target.Y, positionLerpPct);
                Z = Mathf.Lerp(Z, target.Z, positionLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(Pitch, Yaw, Roll);
                t.position = new Vector3(X, Y, Z);
            }
        }

        readonly CameraState m_TargetCameraState = new CameraState();
        readonly CameraState m_InterpolatingCameraState = new CameraState();

        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        [SerializeField]
        private float baseSpeed = 3.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        [SerializeField]
        private float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        [SerializeField]
        private AnimationCurve mouseSensitivityCurve =
            new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        [SerializeField]
        private float rotationLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")] [SerializeField]
        private bool invertY;

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
                direction += Vector3.forward;

            if (Input.GetKey(KeyCode.S))
                direction += Vector3.back;

            if (Input.GetKey(KeyCode.A))
                direction += Vector3.left;

            if (Input.GetKey(KeyCode.D))
                direction += Vector3.right;

            if (Input.GetKey(KeyCode.Q))
                direction += Vector3.down;

            if (Input.GetKey(KeyCode.E))
                direction += Vector3.up;

            return direction;
        }

        private void FixedUpdate()
        {
            if (Input.GetMouseButtonDown(1))
                Cursor.lockState = CursorLockMode.Locked;

            if (Input.GetMouseButtonUp(1))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if (Input.GetMouseButton(1))
            {
                var mouseMovement =
                    new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));

                var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                m_TargetCameraState.Yaw += mouseMovement.x * mouseSensitivityFactor;
                m_TargetCameraState.Pitch += mouseMovement.y * mouseSensitivityFactor;
            }

            var translation = GetInputTranslationDirection() * Time.deltaTime;

            if (Input.GetKey(KeyCode.LeftShift))
                translation *= shiftBoostSpeed;

            translation *= Mathf.Pow(2.0f, baseSpeed);

            m_TargetCameraState.Translate(translation);

            var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

            m_InterpolatingCameraState.UpdateTransform(transform);
        }
    }
}