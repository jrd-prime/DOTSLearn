using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SocialPlatforms;

namespace Jrd.Build
{
    public partial struct PlaceTempBuildPrefabSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenCenterToWorldComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var screenCenterToWorldComponent = SystemAPI.GetSingleton<ScreenCenterToWorldComponent>();

            // тут сопсна мы должны устанавливать данные для префаба
            foreach (var (buffer, prefab) in SystemAPI.Query<DynamicBuffer<PrefabBufferElements>>()
                         .WithAll<TempBuildPrefabComponent>().WithEntityAccess())
            {
                // LOOK гдет надо устанавливать префаб который хотим плэйсить
                
                
                // var a = prefab.ValueRO.TempPrefab;
                // state.EntityManager.SetComponentData(a, new LocalTransform
                // {
                //     Position = screenCenterToWorldComponent.ScreenCenterToWorld,
                //     Rotation = quaternion.identity,
                //     Scale = 1
                // });
            }
        }
    }
}

// foreach (var (mapBuffer, entity) in SystemAPI.Query<DynamicBuffer<MapElement>>().WithAll<MapEntities>()
//              .WithEntityAccess())
//         {
//             for (int i = 0; i < mapBuffer.Length; i++)
//             {
//                 state.EntityManager.Instantiate(mapBuffer[i].MapEntity);
//             }
//         }