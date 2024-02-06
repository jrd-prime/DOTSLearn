using Jrd.Gameplay.Product;
using UnityEngine;

namespace Jrd.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Goods", menuName = "Goods Data", order = 52)]
    public class GoodsDataSO : ScriptableObject
    {
        [SerializeField] private Sprite _productPrefabOrImageMb;
        [SerializeField] private string _productName;
        [SerializeField] private int _productPackSize;
        [SerializeField] private Product _goodsCategory;

        public Sprite PrefabOrImageMb => _productPrefabOrImageMb;
        public string Name => _productName;
        public int Size => _productPackSize;

        // public string Guid { get; private set; }
        //
        // private void OnValidate()
        // {
        //     Guid = Utils.Utils.GetGuid();
        // }
    }
}