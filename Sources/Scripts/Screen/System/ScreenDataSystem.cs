using Sources.Scripts.CommonComponents;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Sources.Scripts.Screen.System
{
    /// <summary>
    /// Get screen sizes and set it to <see cref="ScreenData"/> component
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(JDefaultSimulationSystemGroup))]
    public partial struct ScreenDataSystem : ISystem
    {
        private static readonly FixedString64Bytes ScreenDataName = "___ # Screen Data";
        private static readonly FixedString64Bytes ScreenCenterInWorldDataName = "___ # Screen Center In World Data";

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenData>();

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var screenDataEntity = ecb.CreateEntity();
            ecb.AddComponent<ScreenData>(screenDataEntity);
            ecb.SetName(screenDataEntity, ScreenDataName);

            var screenCenterInWorldCoordsEntity = ecb.CreateEntity();
            ecb.AddComponent<ScreenCenterInWorldCoordsData>(screenCenterInWorldCoordsEntity);
            ecb.SetName(screenCenterInWorldCoordsEntity, ScreenCenterInWorldDataName);

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var screen in SystemAPI.Query<RefRW<ScreenData>>())
            {
                float2 screenSize = screen.ValueRO.WidthAndHeight;

                if (Utility.Utils.IsScreenSizeChanged(screenSize, out float2 newScreenSize)) continue;

                screen.ValueRW.SetWidthAndHeight(newScreenSize);
            }
        }
    }
}