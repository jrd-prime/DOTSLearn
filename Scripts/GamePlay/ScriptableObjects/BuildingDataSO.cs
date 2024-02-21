using System;
using System.Collections.Generic;
using GamePlay.Building.Prefabs;
using GamePlay.Building.SetUp;
using UnityEngine;

namespace Jrd.ScriptableObjects
{
    //LOOK try [SerializeReference]

    [CreateAssetMenu(fileName = "New Building", menuName = "Building Data", order = 51)]
    public class BuildingDataSo : ScriptableObject
    {
        [Header("Building Prefab Info")] [SerializeField]
        private GameObject _prefab;

        [SerializeField] private BuildingCategoryId _categoryId;
        [SerializeField] private BuildingNameId _nameId;
        [SerializeField] private string _name;
        [SerializeField] private Vector2 _size;

        [SerializeField] private List<ProductForProduction> _requiredItems;
        [SerializeField] private List<ProductForProduction> _manufacturedItems;


        [Header("Building Stats")] [SerializeField]
        private int _level = 1;

        [SerializeField] private float _itemsPerHour;
        [SerializeField] private int _loadCapacity;
        [SerializeField] private int _storageCapacity = 20;

        public GameObject Prefab => _prefab;
        public string Name => _name;
        public BuildingNameId NameId => _nameId;
        public Vector2 Size => _size;
        public BuildingCategoryId CategoryId => _categoryId;
        public List<ProductForProduction> RequiredItems => _requiredItems;
        public List<ProductForProduction> ManufacturedItems => _manufacturedItems;
        public int Level => _level;
        public float ItemsPerHour => _itemsPerHour;
        public int LoadCapacity => _loadCapacity;
        public int StorageCapacity => _storageCapacity;

        private void OnValidate()
        {
            // TODO lol, google
            if (_prefab == null
                || _name == ""
                || _nameId == BuildingNameId.Default
                || _size == Vector2.zero
                || _categoryId == BuildingCategoryId.Default
                || _itemsPerHour == 0
                || _loadCapacity == 0)
            {
                Debug.LogError("Fill all fields! In scriptable object ->" + this);
            }
        }
    }

    [Serializable]
    public struct ProductForProduction
    {
        public ProductDataSO _productDataSo;
        public int _quantity;
    }
}