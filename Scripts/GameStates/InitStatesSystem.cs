using Jrd.GameStates.BuildingState;
using Jrd.GameStates.MainGameState;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.GameStates
{
    public partial struct InitStatesSystem : ISystem
    {
        private static readonly FixedString64Bytes GameStateDataEntityName = "___ Game State";
        private static readonly FixedString64Bytes GameplayStateDataEntityName = "___ Gameplay State Data";
        private static readonly FixedString64Bytes BuildingStateDataEntityName = "___ Building State Data";

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            NativeHashMap<FixedString64Bytes, ComponentType> componentsMap = new(1, Allocator.Temp)
            {
                { GameStateDataEntityName, typeof(GameStateData) },
                { GameplayStateDataEntityName, typeof(PlayStateData) },
                { BuildingStateDataEntityName, typeof(BuildingStateData) }
            };

            var entityManager = state.EntityManager;

            foreach (var pair in componentsMap)
            {
                var elementEntity = entityManager.CreateEntity(pair.Value);
                entityManager.SetName(elementEntity, pair.Key);
            }

            componentsMap.Dispose();
        }
    }
}