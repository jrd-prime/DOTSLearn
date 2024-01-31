using Jrd.Goods;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.Prefabs.Goods
{
    [CreateAssetMenu(fileName = "New Goods", menuName = "Goods Data", order = 52)]
    public class GoodsDataSO : ScriptableObject
    {
        [SerializeField] private GameObject productPrefabOrImageMB;
        [SerializeField] private string _productName;
        [SerializeField] private int _productPackSize;
        [SerializeField] private GoodsEnum _goodsCategory;

        public GameObject PrefabOrImageMb => productPrefabOrImageMB;
        public string Name => _productName;
        public int Size => _productPackSize;
    }
}