using Jrd.Gameplay.Building.ControlPanel;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Building
{
    public partial struct GameBuildingsSystem : ISystem
    {
        private RefRW<GameBuildingsData> _gameBuildingsData;

        public void OnCreate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var e = entityManager.CreateEntity();
            entityManager.AddComponent<GameBuildingsData>(e);
            entityManager.SetName(e, "___ GAME BUILDINGS");
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            _gameBuildingsData = SystemAPI.GetSingletonRW<GameBuildingsData>();

            if (!_gameBuildingsData.ValueRO.GameBuildings.IsCreated)
            {
                _gameBuildingsData.ValueRW.GameBuildings =
                    new NativeHashMap<FixedString64Bytes, BuildingData>(1, Allocator.Persistent);
            }
        }

        public void OnDestroy(ref SystemState state)
        {
            _gameBuildingsData = SystemAPI.GetSingletonRW<GameBuildingsData>();
            _gameBuildingsData.ValueRW.GameBuildings.Dispose();
        }
    }
}