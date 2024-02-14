using Jrd.Gameplay.Building.Production.Component;
using Unity.Entities;

namespace Jrd.Gameplay.Building.Production
{
    public partial struct StartProductionSystem : ISystem
    {
        private EntityCommandBuffer _ecb;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var aspect in SystemAPI.Query<BuildingDataAspect>().WithAll<StartProductionTag>())
            {
                
                aspect.SetProductionState(ProductionState.Started);
                _ecb.RemoveComponent<StartProductionTag>(aspect.Self);
            }
        }
    }
}