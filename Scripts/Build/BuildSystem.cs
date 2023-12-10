using Jrd.Build.EditModePanel;
using Jrd.JUI;
using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Build
{
    public partial class BuildSystem : SystemBase
    {
        private EntityCommandBuffer _ecb;
        private EntityManager _em;
        private bool _isSubscribed;
        private Entity _editModePanelComponent;
        private Entity _tempBuildPrefabComponent;
        private Entity _buildPrefabsComponent;

        private Entity _tempPrefab;

        protected override void OnCreate()
        {
            // Debug.Log("BuildSystem cre");
            _ecb = new EntityCommandBuffer(Allocator.Temp);
            _em = EntityManager;

            var editModePanelArchetype = _em.CreateArchetype(typeof(EditModePanelComponent));
            var editModePanelEntity = _ecb.CreateEntity(editModePanelArchetype);
            _ecb.SetName(editModePanelEntity, "_EditModePanelEntity");

            var tempBuildPrefabArchetype = _em.CreateArchetype(typeof(TempBuildPrefabComponent));
            var tempBuildPrefabEntity = _ecb.CreateEntity(tempBuildPrefabArchetype);
            _ecb.SetName(tempBuildPrefabEntity, "_TempBuildPrefabEntity");

            _ecb.Playback(_em);
            _ecb.Dispose();
        }

        protected override void OnStartRunning()
        {
            _buildPrefabsComponent = SystemAPI.GetSingletonEntity<BuildPrefabComponent>();
            _editModePanelComponent = SystemAPI.GetSingletonEntity<EditModePanelComponent>();
            _tempBuildPrefabComponent = SystemAPI.GetSingletonEntity<TempBuildPrefabComponent>();
        }

        protected override void OnUpdate()
        {
            var a = SystemAPI.GetBuffer<PrefabBufferElements>(_buildPrefabsComponent);
            _tempPrefab = a.ElementAt(0).PrefabEntity;

            if (_isSubscribed) return;
            BuildingPanelUI.Building1.clicked += EnterInEditMode;
            EditModeUI.EditModeCancelButton.clicked += ExitFromEditMode;
            _isSubscribed = true;
        }

        private void EnterInEditMode()
        {
            H.T("EnterInEditMode");
            // Enter in edit mode state TODO

            // Show edit mode panel
            _em.AddComponent<VisualElementShowTag>(_editModePanelComponent); // tag for show edit mode panel // TODO ecb

            // Place temp building TODO
            SystemAPI.SetComponent(_tempBuildPrefabComponent,
                new TempBuildPrefabComponent { tempBuildPrefab = _tempPrefab });
            _em.AddComponent<TempPrefabForPlaceTag>(_tempBuildPrefabComponent); // tag for place // TODO ecb
        }

        private void ExitFromEditMode()
        {
            H.T("ExitFromEditMode");

            // Hide edit mode panel
            _em.AddComponent<VisualElementHideTag>(_editModePanelComponent); // tag for hide edit mode panel // TODO ecb

            // Destroy temp building TODO
            _em.AddComponent<TempPrefabForRemoveTag>(_tempBuildPrefabComponent); // TODO ecb

            // Exit from edit mode state TODO
        }
    }
}


// foreach (var (mapBuffer, entity) in SystemAPI.Query<DynamicBuffer<MapElement>>().WithAll<MapEntities>()
//              .WithEntityAccess())
//         {
//             for (int i = 0; i < mapBuffer.Length; i++)
//             {
//                 state.EntityManager.Instantiate(mapBuffer[i].MapEntity);
//             }
//         }