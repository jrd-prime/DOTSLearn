using Unity.Mathematics;
using UnityEngine;

namespace UserInputAndCameraControl.CameraControl
{
    // TODO переделать камеру на синемашин
    // TODO настройки камеры сделать
    public class CameraMono : MonoBehaviour
    {
        public static CameraMono Instance;
        [SerializeField] private float3 _cameraOffset;

        private CameraMono()
        {
        }

        public float Speed => Speed;
        public Camera Camera { get; private set; }
        public GameObject CameraHolder { get; private set; }

        public float3 CameraOffset => _cameraOffset;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _cameraOffset = new float3(0, 10, -10);
            Camera = GetComponent<Camera>();

            // later
            try
            {
                CameraHolder = gameObject.transform.parent.gameObject;
            }
            catch
            {
                Debug.LogError("Camera must be child of empty object");
            }
        }
    }
}