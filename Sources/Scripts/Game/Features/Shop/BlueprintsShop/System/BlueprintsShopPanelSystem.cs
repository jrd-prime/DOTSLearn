using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.Game.Features.Building.PlaceBuilding;
using Sources.Scripts.UI;
using Sources.Scripts.UI.BlueprintsShopPanel;
using Sources.Scripts.UI.BuildingControlPanel;
using Sources.Scripts.UI.PopUpPanels;
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

        private NativeList<Entity> _stateVisualComponents;
        private BeginSimulationEntityCommandBufferSystem.Singleton _bsEcbSystem;
        private BeginInitializationEntityCommandBufferSystem.Singleton _biEcbSystem;
        private EntityCommandBuffer _bsEcb;
        private EntityCommandBuffer _biEcb;


        private Entity _buildingStateEntity;

        private DynamicBuffer<BlueprintsBuffer> _blueprintsBuffers;
        private int _blueprintsCount;
        private int _tempSelectedBlueprintID;

        private BlueprintsShopPanelUI _blueprintsShopUI;
        private ConfirmationPanelUI _confirmationUI;

        #endregion

        #region Create/Start/Destroy

        protected override void OnCreate()
        {
            RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<BlueprintsBuffer>();
        }

        protected override void OnStartRunning()
        {
            _tempSelectedBlueprintID = -1;
            _blueprintsShopUI = BlueprintsShopPanelUI.Instance;
            _confirmationUI = ConfirmationPanelUI.Instance;

            MainUIButtonsMono.BlueprintsShopButton.clicked += BlueprintsShop_Selected;
            BlueprintsShopPanelUI.OnBlueprintSelected += Blueprint_Selected;
            ConfirmationPanelUI.OnTempBuildCancelled += CancelBuilding;
            ConfirmationPanelUI.OnTempBuildApply += ConfirmBuilding;
            BuildingControlPanelUI.Instance.PanelCloseButton.clicked += ClosePanelAndRemoveSelectedTag;
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
            _blueprintsBuffers = SystemAPI.GetSingletonBuffer<BlueprintsBuffer>();

            _blueprintsCount = _blueprintsBuffers.Length;
            
            _biEcbSystem = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            _bsEcbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            _bsEcb = _bsEcbSystem.CreateCommandBuffer(World.Unmanaged);

            _buildingStateEntity = SystemAPI.GetSingletonEntity<BuildingStateData>();
        }

        private void BlueprintsShop_Selected() =>
            _blueprintsShopUI.ShopSelected(_blueprintsCount, GetBlueprintsNamesList());

        private void Blueprint_Selected(Button button, int index)
        {
            _confirmationUI.SetElementVisible(true);

            SetTempSelectedBlueprintId(index);

            InstantiateTempSelectedBlueprint(index);
        }

        private void ClosePanelAndRemoveSelectedTag()
        {
            Debug.LogWarning("ClosePanelAndRemoveSelectedTag ???");
            BuildingControlPanelUI.Instance.SetElementVisible(false);

            var e = SystemAPI.GetSingletonEntity<SelectedBuildingTag>();
            _bsEcb.RemoveComponent<SelectedBuildingTag>(e);
        }

        private void CancelBuilding()
        {
            _confirmationUI.SetElementVisible(false);

            DestroyTempSelectedBlueprint();

            _blueprintsShopUI.ShopSelected(_blueprintsCount, GetBlueprintsNamesList());

            // reset temp id
            _tempSelectedBlueprintID = -1;
        }

        private void ConfirmBuilding()
        {
            _confirmationUI.SetElementVisible(false);
            
            if (SystemAPI.TryGetSingletonEntity<TempBuildingTag>(out var tempBuildingEntity))
            {
                _bsEcb.AddComponent<PlaceTempBuildingTag>(tempBuildingEntity);
                _tempSelectedBlueprintID = -1;
            }
            else
            {
                Debug.LogWarning("We can't find temp building entity!");
            }
        }

        #region Instantiate/Destroy selected blueprint

        private void InstantiateTempSelectedBlueprint(int index) =>
            _bsEcb.AddComponent(_buildingStateEntity, new InstantiateTempBlueprintData { BlueprintId = index });

        private void DestroyTempSelectedBlueprint()
        {
            if (SystemAPI.TryGetSingletonEntity<TempBuildingTag>(out var tempEntity))
            {
                _biEcbSystem
                    .CreateCommandBuffer(World.Unmanaged)
                    .AddComponent(tempEntity, new DestroyTempBlueprintTag());
            }
        }

        #endregion


        private NativeList<FixedString32Bytes> GetBlueprintsNamesList()
        {
            NativeList<FixedString32Bytes> namesList = new(_blueprintsCount, Allocator.Temp);

            foreach (var building in _blueprintsBuffers)
            {
                namesList.Add(new FixedString32Bytes(building.Name));
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