using Unity.Entities;

namespace Jrd.GameStates
{
    public partial struct SetStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SetStateComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (gameStateData, newState, entity) in SystemAPI
                         .Query<RefRW<GameStateData>, RefRO<SetStateComponent>>()
                         .WithEntityAccess())
            {
                gameStateData.ValueRW.GameState = newState.ValueRO._gameState;
                ecb.RemoveComponent<SetStateComponent>(entity);
            }
        }
    }
}