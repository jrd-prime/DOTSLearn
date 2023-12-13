using Jrd.Build.EditModePanel;
using Jrd.JUI;
using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Build
{
    public partial class BuildSystem : SystemBase
    {
        private EntityCommandBuffer _ecb;
        private EntityManager _em;
        private bool _isSubscribed;
        private Entity _editModePanelEntity;
        private Entity _tempBuildPrefabInstantiateEntity;
        private Entity _buildPrefabsEntity;

        private int _currentPrefabId;

        private DynamicBuffer<PrefabBufferElements> _prefabBufferElements;
        private Entity _tempPrefab;
        private Entity _instantiatedTempEntity;

        protected override void OnCreate()
        {
            this.Enabled = false;
            RequireForUpdate<BuildPrefabComponent>();
            RequireForUpdate<EditModePanelComponent>();
            RequireForUpdate<TempBuildPrefabInstantiateComponent>();
            // Debug.Log("BuildSystem");

            _em = EntityManager;
            _currentPrefabId = -1;
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            _editModePanelEntity = SystemAPI.GetSingletonEntity<EditModePanelComponent>();
            _tempBuildPrefabInstantiateEntity = SystemAPI.GetSingletonEntity<TempBuildPrefabInstantiateComponent>();
            _buildPrefabsEntity = SystemAPI.GetSingletonEntity<BuildPrefabComponent>();

            _prefabBufferElements = SystemAPI.GetBuffer<PrefabBufferElements>(_buildPrefabsEntity);
            _instantiatedTempEntity = _em
                .GetComponentData<TempBuildPrefabInstantiateComponent>(_tempBuildPrefabInstantiateEntity)
                .instantiatedTempEntity;


            // LOOK ПЕРЕДЕЛАТЬ ЭТО Г
            if (_isSubscribed) return;
            BuildingPanelUI.Building1.clicked += () => { EnterInEditMode(0); };
            BuildingPanelUI.Building2.clicked += () => { EnterInEditMode(1); };
            EditModeUI.EditModeCancelButton.clicked += ExitFromEditMode;
            _isSubscribed = true;
        }

        private void EnterInEditMode(int a)
        {
            _ecb = new EntityCommandBuffer(Allocator.Temp);
            _tempPrefab = _prefabBufferElements.ElementAt(a).PrefabEntity;

            // LOOK ПЕРЕДЕЛАТЬ ЭТО Г

            H.T("EnterInEditMode");
            if (_currentPrefabId == -1)
            {
                Debug.Log("init");
                _currentPrefabId = _tempPrefab.Index;
                // Enter in edit mode state TODO

                SetEditModeState(true);

                // Show edit mode panel
                _ecb.AddComponent<VisualElementShowTag>(_editModePanelEntity);

                // Place temp building
                _ecb.SetComponent(_tempBuildPrefabInstantiateEntity,
                    new TempBuildPrefabInstantiateComponent { tempBuildPrefab = _tempPrefab });
                _ecb.AddComponent<TempPrefabForPlaceTag>(_tempBuildPrefabInstantiateEntity);

                _ecb.Playback(_em);
                // TODO DISABLE BUTTON
                return;
            }

            if (_currentPrefabId == _tempPrefab.Index)
            {
                Debug.Log("click on same build");
                return;
            }

            if (_currentPrefabId != _tempPrefab.Index)
            {
                Debug.Log("click on other build");
                _currentPrefabId = _tempPrefab.Index;
                // Destroy instantiated temp prefab
                _ecb.DestroyEntity(_instantiatedTempEntity);
                // Set new temp prefab
                _ecb.SetComponent(_tempBuildPrefabInstantiateEntity,
                    new TempBuildPrefabInstantiateComponent { tempBuildPrefab = _tempPrefab });
                // Place new temp prefab
                _ecb.AddComponent<TempPrefabForPlaceTag>(_tempBuildPrefabInstantiateEntity);

                _ecb.Playback(_em);
                // TODO DISABLE BUTTON
                return;
            }

            _ecb.Dispose();
        }

        private void ExitFromEditMode()
        {
            H.T("ExitFromEditMode");
            _ecb = new EntityCommandBuffer(Allocator.Temp);

            // Hide edit mode panel
            _ecb.AddComponent<VisualElementHideTag>(_editModePanelEntity);

            // Destroy temp building TODO
            _ecb.AddComponent<TempPrefabForRemoveTag>(_tempBuildPrefabInstantiateEntity);

            // Exit from edit mode state TODO

            SetEditModeState(false);
            // reset init
            _currentPrefabId = -1;

            _ecb.Playback(_em);
            _ecb.Dispose();
        }

        private void SetEditModeState(bool b)
        {
            // TODO
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