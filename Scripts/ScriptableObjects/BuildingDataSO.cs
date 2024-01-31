using System.Collections.Generic;
using Jrd.Goods;
using Unity.Collections;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.Prefabs
{
    [CreateAssetMenu(fileName = "New Building", menuName = "Building Data", order = 51)]
    public class BuildingDataSo : ScriptableObject
    {
        [SerializeField] private GameObject _buildingPrefab;
        [SerializeField] private string _buildingName;
        [SerializeField] private Vector2 _buildingSize;
        [SerializeField] private BuildingCategory _buildingCategory;
        [SerializeField] private NativeList<GoodsDataStruct> _requiredProducts;
        [SerializeField] private NativeList<GoodsDataStruct> _manufacturedProducts;

        public GameObject Prefab => _buildingPrefab;
        public string Name => _buildingName;
        public Vector2 Size => _buildingSize;
        public BuildingCategory Category => _buildingCategory;
        public NativeList<GoodsDataStruct> Required => _requiredProducts;
        public NativeList<GoodsDataStruct> Manufactured => _manufacturedProducts;
    }
}