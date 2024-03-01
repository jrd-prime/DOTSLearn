using Sources.Scripts.CommonComponents;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox;
using Sources.Scripts.Game.Features.Building.Storage.MainStorage;
using Sources.Scripts.Game.Features.Building.Storage.Warehouse;
using Sources.Scripts.UI;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.ControlPanel
{
    public struct BuildingButtons
    {
        private readonly TextPopUpMono _textPopUpUI;
        private readonly Entity _buildingEntity;
        private EntityCommandBuffer _ecb;

        public BuildingButtons(Entity buildingEntity, EntityCommandBuffer ecb)
        {
            _buildingEntity = buildingEntity;
            _ecb = ecb;
            _textPopUpUI = TextPopUpMono.Instance;
        }

        //TODO disable button if in storage 0 req products
        //TODO add move time to button
        public void MoveButton() =>
            _ecb.AddComponent<MoveToWarehouseRequestTag>(_buildingEntity);

        /// <summary>
        /// <see cref="Storage.InProductionBox.System.MoveToProductionBoxSystem"/>
        /// </summary>
        public void LoadButton() =>
            _ecb.AddComponent<MoveToProductionBoxRequestTag>(_buildingEntity);


        public void TakeButton() =>
            _ecb.AddComponent<MoveToMainStorageRequestTag>(_buildingEntity);


        public void UpgradeButton()
        {
            _textPopUpUI.ShowPopUp("upgrade btn");
        }

        public void BuffButton()
        {
            _textPopUpUI.ShowPopUp("buff btn");
        }

        public void InstantDeliveryButton()
        {
            _textPopUpUI.ShowPopUp("instant btn");
            _ecb.AddComponent<InstantBuffTag>(_buildingEntity);
        }
    }
}