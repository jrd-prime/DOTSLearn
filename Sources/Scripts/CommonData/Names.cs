using Unity.Collections;

namespace Sources.Scripts.CommonData
{
    public class Names
    {
        public static int BuildingNameMaxStringLength = 30;

        // Data entities names
        public static readonly FixedString64Bytes GameStateDataEntityName = "___ Game State";
        public static readonly FixedString64Bytes GameplayStateDataEntityName = "___ Data: Gameplay State";
        public static readonly FixedString64Bytes BuildingStateDataEntityName = "___ Data: Building State";
        public static readonly FixedString64Bytes BlueprintsShopDataName = "___ Data: Blueprints Shop";
        public static readonly FixedString64Bytes MainStorageDataName = "___ Data: Main Storage";


        public static readonly string GoodsIconsPath = "UI/Images/icon-";

        // Production timers UI names
        public const string ProductionFullTimerContainerName = "all-timer-cont";
        public static readonly string ProductionOneTimerContainerName = "one-timer-cont";
        public static readonly string ProductionTimersCycleContainerName = "cycle-cont";
        public static readonly string ProductionTimersBothProgressBarName = "pb-bar";
        public static readonly string ProductionTimersBothLabelsName = "text-label";


        // Blueprints Shop
        public static readonly string CardsContainerIdName = "groupbox";
        // Card
        public static readonly string CardHeadName = "head-text";
        public static readonly string CardImageName = "img";
        public static readonly string CardButtonName = "btn";
    }
}