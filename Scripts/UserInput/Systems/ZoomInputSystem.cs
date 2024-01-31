using Unity.Entities;
using UnityEngine;

namespace Jrd.UserInput
{
    public partial struct ZoomInputSystem : ISystem
    {
        private const string MouseScrollWheel = "Mouse ScrollWheel";

        private float _zoom;
        private const float YAxis = 0f;
        private float _zAxis;

        public void OnUpdate(ref SystemState state)
        {
#if UNITY_EDITOR
            _zoom = Input.GetAxis(MouseScrollWheel);
#endif

#if UNITY_ANDROID
            if (Input.touchCount == 2)
            {
                if (!Utils.Utils.IsPointerOverUIObject())
                {
                    Touch touch1 = Input.GetTouch(0);
                    Touch touch2 = Input.GetTouch(1);
                    int fingerId1 = touch1.fingerId;
                    int fingerId2 = touch2.fingerId;

                    Vector2 t1 = touch1.position - touch1.deltaPosition;
                    Vector2 t2 = touch2.position - touch2.deltaPosition;

                    float prevMag = (t1 - t2).magnitude;
                    float currMag = (touch1.position - touch2.position).magnitude;

                    _zoom = currMag - prevMag;
                }
            }
#endif

            foreach (var query in SystemAPI.Query<RefRW<ZoomDirectionData>>())
            {
                query.ValueRW.ZoomDirection = _zoom != 0 ? _zoom : 0;
            }
        }
    }
}