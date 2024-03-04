using Sources.Scripts.CommonComponents;
using Sources.Scripts.CommonComponents.Building;
using Unity.Burst;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Events.System
{
    [BurstCompile]
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial struct AddEventToBuildingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<AddEventToBuildingData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // not IEntityJob because nested aspect :( mb unsafe
            foreach (var (aspect, addEvent) in SystemAPI.Query<BuildingDataAspect, AddEventToBuildingData>())
            {
                aspect.BuildingData.BuildingEvents.Enqueue(addEvent.Value);

                SystemAPI
                    .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged)
                    .RemoveComponent<AddEventToBuildingData>(aspect.Self);
            }
        }
    }
}