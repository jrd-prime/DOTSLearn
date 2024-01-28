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
        

        public GameObject Prefab => _buildingPrefab;
        public string Name => _buildingName;
        public Vector2 Size => _buildingSize;
        public BuildingCategory Category => _buildingCategory;

    }
}