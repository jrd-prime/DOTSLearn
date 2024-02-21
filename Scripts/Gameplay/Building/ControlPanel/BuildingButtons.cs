

using GamePlay.Products.Component;
using GamePlay.Storage.InProductionBox;
using GamePlay.Storage.MainStorage;
using GamePlay.UI;
using JTimer.Component;
using Unity.Entities;

namespace GamePlay.Building.ControlPanel
{
    public class BuildingButtons
    {
        private readonly TextPopUpMono _textPopUpUI = TextPopUpMono.Instance;

        //TODO disable button if in storage 0 req products
        //TODO add move time to button
        public void MoveButton(Entity buildingEntity, EntityCommandBuffer ecb) =>
            ecb.AddComponent<MoveToWarehouseRequestTag>(buildingEntity);

        /// <summary>
        /// <see cref="MoveToProductionBoxSystem"/>
        /// </summary>
        /// <param name="buildingEntity"></param>
        /// <param name="ecb"></param>
        public void LoadButton(Entity buildingEntity, EntityCommandBuffer ecb) =>
            ecb.AddComponent<MoveToProductionBoxRequestTag>(buildingEntity);


        public void TakeButton(Entity buildingEntity, EntityCommandBuffer ecb) =>
            ecb.AddComponent<MoveToMainStorageRequestTag>(buildingEntity);


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