using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Storage;
using Sources.Scripts.UI;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.ControlPanel.Panel
{
    public class BuildingButtons
    {
        private readonly TextPopUpMono _textPopUpUI;

        public BuildingButtons()
        {
            _textPopUpUI = TextPopUpMono.Instance;
        }

        public void MoveButton(EntityCommandBuffer ecb, Entity entity) =>
            ecb.AddComponent<MoveToWarehouseRequestTag>(entity);
        
        public void LoadButton(EntityCommandBuffer ecb, Entity entity) =>
            ecb.AddComponent<MoveToProductionBoxRequestTag>(entity);

        public void TakeButton(EntityCommandBuffer ecb, Entity entity) =>
            ecb.AddComponent<MoveToMainStorageRequestTag>(entity);

        public void UpgradeButton(EntityCommandBuffer ecb, Entity entity)
        {
            _textPopUpUI.ShowPopUp("upgrade btn");
        }

        public void BuffButton(EntityCommandBuffer ecb, Entity entity)
        {
            _textPopUpUI.ShowPopUp("buff btn");
        }

        public void InstantDeliveryButton(EntityCommandBuffer ecb, Entity entity)
        {
            _textPopUpUI.ShowPopUp("instant btn");
            ecb.AddComponent<InstantBuffTag>(entity);
        }
    }
}