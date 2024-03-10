using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.Game.Features.Building.PlaceBuilding;
using Sources.Scripts.UI;
using Sources.Scripts.UI.BlueprintsShopPanel;
using Sources.Scripts.UI.PopUpPanels;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sources.Scripts.Game.Features.Shop.BlueprintsShop.System
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial class BlueprintsShopPanelSystem : SystemBase
    {
        #region Private

        private BeginSimulationEntityCommandBufferSystem.Singleton _bsEcbSystem;
        private EntityCommandBuffer _bsEcb;

        private DynamicBuffer<BlueprintsBuffer> _blueprintsBuffers;
        private int _blueprintsCount;
        private int _tempSelectedBlueprintID;

        private BlueprintsShopPanelUI _blueprintsShopUI;
        private ConfirmationPanelUI _confirmationUI;

        #endregion

        #region Create/Start/Destroy

        protected override void OnCreate()
        {
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<BlueprintsBuffer>();
        }

        protected override void OnStartRunning()
        {
            _bsEcbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

            _tempSelectedBlueprintID = -1;
            _blueprintsShopUI = BlueprintsShopPanelUI.Instance;
            _confirmationUI = ConfirmationPanelUI.Instance;

            MainUIButtonsMono.BlueprintsShopButton.clicked += BlueprintsShop_Selected;
            BlueprintsShopPanelUI.OnBlueprintSelected += Blueprint_Selected;
            ConfirmationPanelUI.OnTempBuildCancelled += CancelBuilding;
            ConfirmationPanelUI.OnTempBuildApply += ConfirmBuilding;
        }

        protected override void OnDestroy()
        {
            MainUIButtonsMono.BlueprintsShopButton.clicked -= BlueprintsShop_Selected;
            BlueprintsShopPanelUI.OnBlueprintSelected -= Blueprint_Selected;
            ConfirmationPanelUI.OnTempBuildCancelled -= CancelBuilding;
            ConfirmationPanelUI.OnTempBuildApply -= ConfirmBuilding;
        }

        #endregion

        protected override void OnUpdate()
        {
            _bsEcb = _bsEcbSystem.CreateCommandBuffer(World.Unmanaged);

            _blueprintsBuffers = SystemAPI.GetSingletonBuffer<BlueprintsBuffer>();
            _blueprintsCount = _blueprintsBuffers.Length;
        }

        private void BlueprintsShop_Selected() =>
            _blueprintsShopUI.ShopSelected(_blueprintsCount, GetBlueprintsNamesList());

        private void Blueprint_Selected(Button button, int index)
        {
            SetTempSelectedBlueprintId(index);
            InstantiateTempSelectedBlueprint(index);

            _confirmationUI.SetElementVisible(true);
        }

        private void CancelBuilding()
        {
            DestroyTempSelectedBlueprint();

            _confirmationUI.SetElementVisible(false);
            _blueprintsShopUI.ShopSelected(_blueprintsCount, GetBlueprintsNamesList());

            _tempSelectedBlueprintID = -1;
        }

        private void ConfirmBuilding()
        {
            _confirmationUI.SetElementVisible(false);

            PlaceTempBuilding();

            _tempSelectedBlueprintID = -1;
        }


        #region Instantiate/Place/Destroy selected blueprint

        private void InstantiateTempSelectedBlueprint(int index)
        {
            bool condition = SystemAPI.TryGetSingletonEntity<BuildingStateData>(out var buildingStateEntity);

            Assert.IsTrue(condition, "Cant find Building State Data");

            _bsEcb.AddComponent(buildingStateEntity, new InstantiateTempBlueprintData { BlueprintId = index });
        }

        private void PlaceTempBuilding()
        {
            bool condition = SystemAPI.TryGetSingletonEntity<TempBuildingTag>(out var tempBuildingEntity);

            Assert.IsTrue(condition, "We can't find temp building entity!");

            _bsEcb.AddComponent<PlaceTempBuildingTag>(tempBuildingEntity);
        }

        private void DestroyTempSelectedBlueprint()
        {
            bool condition = SystemAPI.TryGetSingletonEntity<TempBuildingTag>(out var tempEntity);

            Assert.IsTrue(condition, "Cant find Temp Building Tag");

            _bsEcbSystem
                .CreateCommandBuffer(World.Unmanaged)
                .AddComponent(tempEntity, new DestroyTempBlueprintTag());
        }

        #endregion

        private NativeList<FixedString32Bytes> GetBlueprintsNamesList()
        {
            NativeList<FixedString32Bytes> namesList = new(_blueprintsCount, Allocator.Temp);

            foreach (var blueprint in _blueprintsBuffers)
            {
                namesList.Add(new FixedString32Bytes(blueprint.Name));
            }

            return namesList;
        }

        private void SetTempSelectedBlueprintId(int index)
        {
            if (_tempSelectedBlueprintID < 0)
            {
                // SetButtonEnabled(index, false);
                _tempSelectedBlueprintID = index;
            }
            else if (_tempSelectedBlueprintID != index)
            {
                // SetButtonEnabled(index, false);
                // SetButtonEnabled(_tempSelectedBuildID, true);
                _tempSelectedBlueprintID = index;
            }
            else
            {
                Debug.LogWarning("Temp = Index! / We have a problem with enable/disable buttons in " + this);
            }
        }
    }
}