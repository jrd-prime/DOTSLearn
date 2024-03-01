using Sources.Scripts.CommonComponents;
using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.Game.Features.Building.Storage;
using Sources.Scripts.Game.Features.Building.Storage.MainStorage;
using Sources.Scripts.UI.BuildingControlPanel;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.ControlPanel.System
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial class BuildingControlPanelSystem : SystemBase
    {
        #region Vars

        private BeginSimulationEntityCommandBufferSystem.Singleton _sys;
        private EntityCommandBuffer _ecb;
        private Entity _buildingEntity;
        private MainStorageData _mainStorageData;

        private BuildingControlPanelUI _buildingUI;
        private BuildingButtons _buildingButtons;

        private BuildingControlPanel _buildingControlPanel;

        #endregion

        protected override void OnCreate()
        {
            RequireForUpdate<MainStorageData>();
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<BuildingsPrefabsBufferTag>();
        }

        protected override void OnStartRunning()
        {
            _sys = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _mainStorageData = SystemAPI.GetSingleton<MainStorageData>();

            _buildingUI = BuildingControlPanelUI.Instance;
            _buildingUI.MoveButton.clicked += MoveButton;
            _buildingUI.LoadButton.clicked += LoadButton;
            _buildingUI.TakeButton.clicked += TakeButton;
            _buildingUI.UpgradeButton.clicked += UpgradeButton;
            _buildingUI.BuffButton.clicked += BuffButton;
            _buildingUI.InstantDeliveryButton.clicked += InstantDeliveryButton;
        }

        protected override void OnUpdate()
        {
            foreach (var aspect in SystemAPI.Query<BuildingDataAspect>().WithAll<SelectedBuildingTag>())
            {
                _ecb = _sys.CreateCommandBuffer(World.Unmanaged);
                var prodsToDelivery = SystemAPI.GetComponent<ProductsToDeliveryData>(aspect.Self).Value;

                _buildingControlPanel = new BuildingControlPanel(
                    new BuildingControlPanelDataWrapper
                    {
                        Ecb = _ecb,
                        Aspect = aspect,
                        MainStorageData = _mainStorageData,
                        BuildingUI = _buildingUI,
                        ProdsToDelivery = prodsToDelivery
                    });
            }
            
            
        }

        #region Building Buttons

        public void MoveButton() => _buildingControlPanel.Button.MoveButton();
        public void LoadButton() => _buildingControlPanel.Button.LoadButton();
        public void TakeButton() => _buildingControlPanel.Button.TakeButton();
        public void UpgradeButton() => _buildingControlPanel.Button.UpgradeButton();
        public void BuffButton() => _buildingControlPanel.Button.BuffButton();
        private void InstantDeliveryButton() => _buildingControlPanel.Button.InstantDeliveryButton();

        #endregion
    }
}