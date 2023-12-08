using Unity.Entities;
using UnityEngine;

namespace Jrd
{
    public struct BuildingPrefabComponent : IComponentData
    {
        public Entity Building1Prefab;
        public Entity Building2Prefab;
    }
}