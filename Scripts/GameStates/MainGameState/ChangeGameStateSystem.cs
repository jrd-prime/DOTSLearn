using Unity.Entities;

namespace Jrd.GameStates.MainGameState
{
    public partial struct ChangeGameStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ChangeGameStateComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            // var ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
            //     .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (gameStateData, newState, entity) in SystemAPI
                         .Query<RefRW<GameStateData>, RefRO<ChangeGameStateComponent>>()
                         .WithEntityAccess())
            {
                if (gameStateData.ValueRO.CurrentGameState == newState.ValueRO.GameState) return;

                gameStateData.ValueRW.CurrentGameState = newState.ValueRO.GameState;
            }
        }
    }
}