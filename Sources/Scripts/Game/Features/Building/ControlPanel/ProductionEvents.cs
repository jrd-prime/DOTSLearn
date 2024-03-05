using Sources.Scripts.CommonData.Building;
using Sources.Scripts.Game.Features.Building.ControlPanel.Panel;
using Sources.Scripts.UI.BuildingControlPanel;

namespace Sources.Scripts.Game.Features.Building.ControlPanel
{
    public class ProductionEvents
    {
        private readonly BuildingControlPanelUI _mainUI;
        private readonly BuildingUIUpdater _uiUpdater;

        public ProductionEvents(BuildingControlPanelUI mainUI, BuildingUIUpdater uiUpdater)
        {
            _mainUI = mainUI;
            _uiUpdater = uiUpdater;
        }

        public void Timers_Started(BuildingDataAspect aspect)
        {
            _uiUpdater.UpdateProductionTimers(aspect.GetLoadedProductsManufacturingTime());
            aspect.ProductionProcessData.ValueRW.LastCycleEnd = false;
            _mainUI.LoadButton.SetEnabled(false);
        }

        public void OneCycle_Finished(BuildingDataAspect aspect)
        {
            aspect.ProductionProcessData.ValueRW.CurrentCycle += 1;
            _uiUpdater.UpdateProductionTimers(aspect.GetLoadedProductsManufacturingTime());
        }

        public void FullCycle_Finished(BuildingDataAspect aspect)
        {
            aspect.ProductionProcessData.ValueRW.LastCycleEnd = true;
            _mainUI.LoadButton.SetEnabled(true);
        }

        public void Timers_InProgressUpdate(BuildingDataAspect aspect)
        {
            _uiUpdater.UpdateProductionTimers(aspect.GetLoadedProductsManufacturingTime());
        }
    }
}