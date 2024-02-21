using Unity.Entities;

namespace GamePlay.Shop.BlueprintsShop
{
    public struct BlueprintsShopData : IComponentData
    {
        public bool SetVisible;
        public int BuildingPrefabsCount;
    }
}