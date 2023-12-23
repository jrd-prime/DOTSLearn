﻿using Jrd.UserInput;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.JCamera
{
    public partial struct CameraFollowSystem : ISystem
    {
        private Vector3 _lastPosition;

        public void OnUpdate(ref SystemState state)
        {
            // TODO camera jump fix
            foreach (var follow in SystemAPI
                         .Query<RefRO<FollowComponent>>()
                         .WithAll<CameraComponent>())
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