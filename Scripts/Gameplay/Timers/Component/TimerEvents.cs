using Jrd.Gameplay.Storage.Warehouse;
using Unity.Entities;

namespace Jrd.Gameplay.Timers.Component
{
    /// <summary>
    /// On timer finished event
    /// <see cref="MoveToWarehouseTimerFinishedSystem"/>
    /// </summary>
    public struct MoveToWarehouseTimerFinishedEvent : IComponentData
    {
    }

    public struct OneProductTimerFinishedEvent : IComponentData
    {
    }

    public struct AllProductsTimerFinishedEvent : IComponentData
    {
    }

    public struct AllLoadedProductsTimerFinishedEvent : IComponentData
    {
    }

    public struct OneLoadedProductsTimerFinishedEvent : IComponentData
    {
    }

    public struct ProductionTimersUpdatedEvent : IComponentData
    {
    }
}