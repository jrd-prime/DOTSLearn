using System;
using Jrd.GameStates.BuildingState.Prefabs.Goods;
using UnityEngine;

namespace Jrd.Goods
{
    [Serializable]
    public struct GoodsDataStruct
    {
        [SerializeField] private GoodsEnum _goodsAssetSo;
        [SerializeField] private int _count;

        public GoodsEnum Goods => _goodsAssetSo;
        public int Count => _count;
    }
}