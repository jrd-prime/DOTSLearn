using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage.MainStorage;
using Jrd.Gameplay.Timers.Component;
using Jrd.UI;
using Unity.Entities;

namespace Jrd.Gameplay.Building.ControlPanel
{
    public class BuildingButtons
    {
        private readonly TextPopUpMono _textPopUpUI = TextPopUpMono.Instance;

        public void MoveButton(Entity buildingEntity, EntityCommandBuffer ecb)
        {
            _textPopUpUI.ShowPopUp("move btn");
            //TODO disable button if in storage 0 req products
            //TODO add move time to button

            ecb.AddComponent<MoveToWarehouseRequestTag>(buildingEntity);
        }

        public void LoadButton(Entity buildingEntity, EntityCommandBuffer ecb)
        {
            _textPopUpUI.ShowPopUp("load btn");

            ecb.AddComponent<MoveToProductionBoxRequestTag>(buildingEntity);
        }

        public void TakeButton(Entity buildingEntity, EntityCommandBuffer ecb)
        {
            _textPopUpUI.ShowPopUp("take btn");
            
            ecb.AddComponent<MoveToMainStorageRequestTag>(buildingEntity);
        }

        public void UpgradeButton(Entity buildingEntity, EntityCommandBuffer ecb)
        {
            _textPopUpUI.ShowPopUp("upgrade btn");
        }

        public void BuffButton(Entity buildingEntity, EntityCommandBuffer ecb)
        {
            _textPopUpUI.ShowPopUp("buff btn");
        }

        public void InstantDeliveryButton(Entity buildingEntity, EntityCommandBuffer ecb)
        {
            _textPopUpUI.ShowPopUp("instant btn");
            ecb.AddComponent<InstantBuffTag>(buildingEntity);
        }
    }
}