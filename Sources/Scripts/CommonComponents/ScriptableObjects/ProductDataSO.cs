﻿using System;
using UnityEngine;

namespace Sources.Scripts.CommonComponents.ScriptableObjects
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Goods", menuName = "Goods Data", order = 52)]
    public class ProductDataSO : ScriptableObject
    {
        [SerializeField] private Sprite _productPrefabOrImageMb;
        [SerializeField] private string _productName;
        [SerializeField] private int _productPackSize;
        [SerializeField] private Product.Product _goodsCategory;
        [SerializeField] [Range(0.5f, 2f)] private float _moveTimeMultiplier;

        public Sprite PrefabOrImageMb => _productPrefabOrImageMb;
        public string Name => _productName;
        public Product.Product Product => _goodsCategory;
        public int Size => _productPackSize;
        public float MoveTimeMultiplier => _moveTimeMultiplier;

        // public string Guid { get; private set; }
        //
        // private void OnValidate()
        // {
        //     Guid = Utils.Utils.GetGuid();
        // }
    }
}