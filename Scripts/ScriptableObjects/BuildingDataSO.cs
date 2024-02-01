using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jrd.GameStates.BuildingState.Prefabs
{
    [CreateAssetMenu(fileName = "New Building", menuName = "Building Data", order = 51)]
    public class BuildingDataSo : ScriptableObject
    {
        [SerializeField] private GameObject _buildingPrefab;
        [SerializeField] private string _buildingName;
        [SerializeField] private Vector2 _buildingSize;
        [SerializeField] private BuildingCategory _buildingCategory;
        [SerializeField] private List<BuildingRequiredItemsBuffer> _requiredItemsProducts;
        [FormerlySerializedAs("_manufacturedProducts")] [SerializeField] private List<BuildingManufacturedItemsBuffer> _manufacturedItemsProducts;

        public GameObject Prefab => _buildingPrefab;
        public string Name => _buildingName;
        public Vector2 Size => _buildingSize;
        public BuildingCategory Category => _buildingCategory;
        public List<BuildingRequiredItemsBuffer> RequiredItems => _requiredItemsProducts;
        public List<BuildingManufacturedItemsBuffer> ManufacturedItems => _manufacturedItemsProducts;

        private void OnValidate()
        {
            // Debug.Log(Required.Count);
        }
    }
}