using Unity.Entities;

namespace Jrd.GameStates
{
    public struct GameStateData : IComponentData
    {
        public Entity self;
        public GameState GameState;
    }
}