using Unity.Entities;

namespace GamePlay.GameStates.MainGameState
{
    public struct ChangeGameStateComponent : IComponentData
    {
        public GameState GameState;
    }
}