using Unity.Entities;

namespace GamePlay.GameStates.MainGameState
{
    public struct GameStateData : IComponentData
    {
        public Entity Self;
        public GameState CurrentGameState;
        public Entity BuildingStateEntity; //TODO подумать, переделать, мб список
    }
}