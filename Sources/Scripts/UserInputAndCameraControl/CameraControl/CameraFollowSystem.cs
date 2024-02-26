using Sources.Scripts.UserInputAndCameraControl.UserInput;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Sources.Scripts.UserInputAndCameraControl.CameraControl
{
    public partial struct CameraFollowSystem : ISystem
    {
        private Vector3 _lastPosition;


        //     // set camera to follow this entity
        //     if (SystemAPI.TryGetSingletonEntity<CameraComponent>(out var e))
        // {
        //     ecb.AddComponent(e, new FollowComponent { Target = tempPrefabEntity });
        //     ecb.RemoveComponent<MovingEventComponent>(e);
        // }


        public void OnUpdate(ref SystemState state)
        {
            // TODO camera jump fix
            foreach (var follow in SystemAPI
                         .Query<RefRO<FollowComponent>>()
                         .WithAll<CameraData>())
            {
                // Debug.Log("// follow movement");
                var instance = CameraMono.Instance;
                if (instance == null) continue;

                var targetPosition = SystemAPI
                    .GetComponent<LocalTransform>(follow.ValueRO.Target)
                    .Position;

                if ((Vector3)targetPosition == _lastPosition) continue;

                _lastPosition = targetPosition;
                instance.CameraHolder.transform.position = targetPosition;
            }
        }
    }
}