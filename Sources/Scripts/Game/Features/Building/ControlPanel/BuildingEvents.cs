using System;
using Sources.Scripts.Game.Features.Building.Events;
using Unity.Collections;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.ControlPanel
{
    public struct BuildingEvents
    {
        private NativeQueue<BuildingEvent> _eQueue;

        public BuildingEvents(NativeQueue<BuildingEvent> buildingDataBuildingEvents)
        {
            _eQueue = buildingDataBuildingEvents;
        }

        public void ProcessEvents()
        {
            while (_eQueue.Count > 0)
            {
                var ev = _eQueue.Dequeue();

                Debug.LogWarning($"___ BUILDING EVENT: {ev}");
                switch (ev)
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
                        SetItemsToMainStorage();
                        break;
                    case BuildingEvent.WarehouseDataUpdated:
                        SetItemsToWarehouse();
                        break;
                    case BuildingEvent.InProductionBoxDataUpdated:
                        SetItemsToInProductionBox();
                        break;
                    case BuildingEvent.ManufacturedBoxDataUpdated:
                        SetItemsToManufacturedBox();
                        break;
                    case BuildingEvent.OneLoadCycleFinished:
                        _aspect.ProductionProcessData.ValueRW.CurrentCycle += 1;

                        var currentCycle = _aspect.ProductionProcessData.ValueRO.CurrentCycle;
                        var maxLoads = _aspect.ProductionProcessData.ValueRO.MaxLoads;

                        UpdateProductionTimers(currentCycle, maxLoads);

                        break;
                    case BuildingEvent.FullLoadCycleFinished:
                        _aspect.ProductionProcessData.ValueRW.LastCycleEnd = true;
                        _buildingUI.LoadButton.SetEnabled(true);
                        break;

                    case BuildingEvent.ProductionTimersStarted:
                        OnProductionTimersStarted();
                        break;
                    case BuildingEvent.ProductionTimersInProgressUpdate:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnProductionTimersStarted()
        {
            var currentCycle = _aspect.ProductionProcessData.ValueRO.CurrentCycle;
            var maxLoads = _aspect.ProductionProcessData.ValueRO.MaxLoads;

            UpdateProductionTimers(currentCycle, maxLoads);

            _aspect.ProductionProcessData.ValueRW.LastCycleEnd = false;
            _buildingUI.LoadButton.SetEnabled(false);
        }

        private void OnMoveToWarehouseTimerStarted()
        {
            SetStorageTimer(10, 3); //TODO
            SetItemsToMainStorage();
            _buildingUI.MoveButton.SetEnabled(false);
        }

        private void OnMoveToWarehouseTimerFinished()
        {
            DeliverProductsToWarehouse();
            SetStorageTimer(10, 10); //TODO
            SetItemsToWarehouse();
            _buildingUI.MoveButton.SetEnabled(true);
        }

        private void OnMoveToProductionBoxFinished()
        {
            SetItemsToWarehouse();
            SetItemsToProduction();
        }
    }
}