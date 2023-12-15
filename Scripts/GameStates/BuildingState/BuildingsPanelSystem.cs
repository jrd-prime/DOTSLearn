using Jrd.Build;
using Jrd.Build.EditModePanel;
using Jrd.Build.old;
using Jrd.GameStates.BuildingState.Tag;
using Jrd.JUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.UIElements;

namespace Jrd.GameStates.BuildingState
{
    /// <summary>
    /// Панель с выбором построек
    /// - включается, когда присутствует BuildingState + BuildingsPanel
    ///
    /// * инициализирует и показывает/скрывает панель построек
    /// </summary>
    [UpdateAfter(typeof(BSBuildingStateSystem))]
    public partial struct BuildingsPanelSystem : ISystem
    {
        private EntityManager _em;
        private Entity _buildPrefabComponentEntity;
        private DynamicBuffer<PrefabBufferElements> _prefabsBuffer;
        private Entity _gameStateEntity;
        private bool _isSubscribed;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildPrefabComponent>();
            _em = state.EntityManager;
        }

        public void OnUpdate(ref SystemState state)
        {
            _gameStateEntity = SystemAPI
                .GetComponent<GameStateData>(state.World.GetExistingSystem(typeof(GameStatesSystem)))
                .GameStateEntity;

            if (!SystemAPI.HasComponent<BSBuildingsPanelComponent>(_gameStateEntity)) return;

            _buildPrefabComponentEntity = SystemAPI.GetSingletonEntity<BuildPrefabComponent>();
            _prefabsBuffer = _em.GetBuffer<PrefabBufferElements>(_buildPrefabComponentEntity);

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var unused in SystemAPI.Query<BSBuildingsPanelComponent, BSBuildingsPanelShowTag>())
            {
                // Debug.Log("show");
                BuildingPanelUI.InstantiateButtons(_prefabsBuffer.Length);
                BuildingPanelUI.ShowEditModePanel();
                ecb.RemoveComponent<BSBuildingsPanelShowTag>(_gameStateEntity);
            }

            foreach (var unused in SystemAPI.Query<BSBuildingsPanelComponent, BSBuildingsPanelHideTag>())
            {
                // Debug.Log("hide");
                BuildingPanelUI.HideEditModePanel();
                ecb.RemoveComponent<BSBuildingsPanelHideTag>(_gameStateEntity);
            }

            ecb.Playback(_em);
            ecb.Dispose();

            if (_isSubscribed) return;
            BuildingPanelUI.OnBuildSelected += PlaceBuilding;
            _isSubscribed = true;
        }


        private void PlaceBuilding(Button button, int index)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // 1. get prefab, set prefab data to BuildingsPanel
            _prefabsBuffer = _em.GetBuffer<PrefabBufferElements>(_buildPrefabComponentEntity);
            ecb.SetComponent(_gameStateEntity, new BuildingStateComponent
            {
                SelectedPrefab = _prefabsBuffer.ElementAt(index).PrefabEntity,
                SelectedPrefabID = index
            });

            // 2. open ApplyPanel
            ecb.AddComponent(_gameStateEntity,
                new ComponentTypeSet(
                    typeof(BSApplyPanelComponent),
                    typeof(BSApplyPanelShowTag)
                ));
            
            // 3. instantiate prefab
            // 4. add to prefab component with details
            // 5. add to prefab component PlaceBuilding
            // 6. set temp data/properties to PlaceBuilding
            // 7. set prefab position to place

            // Debug.Log($"button ID : {index} /// {button}");


            ecb.Playback(_em);
            ecb.Dispose();
        }

        public void OnDestroy(ref SystemState state)
        {
            BuildingPanelUI.OnBuildSelected -= PlaceBuilding;
        }
    }
}