using Unity.Entities;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.TempBuilding
{
    public partial struct PlaceTempBuildingSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (buildingData, entity) in SystemAPI
                         .Query<RefRW<BuildingData>>()
                         .WithAll<PlaceTempBuildingTag, TempBuildingTag>()
                         .WithEntityAccess())
            {
                Debug.LogWarning("in " + this);
                // var a = SystemAPI.GetSingletonRW<GameData>();
            }
        }
    }
}