using Unity.Entities;

namespace Jrd.Gameplay.Timers.Component
{
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