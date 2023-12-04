using Unity.Mathematics;
using UnityEngine;

namespace Jrd.JCamera
{
    public class CameraSingleton : MonoBehaviour
    {
        public static CameraSingleton Instance;
        [SerializeField] private Vector3 _cursorInWorld;
        [SerializeField] private Camera _cameraObject;

        private CameraSingleton()
        {
        }

        public float Speed => Speed;
        public float3 CursorPosition => _cursorInWorld;
        public Camera Camera => _cameraObject;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
    }
}