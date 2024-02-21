using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

namespace Jrd
{
    public partial struct TestotempSystem : ISystem
    {
        private EntityQuery m_SceneSections;

        public void OnCreate(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            // var sceneSectionEntities = m_SceneSections.ToEntityArray(state.WorldUpdateAllocator);
            //
            // Debug.Log(sceneSectionEntities.Length);
            //
            //
            // foreach (var entity in sceneSectionEntities)
            // {
            //     Debug.Log("in");
            //     if (SceneSystem.IsSectionLoaded(state.WorldUnmanaged, entity))
            //     {
            //         Debug.Log("asd ");
            //     }
            // }
        }
    }
}