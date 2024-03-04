using System;
using Sources.Scripts.CommonComponents.test;
using Sources.Scripts.Game.Features.Building.Events;
using Sources.Scripts.UI.BuildingControlPanel;
using Unity.Collections;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.ControlPanel.Panel
{
    public class BuildingEvents
    {
        #region Private

        private CommonComponents.test.BuildingDataAspect _aspect;

        private readonly BuildingControlPanelUI _mainUI;
        private readonly BuildingUIUpdater _uiUpdater;

        #endregion

        public BuildingEvents(BuildingUIUpdater uiUpdater)
        {
            _uiUpdater = uiUpdater;
            _mainUI = BuildingControlPanelUI.Instance;
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
                    case BuildingEvent.MoveToWarehouseTimerStarted:
                        OnMoveToWarehouseTimerStarted();
                        break;
                    case BuildingEvent.MoveToWarehouseTimerFinished:
                        OnMoveToWarehouseTimerFinished();
                        break;
                    case BuildingEvent.MoveToProductionBoxFinished:
                        OnMoveToProductionBoxFinished();
                        break;
                    case BuildingEvent.MainStorageDataUpdated:
                        OnMainStorageDataUpdated();
                        break;
                    case BuildingEvent.WarehouseDataUpdated:
                        OnWarehouseDataUpdated();
                        break;
                    case BuildingEvent.InProductionBoxDataUpdated:
                        OnInProductionBoxDataUpdated();
                        break;
                    case BuildingEvent.ManufacturedBoxDataUpdated:
                        OnManufacturedBoxDataUpdated();
                        break;
                    case BuildingEvent.OneLoadCycleFinished:
                        OnOneLoadCycleFinished();
                        break;
                    case BuildingEvent.FullLoadCycleFinished:
                        OnFullLoadCycleFinished();
                        break;
                    case BuildingEvent.ProductionTimersStarted:
                        OnProductionTimersStarted();
                        break;
                    case BuildingEvent.ProductionTimersInProgressUpdate:
                        OnProductionTimersInProgressUpdate();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //TODO refact? move?

        #region Events Methods

        private void OnProductionTimersInProgressUpdate()
        {
            throw new NotImplementedException();
        }

        private void OnFullLoadCycleFinished()
        {
            _aspect.ProductionProcessData.ValueRW.LastCycleEnd = true;
            _mainUI.LoadButton.SetEnabled(true);
        }

        private void OnOneLoadCycleFinished()
        {
            _aspect.ProductionProcessData.ValueRW.CurrentCycle += 1;
            _uiUpdater.UpdateProductionTimers();
        }

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

        private void OnProductionTimersStarted()
        {
            _uiUpdater.UpdateProductionTimers();
            _aspect.ProductionProcessData.ValueRW.LastCycleEnd = false;
            _mainUI.LoadButton.SetEnabled(false);
        }

        private void OnMoveToWarehouseTimerStarted()
        {
            _uiUpdater.SetStorageTimer(10, 3); //TODO
            _uiUpdater.SetItemsToMainStorage();
            _mainUI.MoveButton.SetEnabled(false);
        }

        private void OnMoveToWarehouseTimerFinished()
        {
            _uiUpdater.DeliverProductsToWarehouse();
            _uiUpdater.SetStorageTimer(10, 10); //TODO
            _uiUpdater.SetItemsToWarehouse();
            _mainUI.MoveButton.SetEnabled(true);
        }

        private void OnMoveToProductionBoxFinished()
        {
            _uiUpdater.SetItemsToWarehouse();
            _uiUpdater.SetItemsToProduction();
        }

        #endregion
    }
}