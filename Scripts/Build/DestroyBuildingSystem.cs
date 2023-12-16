using Jrd.Build.old;
using Jrd.Build.Screen;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.Build
{
    public partial struct DestroyBuildingSystem : ISystem
    {
        private EntityCommandBuffer _ecb;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ScreenCenterInWorldCoordsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            // Destroy temp prefab if we exit from edit mode
        }
    }
}