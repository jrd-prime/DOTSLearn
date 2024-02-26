using Unity.Entities;

namespace Sources.Scripts.Game.Features.Shop.BlueprintsShop
{
    public struct BlueprintsShopData : IComponentData
    {
        public bool SetVisible;
        public int BuildingPrefabsCount;
    }
}