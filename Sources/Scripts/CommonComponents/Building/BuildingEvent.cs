namespace Sources.Scripts.CommonComponents.Building
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