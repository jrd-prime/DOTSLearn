using Unity.Entities;

namespace Jrd.Gameplay.Building.TempBuilding
{
    /// <summary>
    /// A tag indicating that it is a temporary building
    /// </summary>
    public struct TempBuildingTag : IComponentData
    {
    }

    /// <summary>
    /// Tag for prefab placement
    /// </summary>
    public struct PlaceTempBuildingTag : IComponentData
    {
    }

    /// <summary>
    /// Tag to destroy prefab entity
    /// </summary>
    public struct DestroyTempPrefabTag : IComponentData
    {
    }
}