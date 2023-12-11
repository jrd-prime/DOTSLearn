using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.Build.Screen
{
    /// <summary>
    /// Размер экрана / Координаты центра экрана
    /// </summary>

    public partial struct ScreenDataSystem : ISystem
    {
        private Entity _screenSingleton;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenComponent>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entity = ecb.CreateEntity();
            ecb.AddComponent<ScreenComponent>(entity);
            ecb.SetName(entity, "_ScreenComponentSingleton");
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            _screenSingleton = SystemAPI.GetSingletonEntity<ScreenComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var width = UnityEngine.Screen.width;
            var height = UnityEngine.Screen.height;

            // return if screen size not changed
            var screenSizes = SystemAPI.GetComponentRO<ScreenComponent>(_screenSingleton).ValueRO.WidthAndHeight;
            if (Utils.IsScreenSizeChanged(screenSizes.x, screenSizes.y)) return;

            var screenWidthAndHeight = new float2(width, height);
            var center = new float2(width / 2f, height / 2f);

            SystemAPI.SetComponent(_screenSingleton, new ScreenComponent
            {
                ScreenCenter = center,
                WidthAndHeight = screenWidthAndHeight
            });
        }
    }
}