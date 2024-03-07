using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sources.Scripts.CommonData.SO
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Goods", menuName = "Add new Product Data", order = 52)]
    public class ProductDataScriptable : ScriptableObject
    {
        [SerializeField] private Sprite _productPrefabOrImageMb;
        [SerializeField] private string _productName;
        [SerializeField] private int _productPackSize;
         [SerializeField] private Product.Product _product;
        [SerializeField] [Range(0.5f, 2f)] private float _moveTimeMultiplier;

        public Sprite PrefabOrImageMb => _productPrefabOrImageMb;
        public string Name => _productName;
        public Product.Product Product => _product;
        public int Size => _productPackSize;
        public float MoveTimeMultiplier => _moveTimeMultiplier;
    }
}