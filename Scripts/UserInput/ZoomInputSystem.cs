using Jrd.DebSet;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.UserInput
{
    public partial struct ZoomInputSystem : ISystem
    {
        private const string MouseScrollWheel = "Mouse ScrollWheel";

        private float _zoom;
        private const float YAxis = 0f;
        private float _zAxis;
        private int _fingerId1;
        private int _fingerId2;

        public void OnUpdate(ref SystemState state)
        {
            // TODO сделать через состояния, когда можно зумить или нет
#if UNITY_EDITOR
            _zoom = Input.GetAxis(MouseScrollWheel);
#endif

#if UNITY_ANDROID
            if (Input.touchCount == 2)
            {
                if (!Utils.IsPointerOverUIObject())
                {
                    var touch1 = Input.GetTouch(0);
                    var touch2 = Input.GetTouch(1);
                    _fingerId1 = touch1.fingerId;
                    _fingerId2 = touch2.fingerId;

                    var t1 = touch1.position - touch1.deltaPosition;
                    var t2 = touch2.position - touch2.deltaPosition;

                    var prevMag = (t1 - t2).magnitude;
                    var currMag = (touch1.position - touch2.position).magnitude;

                     _zoom = currMag - prevMag;
                     DebSetUI.DebSetText.text = _zoom.ToString();
                }
            }
#endif

            foreach (var query in SystemAPI.Query<RefRW<ZoomingEventComponent>>())
            {
                query.ValueRW.zoom = _zoom != 0 ? _zoom : 0;
            }
        }
    }
}