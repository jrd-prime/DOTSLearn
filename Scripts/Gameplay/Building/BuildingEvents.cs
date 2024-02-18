namespace Jrd.Gameplay.Building
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