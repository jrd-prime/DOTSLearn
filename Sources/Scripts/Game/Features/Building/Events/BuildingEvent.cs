namespace Sources.Scripts.Game.Features.Building.Events
{
    public enum BuildingEvent
    {
        MoveToWarehouseTimerStarted,
        MoveToWarehouseTimerFinished,
        MoveToProductionBoxFinished,
        MainStorageDataUpdated,
        WarehouseDataUpdated,
        InProductionBoxDataUpdated,
        ManufacturedBoxDataUpdated,
        OneLoadCycleFinished,
        FullLoadCycleFinished,
        ProductionTimersStarted,
        ProductionTimersInProgressUpdate
    }
}