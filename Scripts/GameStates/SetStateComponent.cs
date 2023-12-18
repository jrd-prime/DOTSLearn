using Unity.Entities;

namespace Jrd.GameStates
{
    public struct SetStateComponent : IComponentData
    {
        public GameState _gameState;
    }
}