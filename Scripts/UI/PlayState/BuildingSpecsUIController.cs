using System;
using UnityEngine.UIElements;

namespace Jrd.PlayState
{
    public class BuildingSpecsUIController : IBuildingSpecs
    {
        private readonly Label _statConvLabel;
        private readonly Label _statConvIntLabel;
        private readonly Label _statLoadLabel;
        private readonly Label _statLoadIntLabel;
        private readonly Label _statStorageLabel;
        private readonly Label _statStorageIntLabel;

        private const string StatConvLabelId = "stat-conv-label";
        private const string StatConvIntLabelIdName = "stat-conv-int-label";
        private const string StatLoadLabelIdName = "stat-load-label";
        private const string StatLoadIntLabelIdName = "stat-load-int-label";
        private const string StatStorageLabelIdName = "stat-storage-label";
        private const string StatStorageIntLabelIdName = "stat-storage-int-label";
        
        public BuildingSpecsUIController(VisualElement panel)
        {
            _statConvLabel = panel.Q<Label>(StatConvLabelId);
            _statConvIntLabel = panel.Q<Label>(StatConvIntLabelIdName);
            _statLoadLabel = panel.Q<Label>(StatLoadLabelIdName);
            _statLoadIntLabel = panel.Q<Label>(StatLoadIntLabelIdName);
            _statStorageLabel = panel.Q<Label>(StatStorageLabelIdName);
            _statStorageIntLabel = panel.Q<Label>(StatStorageIntLabelIdName);
        }
        
        public void SetSpecName(Spec specName, string value)
        {
            switch (specName)
            {
                case Spec.Productivity:
                    _statConvLabel.text = $"{value} / hour";
                    break;
                case Spec.LoadCapacity:
                    _statLoadLabel.text = $"{value} / load";
                    break;
                case Spec.StorageCapacity:
                    _statStorageLabel.text = $"Max {value} in storage";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(specName), specName, null);
            }
        }

        public void SetProductivity(float value) => _statConvIntLabel.text = value.ToString();
        public void SetLoadCapacity(int value) => _statLoadIntLabel.text = value.ToString();
        public void SetStorageCapacity(int value) => _statStorageIntLabel.text = value.ToString();
    }

    public interface IBuildingSpecs
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
        StorageCapacity
    }
}