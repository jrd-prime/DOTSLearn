using System;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.UI.BuildingControlPanel;
using Unity.Collections;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.ControlPanel.Panel
{
    public class BuildingEvents
    {
        #region Private

        private BuildingDataAspect _aspect;

        private readonly BuildingControlPanelUI _mainUI;
        private readonly BuildingUIUpdater _uiUpdater;
        private readonly ProductionEvents _productionEvents;

        #endregion

        public BuildingEvents(BuildingUIUpdater uiUpdater)
        {
            _uiUpdater = uiUpdater;
            _mainUI = BuildingControlPanelUI.Instance;
            _productionEvents = new ProductionEvents(_mainUI, _uiUpdater);
        }

        public void Process(ref EventsDataWrapper eventsDataWrapper)
        {
            // Update data in ui updater, than process events queue
            _uiUpdater.UpdateVarsData(ref eventsDataWrapper);

            _aspect = eventsDataWrapper.Aspect;

            NativeQueue<BuildingEvent> eQueue = _aspect.BuildingData.BuildingEvents;

            while (eQueue.Count > 0)
            {
                BuildingEvent evt = eQueue.Dequeue();

                // TODO refact to classes
                Debug.LogWarning($"___ BUILDING EVENT: {evt}");
                switch (evt)
                {
                    case BuildingEvent.MoveToWarehouse_Timer_Started:
                        OnMoveToWarehouseTimerStarted();
                        break;
                    case BuildingEvent.MoveToWarehouse_Timer_Finished:
                        OnMoveToWarehouseTimerFinished();
                        break;
                    case BuildingEvent.MoveToProductionBox_Finished:
                        OnMoveToProductionBoxFinished();
                        break;
                    case BuildingEvent.MainStorageBox_Updated:
                        OnMainStorageDataUpdated();
                        break;
                    case BuildingEvent.WarehouseBox_Updated:
                        OnWarehouseDataUpdated();
                        break;
                    case BuildingEvent.InProductionBox_Updated:
                        OnInProductionBoxDataUpdated();
                        break;
                    case BuildingEvent.ManufacturedBox_Updated:
                        OnManufacturedBoxDataUpdated();
                        break;
                    case BuildingEvent.Production_OneLoadCycle_Finished:
                        _productionEvents.OneCycle_Finished(_aspect);
                        break;
                    case BuildingEvent.Production_FullLoadCycle_Finished:
                        _productionEvents.FullCycle_Finished(_aspect);
                        break;
                    case BuildingEvent.Production_Timers_Started:
                        _productionEvents.Timers_Started(_aspect);
                        break;
                    case BuildingEvent.Production_Timers_InProgressUpdate:
                        _productionEvents.Timers_InProgressUpdate(_aspect);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //TODO refact? move?

        #region Events Methods

        private void OnManufacturedBoxDataUpdated()
        {
            _uiUpdater.SetItemsToManufacturedBox();
        }

        private void OnInProductionBoxDataUpdated()
        {
            _uiUpdater.SetItemsToInProductionBox();
        }

        private void OnWarehouseDataUpdated()
        {
            _uiUpdater.SetItemsToWarehouse();
        }

        private void OnMainStorageDataUpdated()
        {
            _uiUpdater.SetItemsToMainStorage();
        }

        #region Timers

        private void OnMoveToWarehouseTimerStarted()
        {
            int duration = ProductData.GetProductsQuantity(_aspect.ProductsInBuildingData.ProductsToDelivery);

            _uiUpdater.RunMoveFromStorageTimerAsync(duration);
            _uiUpdater.SetItemsToMainStorage();
            _mainUI.MoveButton.SetEnabled(false);
        }

        private void OnMoveToWarehouseTimerFinished()
        {
            _uiUpdater.DeliverProductsToWarehouse();
            _uiUpdater.SetItemsToWarehouse();
            _mainUI.MoveButton.SetEnabled(true);
        }

        #endregion
        
        private void OnMoveToProductionBoxFinished()
        {
            _uiUpdater.SetItemsToWarehouse();
            _uiUpdater.SetItemsToProduction();
        }

        #endregion
    }
}