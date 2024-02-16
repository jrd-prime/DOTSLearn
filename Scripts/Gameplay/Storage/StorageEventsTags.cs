using Unity.Entities;

namespace Jrd.Gameplay.Storage
{
    /// <summary>
    /// Data updated in Main Storage
    /// <para>Do stuff wit UI or etc.</para>
    /// </summary>
    public struct MainStorageDataUpdatedEvent : IComponentData
    {
    }

    /// <summary>
    /// Data updated in Warehouse
    /// <para>Do stuff wit UI or etc.</para>
    /// </summary>
    public struct WarehouseDataUpdatedEvent : IComponentData
    {
    }

    /// <summary>
    /// Data updated in In Production Box
    /// <para>Do stuff wit UI or etc.</para>
    /// </summary>
    public struct InProductionDataUpdatedEvent : IComponentData
    {
    }

    /// <summary>
    /// Data updated in Manufactured Box
    /// <para>Do stuff wit UI or etc.</para>
    /// </summary>
    public struct ManufacturedDataUpdatedEvent : IComponentData
    {
    }
}