using Unity.Entities;

namespace Sources.Scripts.CommonComponents.Building
{
    public struct BuildingTag : IComponentData
    {
    }

    /// <summary>
    /// Selectable entity
    /// </summary>
    public struct SelectableBuildingTag : IComponentData
    {
    }

    public struct SelectedBuildingTag : IComponentData
    {
    }
}