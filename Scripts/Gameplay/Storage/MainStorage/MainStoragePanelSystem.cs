using Jrd.Gameplay.Storage.MainStorage.Component;
using Jrd.UI;
using Jrd.UI.MainStoragePanel;
using Unity.Entities;

namespace Jrd.Gameplay.Storage.MainStorage
{
    /// <summary>
    /// Get data from component and Set data to UI 
    /// More?
    /// </summary>
    public partial class MainStoragePanelSystem : SystemBase
    {
        private EntityCommandBuffer _ecb;
        private Entity _mainStorageEntity;
        private MainStoragePanelUI _mainStoragePanelUI;
        private MainStorageData _mainStorageData;

        protected override void OnCreate()
        {
            RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        protected override void OnStartRunning()
        {
            _mainStoragePanelUI = MainStoragePanelUI.Instance;
            
            MainUIButtonsMono.MainStorageButton.clicked += MainStorageSelected;
        }

        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingletonEntity<MainStorageData>(out Entity entity)) return;
            _mainStorageEntity = entity;

            _mainStorageData = SystemAPI.GetSingleton<MainStorageData>();

            _ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);
        }

        private void MainStorageSelected()
        {
            _mainStoragePanelUI.SetTestItems(_mainStorageData);
            _mainStoragePanelUI.SetElementVisible(true);
        }

        protected override void OnDestroy()
        {
            MainUIButtonsMono.MainStorageButton.clicked -= MainStorageSelected;
        }
    }
}