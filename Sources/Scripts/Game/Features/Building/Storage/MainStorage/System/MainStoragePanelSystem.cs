using Sources.Scripts.CommonComponents;
using Sources.Scripts.UI;
using Sources.Scripts.UI.MainStoragePanel;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Storage.MainStorage.System
{
    /// <summary>
    /// Get data from component and Set data to UI 
    /// More?
    /// </summary>
    [UpdateInGroup(typeof(JDefaultSimulationSystemGroup))]
    public partial class MainStoragePanelSystem : SystemBase
    {
        private EntityCommandBuffer _ecb;
        private Entity _mainStorageEntity;
        private MainStoragePanelUI _mainStoragePanelUI;

        private NativeParallelHashMap<int, int> _cachedMainStorageData;

        protected override void OnCreate()
        {
        }

        protected override void OnStartRunning()
        {
            _mainStoragePanelUI = MainStoragePanelUI.Instance;

            MainUIButtonsMono.MainStorageButton.clicked += MainStorageSelected;
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingleton<MainStorageData>(out var mainStorageData))
            {
                Debug.Log("Main storage data singleton does not exist!");
            }

            _cachedMainStorageData = new NativeParallelHashMap<int, int>(0, Allocator.Persistent);

            foreach (var q in mainStorageData.Value)
            {
                _cachedMainStorageData.Add(q.Key, q.Value);
            }
        }

        private void MainStorageSelected()
        {
            _mainStoragePanelUI.SetTestItems(_cachedMainStorageData);
            _mainStoragePanelUI.SetElementVisible(true);

            _cachedMainStorageData.Dispose();
        }

        protected override void OnDestroy()
        {
            MainUIButtonsMono.MainStorageButton.clicked -= MainStorageSelected;
        }
    }
}