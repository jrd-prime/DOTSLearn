using Unity.Entities;

namespace Jrd.Build
{
    public partial struct PlaceTempBuildPrefabSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var b in SystemAPI.Query<RefRO<TempBuildPrefabComponent>>())
            {
            }
        }
    }
}