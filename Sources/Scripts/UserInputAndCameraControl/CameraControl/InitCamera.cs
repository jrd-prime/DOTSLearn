using Sources.Scripts.UserInputAndCameraControl.UserInput;
using Sources.Scripts.UserInputAndCameraControl.UserInput.Components;
using Unity.Entities;

namespace Sources.Scripts.UserInputAndCameraControl.CameraControl
{
    [UpdateBefore(typeof(CameraControlSystem))]
    public partial struct InitCamera : ISystem
    {
        public static readonly string CameraEntityName = "___ Main Camera Entity";

        private const float CameraSpeed = 30f;
        private const float MinFOV = 30f;
        private const float MaxFOV = 70f;
        private const float ZoomSpeed = 20f;
        private const float RotationAngle = 30f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            
            EntityCommandBuffer biEcb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            EntityArchetype cameraArchetype = state.EntityManager.CreateArchetype(
                typeof(CameraData),
                typeof(MovableComponent),
                typeof(MoveDirectionData),
                typeof(ZoomDirectionData)
            );

            Entity entity = biEcb.CreateEntity(cameraArchetype);
            biEcb.SetName(entity, CameraEntityName);
            biEcb.SetComponent(entity, new MovableComponent { speed = CameraSpeed });
            biEcb.SetComponent(entity,
                new CameraData
                {
                    ZoomSpeed = ZoomSpeed,
                    MinFOV = MinFOV,
                    MaxFOV = MaxFOV,
                    RotationAngleY = RotationAngle
                });
        }
    }
}