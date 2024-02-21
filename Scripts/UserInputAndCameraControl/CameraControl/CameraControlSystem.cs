using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace UserInputAndCameraControl.CameraControl
{
    public partial struct CameraControlSystem : ISystem
    {
        //TODO переделать
        private const float MinFOV = 30f;
        private const float MaxFOV = 70f;
        private const float ZoomSpeed = 20f;
        private const float RotationAngle = 30f;

        private float _previousZoomDirection;

        public void OnCreate(ref SystemState state)
        {
            _previousZoomDirection = 0;
        }

        public void OnUpdate(ref SystemState state)
        {
            var instance = CameraMono.Instance;
            if (instance == null) return;

            float dt = SystemAPI.Time.DeltaTime;
            GameObject cameraHolder = instance.CameraHolder;
            Camera camera = instance.Camera;

            foreach (var aspect in SystemAPI.Query<CameraAspect>())
            {
                if (Math.Abs(aspect.ZoomDirection - _previousZoomDirection) > float.Epsilon)
                {
                    _previousZoomDirection = aspect.ZoomDirection;
                    camera.fieldOfView =
                        Mathf.Clamp(camera.fieldOfView - aspect.ZoomDirection * aspect.ZoomSpeed, aspect.MinFOV,
                            aspect.MaxFOV);
                }

                if (Equals(aspect.Direction, (float3)Vector3.zero)) continue;

                // Compensate direction.
                Vector3 direction =
                    Quaternion.AngleAxis(cameraHolder.transform.eulerAngles.y, Vector3.up) * aspect.Direction;

                // Move camera holder.
                cameraHolder.transform.position += direction * dt * aspect.Speed;

                aspect.IsMoving = !Equals(aspect.Direction, (float3)Vector3.zero);
            }
        }
    }
}