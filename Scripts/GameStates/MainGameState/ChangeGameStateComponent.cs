using Unity.Entities;

namespace Jrd.GameStates.MainGameState
{
    public struct ChangeGameStateComponent : IComponentData
    {
        public GameState GameState;
    }
}