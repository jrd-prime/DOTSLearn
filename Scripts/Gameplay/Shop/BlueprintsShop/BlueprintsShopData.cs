using Unity.Entities;

namespace Jrd.Gameplay.Shop.BlueprintsShop
{
    public struct BlueprintsShopData : IComponentData
    {
        public bool SetVisible;
        public int BuildingPrefabsCount;
    }
}