using Jrd.Build.EditModePanel;
using Jrd.JUI;
using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Build
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
    public partial struct BuildSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private EntityManager _em;
        private bool _isSubscribed;
        private Entity _editModePanelComponent;
        private Entity _tempBuildPrefabComponent;
        private Entity _buildPrefabsComponent;

        private int _currentPrefabId;

        private DynamicBuffer<PrefabBufferElements> _prefabBufferElements;
        private Entity _tempPrefab;

        public void OnCreate(ref SystemState state)
        {
            Debug.Log("BuildSystem");
            // state.RequireForUpdate<TempBuildPrefabComponent>();
            state.RequireForUpdate<EditModePanelComponent>();

            _ecb = new EntityCommandBuffer(Allocator.Temp);
            _em = state.EntityManager;
            _editModePanelComponent = SystemAPI.GetSingletonEntity<EditModePanelComponent>();
            Debug.Log("=== " + _editModePanelComponent);
            // _tempBuildPrefabComponent = SystemAPI.GetSingletonEntity<TempBuildPrefabComponent>();
            // Debug.Log("=== " + _tempBuildPrefabComponent);
            _currentPrefabId = -1;
        }

        public void OnUpdate(ref SystemState state)
        {
            _prefabBufferElements = SystemAPI.GetBuffer<PrefabBufferElements>(_buildPrefabsComponent);


            // LOOK переделать
            if (_isSubscribed) return;
            var tmpThis = this;
            BuildingPanelUI.Building1.clicked += () => { tmpThis.EnterInEditMode(0); };
            BuildingPanelUI.Building2.clicked += () => { tmpThis.EnterInEditMode(1); };
            EditModeUI.EditModeCancelButton.clicked += ExitFromEditMode;
            _isSubscribed = true;
        }

        private void EnterInEditMode(int a)
        {
            _tempPrefab = _prefabBufferElements.ElementAt(a).PrefabEntity;
            H.T(_tempPrefab.ToString());

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
                _em.SetComponentData(_tempBuildPrefabComponent,
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
                _em.DestroyEntity(_em.GetComponentData<TempBuildPrefabComponent>(_tempBuildPrefabComponent)
                    .instantiatedTempEntity);
                // Set new temp prefab
                _em.SetComponentData(_tempBuildPrefabComponent,
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