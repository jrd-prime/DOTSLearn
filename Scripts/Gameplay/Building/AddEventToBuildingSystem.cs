using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Building
{
    public partial struct AddEventToBuildingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<AddEventToBuildingData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (aspect, eventToAdd) in SystemAPI.Query<BuildingDataAspect, AddEventToBuildingData>())
            {
                aspect.BuildingData.BuildingEvents.Add(eventToAdd.Value);

                Debug.LogWarning($"event added = " + eventToAdd.Value);

                ecb.RemoveComponent<AddEventToBuildingData>(aspect.Self);
            }
        }
    }
}