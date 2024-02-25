using Unity.Collections;

namespace GamePlay.Const
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


        public const string GoodsIconsPath = "UI/Images/icon-";
    }
}