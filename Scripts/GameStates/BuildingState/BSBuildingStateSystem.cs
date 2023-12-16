using Jrd.Build;
using Jrd.Build.old;
using Jrd.Build.Screen;
using Jrd.GameStates.BuildingState.Tag;
using Jrd.JUI;
using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.GameStates.BuildingState
{
    // LOOK TODO переделать всё
    /// <summary>
    /// Добавляем компоненты, для подключения систем необходимых в билдинг моде
    /// + панель с выбором построек
    /// + панель построить/отменить
    /// Осознанно делаю через 1 сущность
    /// </summary>
    [UpdateAfter(typeof(GameStatesSystem))]
    public partial struct BSBuildingStateSystem : ISystem
    {
        private Entity _gameStateEntity;
        private bool _isSubscribed;
        private EntityManager _em;
        private int _prefabsCount;
        private DynamicBuffer<PrefabBufferElements> _array;
        private RefRW<BuildingStateComponent> _buildingStateComponent;
        private Entity _prefabsComponentEntity;


        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenCenterInWorldCoordsComponent>();
            state.RequireForUpdate<BuildPrefabsComponent>();
            state.RequireForUpdate<BuildingStateComponent>();
            _isSubscribed = false;
            Application.targetFrameRate = default;
        }

        public void OnUpdate(ref SystemState state)
        {
            _em = state.EntityManager;
            _gameStateEntity = SystemAPI
                .GetComponent<GameStateData>(state.World.GetExistingSystem(typeof(GameStatesSystem)))
                .GameStateEntity;

            _prefabsComponentEntity = SystemAPI.GetSingletonEntity<BuildPrefabsComponent>();

            _buildingStateComponent = SystemAPI.GetComponentRW<BuildingStateComponent>(_gameStateEntity);

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            _buildingStateComponent.ValueRW.PrefabsCount =
                SystemAPI.GetBuffer<PrefabBufferElements>(_prefabsComponentEntity).Length;


            // Init
            foreach (var unused in SystemAPI.Query<BuildingStateComponent, InitializeTag>())
            {
                Debug.Log("initialize ".ToUpper() + GetType());

                ecb.AddComponent(_gameStateEntity,
                    new ComponentTypeSet(
                        typeof(BSBuildingsPanelComponent),
                        typeof(BSBuildingsPanelShowTag)
                    ));
                ecb.RemoveComponent<InitializeTag>(_gameStateEntity);
            }

            // deactivate
            foreach (var unused in SystemAPI.Query<BuildingStateComponent, DeactivateStateTag>())
            {
                Debug.Log("deactivate ".ToUpper() + GetType());

                ecb.AddComponent<BSBuildingsPanelHideTag>(_gameStateEntity); // TODO
                ecb.AddComponent<BSApplyPanelHideTag>(_gameStateEntity); // TODO
                ecb.RemoveComponent<BuildingStateComponent>(_gameStateEntity); // TODO
                ecb.RemoveComponent<DeactivateStateTag>(_gameStateEntity); // TODO
            }

            foreach (var q in SystemAPI.Query<RefRO<TempBuildingTag>, RefRO<BuildingDetailsComponent>>())
            {
                _buildingStateComponent.ValueRW.TempEntity = q.Item2.ValueRO.entity;
            }

            ecb.Playback(_em);
            ecb.Dispose();

            if (_isSubscribed) return;
            BuildingPanelUI.OnBuildSelected += PlaceBuilding;
            ApplyPanelUI.ApplyPanelCancelButton.clicked += OnCancelBtnClicked;
            ApplyPanelUI.ApplyPanelApplyButton.clicked += OnApplyBtnClicked;
            _isSubscribed = true;
        }

        private void PlaceBuilding(Button button, int index)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            // 1. get prefab, set prefab data to BuildingsPanel
            var selectedPrefab = _em.GetBuffer<PrefabBufferElements>(_prefabsComponentEntity)[index].PrefabEntity;
            
            ecb.SetComponent(_gameStateEntity, new BuildingStateComponent
            {
                SelectedPrefab = selectedPrefab,
                SelectedPrefabID = index
            });
            ApplyPanelUI.ApplyPanelLabel.text = "Build " + "b-" + index + "?";
            ecb.AddComponent(_gameStateEntity,
                new ComponentTypeSet(
                    typeof(BSApplyPanelComponent),
                    typeof(BSApplyPanelShowTag)
                ));
            ecb.AddComponent(_gameStateEntity, new PlaceBuildingComponent
            {
                placePosition = SystemAPI.GetSingleton<ScreenCenterInWorldCoordsComponent>().ScreenCenterToWorld,
                placePrefab = selectedPrefab
            });
            ecb.Playback(_em);
            ecb.Dispose();
            // Debug.Log($"button ID : {index} /// {button}");
        }

        private void OnCancelBtnClicked()
        {
            Debug.Log("cancel button");

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var building = _em.GetComponentData<BuildingStateComponent>(_gameStateEntity);

            // remove temp building
            ecb.DestroyEntity(building.TempEntity);

            ecb.AddComponent<DeactivateStateTag>(_gameStateEntity);

            ecb.Playback(_em);
            ecb.Dispose();
        }

        private void OnApplyBtnClicked()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            Debug.Log("apply button");

            ecb.Playback(_em);
            ecb.Dispose();
        }

        public void OnDestroy(ref SystemState state)
        {
            ApplyPanelUI.ApplyPanelCancelButton.clicked -= OnCancelBtnClicked;
            ApplyPanelUI.ApplyPanelApplyButton.clicked -= OnApplyBtnClicked;
            BuildingPanelUI.OnBuildSelected -= PlaceBuilding;
        }
    }
}