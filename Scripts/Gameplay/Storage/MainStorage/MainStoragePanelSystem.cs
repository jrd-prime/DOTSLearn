using Jrd.UI.MainStorage;
using Unity.Entities;

namespace Jrd.Gameplay.Storage.MainStorage
{
    /// <summary>
    /// Get data from component and Set data to UI 
    /// More?
    /// </summary>
    public partial struct MainStoragePanelSystem : ISystem
    {
        private EntityCommandBuffer _ecb;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            MainStoragePanelUI mainStoragePanelUI = MainStoragePanelUI.Instance;

            _ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (data, entity) in SystemAPI
                         .Query<MainStorageData>()
                         .WithAll<UpdateRequestTag>()
                         .WithEntityAccess())
            {
                mainStoragePanelUI.SetTestItems(data);
                _ecb.RemoveComponent<UpdateRequestTag>(entity);
            }
        }

        public void OnDestroy(ref SystemState state)
        {
            // throw new System.NotImplementedException();
        }
    }
}