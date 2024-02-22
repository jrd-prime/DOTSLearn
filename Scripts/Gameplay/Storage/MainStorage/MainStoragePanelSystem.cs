using GamePlay.GameStates;
using GamePlay.Storage.MainStorage.Component;
using GamePlay.UI;
using GamePlay.UI.MainStoragePanel;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GamePlay.Storage.MainStorage
{
    /// <summary>
    /// Get data from component and Set data to UI 
    /// More?
    /// </summary>
    [UpdateInGroup(typeof(MyDefaultSystemGroup))]
    public partial class MainStoragePanelSystem : SystemBase
    {
        private EntityCommandBuffer _ecb;
        private Entity _mainStorageEntity;
        private MainStoragePanelUI _mainStoragePanelUI;

        private NativeParallelHashMap<int, int> cachedMainStorageData;

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
            
            Debug.Log(mainStorageData);

            cachedMainStorageData = new NativeParallelHashMap<int, int>(0, Allocator.Persistent);

            foreach (var q in mainStorageData.Value)
            {
                cachedMainStorageData.Add(q.Key, q.Value);
            }
        }

        private void MainStorageSelected()
        {
            _mainStoragePanelUI.SetTestItems(cachedMainStorageData);
            _mainStoragePanelUI.SetElementVisible(true);

            cachedMainStorageData.Dispose();
        }

        protected override void OnDestroy()
        {
            MainUIButtonsMono.MainStorageButton.clicked -= MainStorageSelected;
        }
    }
}