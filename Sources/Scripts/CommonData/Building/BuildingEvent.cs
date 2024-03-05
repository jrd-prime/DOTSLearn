namespace Sources.Scripts.CommonData.Building
{
    public enum BuildingEvent
    {
        MoveToWarehouse_Timer_Started,
        MoveToWarehouse_Timer_Finished,
        MoveToProductionBox_Finished,
        MainStorageBox_Updated,
        WarehouseBox_Updated,
        InProductionBox_Updated,
        ManufacturedBox_Updated,
        Production_OneLoadCycle_Finished,
        Production_FullLoadCycle_Finished,
        Production_Timers_Started,
        Production_Timers_InProgressUpdate
    }
}