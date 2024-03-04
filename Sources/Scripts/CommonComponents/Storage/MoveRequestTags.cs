using Unity.Entities;

namespace Sources.Scripts.CommonComponents.Storage
{
    public struct MoveToMainStorageRequestTag : IComponentData
    {
    }

    public struct MoveToProductionBoxRequestTag : IComponentData
    {
    }

    /// <summary>
    /// Move products to warehouse
    /// <see cref="System.MoveToWarehouseRequestSystem"/>
    /// </summary>
    public struct MoveToWarehouseRequestTag : IComponentData
    {
    }

    public class ProductsToMainStorageRequestTag : IComponentData
    {
    }

    public class ProductsToManufacturedBoxRequestTag : IComponentData
    {
    }
}