using Unity.Entities;

namespace Sources.Scripts.CommonData.Building
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
    public struct DestroyTempBlueprintTag : IComponentData
    {
    }
}