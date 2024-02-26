using Unity.Entities;

namespace GamePlay.Features.Building
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