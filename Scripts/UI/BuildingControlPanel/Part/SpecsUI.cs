using System;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel.Part
{
    public class SpecsUI : IBcpSpecs
    {
        private readonly Label _productivityLabel;
        private readonly Label _productivityIntLabel;
        private readonly Label _loadCapacityLabel;
        private readonly Label _loadCapacityIntLabel;
        private readonly Label _warehouseCapacityLabel;
        private readonly Label _warehouseCapacityIntLabel;

        public SpecsUI(VisualElement panel)
        {
            _productivityLabel = panel.Q<Label>(BCPNamesID.SpecProductivityNameLabelId);
            _productivityIntLabel = panel.Q<Label>(BCPNamesID.SpecProductivityIntNameLabelId);
            _loadCapacityLabel = panel.Q<Label>(BCPNamesID.SpecLoadCapacityNameLabelId);
            _loadCapacityIntLabel = panel.Q<Label>(BCPNamesID.SpecLoadCapacityIntNameLabelId);
            _warehouseCapacityLabel = panel.Q<Label>(BCPNamesID.SpecStorageCapacityNameLabelId);
            _warehouseCapacityIntLabel = panel.Q<Label>(BCPNamesID.SpecStorageCapacityIntNameLabelId);
        }

        public void SetSpecName(Spec specName, string value)
        {
            switch (specName)
            {
                case Spec.Productivity:
                    _productivityLabel.text = $"{value} / hour";
                    break;
                case Spec.LoadCapacity:
                    _loadCapacityLabel.text = $"{value} / load";
                    break;
                case Spec.WarehouseCapacity:
                    _warehouseCapacityLabel.text = $"Max {value} in storage";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(specName), specName, null);
            }
        }

        public void SetProductivity(float value) => _productivityIntLabel.text = value.ToString();
        public void SetLoadCapacity(int value) => _loadCapacityIntLabel.text = value.ToString();
        public void SetStorageCapacity(int value) => _warehouseCapacityIntLabel.text = value.ToString();
    }

    public interface IBcpSpecs
    {
        public void SetSpecName(Spec specName, string value);
        public void SetProductivity(float value);
        public void SetLoadCapacity(int value);
        public void SetStorageCapacity(int value);
    }

    public enum Spec
    {
        Productivity,
        LoadCapacity,
        WarehouseCapacity
    }
}