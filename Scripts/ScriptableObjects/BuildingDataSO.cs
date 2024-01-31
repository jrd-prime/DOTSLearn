using System;
using System.Collections.Generic;
using Jrd.GameStates.BuildingState.Prefabs.Goods;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.Prefabs
{
    [CreateAssetMenu(fileName = "New Building", menuName = "Building Data", order = 51)]
    public class BuildingDataSO : ScriptableObject
    {
        [SerializeField] private GameObject _buildingPrefab;
        [SerializeField] private string _buildingName;
        [SerializeField] private Vector2 _buildingSize;
        [SerializeField] private BuildingCategory _buildingCategory;

        [SerializeField] private List<ProductWithRequiredCount> _requiredProducts;
        [SerializeField] private List<ProductWithRequiredCount> _manufacturedProducts;

        public GameObject Prefab => _buildingPrefab;
        public string Name => _buildingName;
        public Vector2 Size => _buildingSize;
        public BuildingCategory Category => _buildingCategory;
    }

    [Serializable]
    internal struct ProductWithRequiredCount
    {
        public int _count;
        public GoodsDataSO _product;
    }
}