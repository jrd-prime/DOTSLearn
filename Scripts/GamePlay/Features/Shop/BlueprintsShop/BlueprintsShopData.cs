using Unity.Entities;

namespace GamePlay.Features.Shop.BlueprintsShop
{
    public struct BlueprintsShopData : IComponentData
    {
        public bool SetVisible;
        public int BuildingPrefabsCount;
    }
}