using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jrd.GameStates.BuildingState.Prefabs
{
    [CreateAssetMenu(fileName = "New Building", menuName = "Building Data", order = 51)]
    public class BuildingDataSo : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private string _name;
        [SerializeField] private BuildingNameId _nameId;
        [SerializeField] private Vector2 _size;
        [SerializeField] private BuildingCategoryId _categoryId;
        [SerializeField] private List<BuildingRequiredItemsBuffer> _requiredItems;
        [SerializeField] private List<BuildingManufacturedItemsBuffer> _manufacturedItems;

        public GameObject Prefab => _prefab;
        public string Name => _name;
        public BuildingNameId NameId => _nameId;
        public Vector2 Size => _size;
        public BuildingCategoryId CategoryId => _categoryId;
        public List<BuildingRequiredItemsBuffer> RequiredItems => _requiredItems;
        public List<BuildingManufacturedItemsBuffer> ManufacturedItems => _manufacturedItems;

        private void OnValidate()
        {
            // TODO lol, google
            if (_prefab == null
                || _name == ""
                || _nameId == BuildingNameId.Default
                || _size == Vector2.zero
                || _categoryId == BuildingCategoryId.Default)
            {
                Debug.LogError("Fill all fields! In scriptable object ->" + this);
            }
        }
    }
}