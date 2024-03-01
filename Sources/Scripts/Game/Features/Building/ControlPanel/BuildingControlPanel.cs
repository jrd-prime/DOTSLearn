using System;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.Game.Features.Building.Storage.MainStorage;
using Sources.Scripts.UI.BuildingControlPanel;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.ControlPanel
{
    public struct BuildingControlPanelDataWrapper
    {
        public EntityCommandBuffer Ecb;
        public BuildingDataAspect Aspect;
        public MainStorageData MainStorageData;
        public BuildingControlPanelUI BuildingUI;
        public NativeList<ProductData> ProdsToDelivery;
    }

    public struct BuildingControlPanel : IDisposable
    {
        public BuildingButtons Button { get; private set; }
        public BuildingEvents Event { get; private set; }
        public BuildingUIUpdater UI { get; private set; }
        
        public BuildingControlPanel(BuildingControlPanelDataWrapper buildingControlPanelData)
        {
            var ecb = buildingControlPanelData.Ecb;
            var aspect = buildingControlPanelData.Aspect;

            var buildingUI = buildingControlPanelData.BuildingUI;
            var mainStorageData = buildingControlPanelData.MainStorageData.Value;
            var productsToDelivery = buildingControlPanelData.ProdsToDelivery;

            Button = new BuildingButtons(aspect.Self, ecb);
            Event = new BuildingEvents(aspect.BuildingData.BuildingEvents);
            UI = new BuildingUIUpdater(buildingUI, aspect, mainStorageData, productsToDelivery);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Process()
        {
            return true;
        }
    }
}