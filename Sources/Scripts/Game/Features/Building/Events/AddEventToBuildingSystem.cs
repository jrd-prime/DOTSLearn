using Unity.Burst;
using Unity.Entities;

namespace GamePlay.Features.Building.Events
{
    [BurstCompile]
    public partial struct AddEventToBuildingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<AddEventToBuildingData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (aspect, eventData) in SystemAPI.Query<BuildingDataAspect, AddEventToBuildingData>())
            {
                aspect.BuildingData.BuildingEvents.Enqueue(eventData.Value);

                ecb.RemoveComponent<AddEventToBuildingData>(aspect.Self);
            }
        }
    }
}