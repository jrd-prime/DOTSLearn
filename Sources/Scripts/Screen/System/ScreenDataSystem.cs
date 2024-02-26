using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Sources.Scripts.Screen.System
{
    /// <summary>
    /// Размер экрана / Координаты центра экрана
    /// </summary>
    [BurstCompile]
    public partial struct ScreenDataSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenData>();

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var screenEntity = ecb.CreateEntity();
            ecb.AddComponent<ScreenData>(screenEntity);
            ecb.SetName(screenEntity, "___ Screen Component");

            var screenCenterInWorldCoordsEntity = ecb.CreateEntity();
            ecb.AddComponent<ScreenCenterInWorldCoordsData>(screenCenterInWorldCoordsEntity);
            ecb.SetName(screenCenterInWorldCoordsEntity, "___ # Screen Center To World");

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var screenComponent in SystemAPI.Query<RefRW<ScreenData>>())
            {
                // continue if screen size not changed
                var widthAndHeight = screenComponent.ValueRO.WidthAndHeight;
                if (Utility.Utils.IsScreenSizeChanged(widthAndHeight.x, widthAndHeight.y)) continue;

                var width = UnityEngine.Screen.width;
                var height = UnityEngine.Screen.height;

                screenComponent.ValueRW.ScreenCenter = new float2(width / 2f, height / 2f);
                screenComponent.ValueRW.WidthAndHeight = new float2(width, height);
            }
        }
    }
}