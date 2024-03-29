﻿using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.Storage.Data;
using Sources.Scripts.Game.Features.Building.ControlPanel.Panel;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.UIElements;

namespace Sources.Scripts.Game.Features.Building.ControlPanel.System
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial class BuildingControlPanelSystem : SystemBase
    {
        #region PrivateVars

        private EntityCommandBuffer _ecb;
        private Entity _entity;

        private EventsDataWrapper _eventsDataWrapper;

        private BuildingControlPanel _panel;
        private bool _isInitialized;

        #endregion

        protected override void OnCreate()
        {
            RequireForUpdate<MainStorageBoxData>();
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

            _isInitialized = false;

            _eventsDataWrapper = new EventsDataWrapper
            {
                Aspect = default,
                MainStorageBoxData = default,
                ProductsToDelivery = default
            };
        }

        protected override void OnUpdate()
        {
            _ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);

            foreach (var aspect in SystemAPI.Query<BuildingDataAspect>().WithAll<SelectedBuildingTag>())
            {
                _entity = aspect.Self;

                if (!_isInitialized)
                    InitPanel();
                else
                    UpdatePanel(aspect, SystemAPI.GetSingleton<MainStorageBoxData>());
            }
        }

        #region Methods

        private void UpdatePanel(BuildingDataAspect aspect, MainStorageBoxData mainStorageBoxData)
        {
            _eventsDataWrapper.Aspect = aspect;
            _eventsDataWrapper.MainStorageBoxData = mainStorageBoxData;
            _eventsDataWrapper.ProductsToDelivery = GetProdsToDelivery(aspect.Self);

            _panel.ProcessEvents(ref _eventsDataWrapper);
        }

        private void InitPanel()
        {
            _panel = new BuildingControlPanel();
            _isInitialized = true;

            ButtonsCallbacks();
        }

        private NativeList<ProductData> GetProdsToDelivery(Entity entity)
        {
            return SystemAPI.HasComponent<ProductsToDeliveryData>(entity)
                ? SystemAPI.GetComponent<ProductsToDeliveryData>(entity).Value
                : new NativeList<ProductData>(0, Allocator.Temp);
        }

        private void ButtonsCallbacks()
        {
            // TODO refact
            _panel.UI.MoveButton
                .RegisterCallback<ClickEvent>(e => _panel.Buttons.MoveButton(_ecb, _entity));
            _panel.UI.LoadButton
                .RegisterCallback<ClickEvent>(e => _panel.Buttons.LoadButton(_ecb, _entity));
            _panel.UI.TakeButton
                .RegisterCallback<ClickEvent>(e => _panel.Buttons.TakeButton(_ecb, _entity));
            _panel.UI.UpgradeButton
                .RegisterCallback<ClickEvent>(e => _panel.Buttons.UpgradeButton(_ecb, _entity));
            _panel.UI.BuffButton
                .RegisterCallback<ClickEvent>(e => _panel.Buttons.BuffButton(_ecb, _entity));
            _panel.UI.InstantDeliveryButton
                .RegisterCallback<ClickEvent>(e => _panel.Buttons.InstantDeliveryButton(_ecb, _entity));
        }

        #endregion
    }
}