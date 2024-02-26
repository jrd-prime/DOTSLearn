using Unity.Collections;
using Unity.Entities;

namespace GamePlay.UI
{
    /// <summary>
    /// List UI Components to create entities for them
    /// </summary>
    public partial struct InitUISystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            NativeHashMap<FixedString64Bytes, ComponentType> componentsMap = new(0, Allocator.Temp);


            var entityManager = state.EntityManager;

            foreach (var pair in componentsMap)
            {
                var elementEntity = entityManager.CreateEntity(pair.Value, typeof(UIElementTag));
                entityManager.SetName(elementEntity, pair.Key);

                // add test data to main storage
                // if (pair.Key != MainStorageDataName) continue;
                // entityManager.AddComponent<UpdateRequestTag>(elementEntity);
                
            }

            componentsMap.Dispose();
        }
    }
}