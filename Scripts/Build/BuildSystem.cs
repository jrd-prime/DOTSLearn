using Jrd.Build.EditModePanel;
using Jrd.JUI;
using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

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

        private int _currentPrefabId = -1;

        private DynamicBuffer<PrefabBufferElements> _prefabBufferElements;
        private Entity _tempPrefab;
        private string _editModeState;

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
            _prefabBufferElements = SystemAPI.GetBuffer<PrefabBufferElements>(_buildPrefabsComponent);

            // LOOK переделать
            if (_isSubscribed) return;

            BuildingPanelUI.Building1.clicked += () => { EnterInEditMode(0); };
            BuildingPanelUI.Building2.clicked += () => { EnterInEditMode(1); };

            EditModeUI.EditModeCancelButton.clicked += ExitFromEditMode;
            _isSubscribed = true;
        }

        private void EnterInEditMode(int a)
        {
            _tempPrefab = _prefabBufferElements.ElementAt(a).PrefabEntity;

            // LOOK ПЕРЕДЕЛАТЬ ЭТО Г

            H.T("EnterInEditMode");

            // init
            if (_currentPrefabId == -1)
            {
                _currentPrefabId = _tempPrefab.Index;
                // Enter in edit mode state TODO

                // Show edit mode panel
                _em.AddComponent<VisualElementShowTag>(
                    _editModePanelComponent); // tag for show edit mode panel // TODO ecb

                // Place temp building
                SystemAPI.SetComponent(_tempBuildPrefabComponent,
                    new TempBuildPrefabComponent { tempBuildPrefab = _tempPrefab });
                _em.AddComponent<TempPrefabForPlaceTag>(_tempBuildPrefabComponent); // tag for place // TODO ecb


                // TODO DISABLE BUTTON
                return;
            }

            // click on same build
            if (_currentPrefabId == _tempPrefab.Index) return;

            // click on other build
            if (_currentPrefabId != _tempPrefab.Index)
            {
                _currentPrefabId = _tempPrefab.Index;
                // Destroy previous prefab
                _em.DestroyEntity(SystemAPI.GetComponent<TempBuildPrefabComponent>(_tempBuildPrefabComponent)
                    .instantiatedTempEntity);
                // Set new temp prefab
                SystemAPI.SetComponent(_tempBuildPrefabComponent,
                    new TempBuildPrefabComponent { tempBuildPrefab = _tempPrefab });
                // Place new temp prefab
                _em.AddComponent<TempPrefabForPlaceTag>(_tempBuildPrefabComponent); // tag for place // TODO ecb
                // TODO DISABLE BUTTON
                return;
            }
        }

        private void ExitFromEditMode()
        {
            H.T("ExitFromEditMode");

            // Hide edit mode panel
            _em.AddComponent<VisualElementHideTag>(_editModePanelComponent); // tag for hide edit mode panel // TODO ecb

            // Destroy temp building TODO
            _em.AddComponent<TempPrefabForRemoveTag>(_tempBuildPrefabComponent); // TODO ecb

            // Exit from edit mode state TODO

            // reset init
            _currentPrefabId = -1;
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