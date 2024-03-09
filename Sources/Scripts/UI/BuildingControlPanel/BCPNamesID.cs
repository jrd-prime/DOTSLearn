namespace Sources.Scripts.UI.BuildingControlPanel
{
    /// <summary>
    /// Building Control Panel Constant Names ID's 
    /// </summary>
    public static class BCPNamesID
    {
        // Main
        public static string PanelId => "building-config-panel";
        public static string PanelTitleId => "panel-title";

        // Common
        public static string LevelLabelId => "lvl-label";

        // Buttons
        public static string CloseButtonIdName = "close-button";
        public static string UpgradeButtonId => "btn-upgrade";
        public static string BuffButtonId => "btn-buff";
        public static string MoveButtonId => "btn-move";
        public static string LoadButtonId => "btn-load";
        public static string TakeButtonId => "btn-take";

        // Production Line
        public static string ProdLineContainerId => "production-line-cont";
        public static string ProdLineItemContainerId => "prod-line-item-cont";
        public static string ProdLineItemCountLabelId => "prod-line-item-count";

        // Specs
        public static string SpecProductivityNameLabelId => "stat-conv-label";
        public static string SpecProductivityIntNameLabelId => "stat-conv-int-label";
        public static string SpecLoadCapacityNameLabelId => "stat-load-label";
        public static string SpecLoadCapacityIntNameLabelId => "stat-load-int-label";
        public static string SpecStorageCapacityNameLabelId => "stat-storage-label";
        public static string SpecStorageCapacityIntNameLabelId => "stat-storage-int-label";

        // Warehouse
        public static string WarehouseContainerId => "warehouse-cont";
        public static string WarehouseNameLabelId => "warehouse-label";

        // Main Storage
        public static string MainStorageContainerId => "main-storage-cont";
        public static string MainStorageNameLabelId => "storage-label";
        
        // Required box
        public static string RequiredBoxContainerId => "req-box-items-cont";
        public static string RequiredBoxNameLabelId  => "req-box-label";
        
        // Manufactured box
        public static string ManufacturedBoxContainerId => "man-box-items-cont";
        public static string ManufacturedBoxNameLabelId => "man-box-label";
        
        // Blueprints Shop
        // Blueprints Card
        
        public static string CardNamePrefix = "card-";
        public static string CardButtonNamePrefix = "building-";
    }
}