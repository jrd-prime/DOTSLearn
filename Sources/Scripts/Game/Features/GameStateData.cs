﻿using Unity.Entities;

namespace Sources.Scripts.Game.Features
{
    public struct GameStateData : IComponentData
    {
        public Entity Self;
        public GameState CurrentGameState;
        public Entity BuildingStateEntity; //TODO подумать, переделать, мб список
    }

    public enum GameState
    {
        GamePlayState,
        BuildingState
    }
}