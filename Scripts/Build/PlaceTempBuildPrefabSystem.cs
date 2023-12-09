using Jrd.Build.Screen;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
            foreach (var prefab in SystemAPI.Query<RefRO<TempBuildPrefabComponent>>())
            {
                // LOOK гдет надо устанавливать префаб который хотим плэйсить
                // типа надо гдето выбирать конкретный префаб, создавать компонент, который тут будем ловить
                // и сэтить в него префаб, а тут только плэйс

                var tempPrefab = prefab.ValueRO.tempBuildPrefab;

                if (tempPrefab != Entity.Null)
                {
                    var instantiate = state.EntityManager.Instantiate(prefab.ValueRO.tempBuildPrefab);
                    state.EntityManager.SetComponentData(instantiate, new LocalTransform
                    {
                        Position = new float3(3, 0, 3),
                        Rotation = quaternion.identity,
                        Scale = 1
                    });
                }


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