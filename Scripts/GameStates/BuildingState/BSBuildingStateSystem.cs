using System.Linq;
using Jrd.Build;
using Jrd.Build.old;
using Jrd.Build.Screen;
using Jrd.GameStates.BuildingState.Tag;
using Jrd.JUI;
using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.GameStates.BuildingState
{
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
        private EntityCommandBuffer _ecb;
        private int _prefabsCount;
        private DynamicBuffer<PrefabBufferElements> _array;
        private Entity _buildingStateComponent;


        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ScreenCenterInWorldCoordsComponent>();
            state.RequireForUpdate<BuildPrefabsComponent>();
            state.RequireForUpdate<BuildingStateComponent>();
            _em = state.EntityManager;
            _isSubscribed = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            _gameStateEntity = SystemAPI
                .GetComponent<GameStateData>(state.World.GetExistingSystem(typeof(GameStatesSystem)))
                .GameStateEntity;

            _ecb = new EntityCommandBuffer(Allocator.Temp);



            var prefabsComponentEntity = SystemAPI.GetSingletonEntity<BuildPrefabsComponent>();


            _ecb.SetComponent(_gameStateEntity, new BuildingStateComponent
            {
                PrefabsCount = SystemAPI.GetBuffer<PrefabBufferElements>(prefabsComponentEntity).Length,
            });


            // Init
            foreach (var unused in SystemAPI.Query<BuildingStateComponent, InitializeTag>())
            {
                Debug.Log("initialize ".ToUpper() + GetType());

                _ecb.AddComponent(_gameStateEntity,
                    new ComponentTypeSet(
                        typeof(BSBuildingsPanelComponent),
                        typeof(BSBuildingsPanelShowTag)
                    ));
                _ecb.RemoveComponent<InitializeTag>(_gameStateEntity);
            }

            // deactivate
            foreach (var unused in SystemAPI.Query<BuildingStateComponent, DeactivateStateTag>())
            {
                Debug.Log("deactivate ".ToUpper() + GetType());

                _ecb.AddComponent<BSBuildingsPanelHideTag>(_gameStateEntity); // TODO
                _ecb.AddComponent<BSApplyPanelHideTag>(_gameStateEntity); // TODO
                _ecb.RemoveComponent<BuildingStateComponent>(_gameStateEntity); // TODO
                _ecb.RemoveComponent<DeactivateStateTag>(_gameStateEntity); // TODO

                // LOOK возможно этот компонент не удалять, к примеру, можно сохранить в нем какое-нибудь
                // LOOK состояние панели, для последующего восстановления стэйта
                // ecb.RemoveComponent<BuildingsPanelData>(_gameStateEntity); // TODO // LOOK если удлить будет проблема с хайдом панели

                // LOOK возможно этот компонент не удалять, к примеру, можно сохранить в нем какое-нибудь
                // LOOK состояние панели, для последующего восстановления стэйта
                // ecb.RemoveComponent<BSApplyPanelComponent>(_gameStateEntity); // TODO // LOOK если удлить будет проблема с хайдом панели
            }

            _ecb.Playback(_em);
            _ecb.Dispose();

            if (_isSubscribed) return;
            BuildingPanelUI.OnBuildSelected += PlaceBuilding;
            ApplyPanelUI.ApplyPanelCancelButton.clicked += OnCancelBtnClicked;
            ApplyPanelUI.ApplyPanelApplyButton.clicked += OnApplyBtnClicked;
            _isSubscribed = true;
        }

        private void PlaceBuilding(Button button, int index)
        {
            _ecb = new EntityCommandBuffer(Allocator.Temp);
            var prefabsComponentEntity = SystemAPI.GetSingletonEntity<BuildPrefabsComponent>();
            _array = _em.GetBuffer<PrefabBufferElements>(prefabsComponentEntity);
            // 1. get prefab, set prefab data to BuildingsPanel
            var selectedPrefab = _array[index].PrefabEntity;
            Debug.Log("Select prefab " + selectedPrefab);

            _ecb.SetComponent(_gameStateEntity, new BuildingStateComponent
            {
                SelectedPrefab = selectedPrefab,
                SelectedPrefabID = index
            });

            // 2. open ApplyPanel
            ApplyPanelUI.ApplyPanelLabel.text = "Build " + "b-" + index + "?";
            _ecb.AddComponent(_gameStateEntity,
                new ComponentTypeSet(
                    typeof(BSApplyPanelComponent),
                    typeof(BSApplyPanelShowTag)
                ));

            // 3. instantiate prefab
            _ecb.AddComponent(_gameStateEntity, new PlaceBuildingComponent
            {
                placePosition = SystemAPI.GetSingleton<ScreenCenterInWorldCoordsComponent>().ScreenCenterToWorld,
                placePrefab = selectedPrefab
            });

            _ecb.Playback(_em);
            _ecb.Dispose();
            // Debug.Log($"button ID : {index} /// {button}");
        }

        private void OnCancelBtnClicked()
        {
            _buildingStateComponent = _em.GetComponentData<BuildingStateComponent>(_gameStateEntity).TempEntity;
            Debug.Log(_buildingStateComponent + " building state component entity");
            
            _ecb = new EntityCommandBuffer(Allocator.Temp);
            Debug.Log("cancel button");


            // remove temp building
            _ecb.DestroyEntity(_buildingStateComponent);

            Debug.Log(_buildingStateComponent);

            _ecb.AddComponent<DeactivateStateTag>(_gameStateEntity);

            _ecb.Playback(_em);
            _ecb.Dispose();
        }

        private void OnApplyBtnClicked()
        {
            _ecb = new EntityCommandBuffer(Allocator.Temp);
            Debug.Log("apply button");

            _ecb.Playback(_em);
            _ecb.Dispose();
        }

        public void OnDestroy(ref SystemState state)
        {
            ApplyPanelUI.ApplyPanelCancelButton.clicked -= OnCancelBtnClicked;
            ApplyPanelUI.ApplyPanelApplyButton.clicked -= OnApplyBtnClicked;
            BuildingPanelUI.OnBuildSelected -= PlaceBuilding;
        }
    }
}