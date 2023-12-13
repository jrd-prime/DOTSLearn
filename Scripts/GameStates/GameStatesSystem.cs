using Unity.Entities;

namespace Jrd.GameStates
{
    public partial struct GameStatesSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var em = state.EntityManager;
            var gameStateEntity = em.CreateEntity();
            em.SetName(gameStateEntity, "_GameStateEntity");

            em.AddComponent<GameStateData>(state.SystemHandle);
            em.SetComponentData(state.SystemHandle, new GameStateData
            {
                GameStateEntity = gameStateEntity
            });
        }
    }
}