using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Storage.Data;
using Unity.Entities;
using Random = System.Random;

namespace Sources.Scripts.Dev
{
    public partial class DevSystem : SystemBase
    {
        private MainStorageBoxData _mainStorageBoxData;

        private DevUI _devUI;

        protected override void OnCreate()
        {
            RequireForUpdate<MainStorageBoxData>();
        }

        protected override void OnStartRunning()
        {
            _devUI = DevUI.Instance;

            _devUI.AddRndProdsToMainStorageButton.clicked += OnAddRndProdsToMainStorageButtonClicked;
        }

        private void OnAddRndProdsToMainStorageButtonClicked()
        {
            var rnd = new Random();

            foreach (var product in _mainStorageBoxData.Value)
            {
                product.Value += rnd.Next(3, 20);
            }

            foreach (var aspect in SystemAPI.Query<BuildingDataAspect>().WithAll<SelectedBuildingTag>())
            {
                aspect.AddEvent(BuildingEvent.MainStorageBox_Updated);
            }
        }

        protected override void OnUpdate()
        {
            _mainStorageBoxData = SystemAPI.GetSingleton<MainStorageBoxData>();
        }
    }
}