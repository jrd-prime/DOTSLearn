using Unity.Collections;
using Unity.Entities;

namespace Jrd.GameplayBuildings
{
    public partial struct GameBuildingsSystem : ISystem
    {
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

            var gameBuildingsData = SystemAPI.GetSingletonRW<GameBuildingsData>();

            if (!gameBuildingsData.ValueRO.GameBuildings.IsCreated)
            {
                gameBuildingsData.ValueRW.GameBuildings =
                    new NativeHashMap<FixedString64Bytes, BuildingData>(1, Allocator.Persistent);
            }
        }
    }
}