using Unity.Entities;

namespace Jrd.GameStates
{
    public struct GameStateData : IComponentData
    {
        public Entity Self;
        public GameState GameState;
        public Entity BuildingStateEntity; //TODO подумать, переделать, мб список
    }
}