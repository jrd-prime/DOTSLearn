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
        private int _prefabsCount;
        private Entity _prefabsComponentEntity;
        private NativeArray<PrefabBufferElements> _array;
        private DynamicBuffer<PrefabBufferElements> _prefabsBuffer;

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

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            _prefabsComponentEntity = SystemAPI.GetSingletonEntity<BuildPrefabsComponent>();
            _prefabsBuffer = _em.GetBuffer<PrefabBufferElements>(_prefabsComponentEntity);


            if (_prefabsCount != _prefabsBuffer.Length)
            {
                _prefabsCount = _prefabsBuffer.Length;
                _em.SetComponentData(_gameStateEntity, new BuildingStateComponent
                {
                    PrefabsCount = _prefabsCount,
                    PrefabsBufferElementsCache = _prefabsBuffer.ToNativeArray(Allocator.Persistent)
                });
            }

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

            foreach (var unused in SystemAPI.Query<BuildingStateComponent, DeactivateStateTag>())
            {
                Debug.Log("deactivate ".ToUpper() + GetType());

                ecb.AddComponent<BSBuildingsPanelHideTag>(_gameStateEntity); // TODO
                ecb.AddComponent<BSApplyPanelHideTag>(_gameStateEntity); // TODO
                ecb.RemoveComponent<BuildingStateComponent>(_gameStateEntity); // TODO
                ecb.RemoveComponent<DeactivateStateTag>(_gameStateEntity); // TODO

                // LOOK возможно этот компонент не удалять, к примеру, можно сохранить в нем какое-нибудь
                // LOOK состояние панели, для последующего восстановления стэйта
                // ecb.RemoveComponent<BuildingsPanelData>(_gameStateEntity); // TODO // LOOK если удлить будет проблема с хайдом панели

                // LOOK возможно этот компонент не удалять, к примеру, можно сохранить в нем какое-нибудь
                // LOOK состояние панели, для последующего восстановления стэйта
                // ecb.RemoveComponent<BSApplyPanelComponent>(_gameStateEntity); // TODO // LOOK если удлить будет проблема с хайдом панели
            }

            ecb.Playback(state.EntityManager);
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
            _prefabsBuffer = _em.GetBuffer<PrefabBufferElements>(_prefabsComponentEntity);
            var selectedPrefab = _prefabsBuffer.ElementAt(index).PrefabEntity;
            ecb.SetComponent(_gameStateEntity, new BuildingStateComponent
            {
                SelectedPrefab = selectedPrefab,
                SelectedPrefabID = index
            });

            // 2. open ApplyPanel
            ApplyPanelUI.ApplyPanelLabel.text = "Build " + "b-" + index + "?";
            ecb.AddComponent(_gameStateEntity,
                new ComponentTypeSet(
                    typeof(BSApplyPanelComponent),
                    typeof(BSApplyPanelShowTag)
                ));

            // 3. instantiate prefab

            // мутим инстанс , накидываем что это билдинг, и ток потом накидываем компонент для плэйса
            var instance = ecb.Instantiate(selectedPrefab);
            var coordsForPlace = SystemAPI.GetSingleton<ScreenCenterInWorldCoordsComponent>().ScreenCenterToWorld;

            ecb.AddComponent(instance, new BuildingDetailsComponent
            {
                entity = instance,
                prefab = selectedPrefab,
                id = index,
                position = new float3(0, 0, 0)
            });
            ecb.AddComponent(instance, new PlaceBuildingComponent
            {
                placePosition = coordsForPlace
            });
            // ecb.AddComponent<>();

            // 4. add to prefab component with details
            // 5. add to prefab component PlaceBuilding
            // 6. set temp data/properties to PlaceBuilding
            // 7. set prefab position to place

            // Debug.Log($"button ID : {index} /// {button}");


            ecb.Playback(_em);
            ecb.Dispose();
        }

        private void OnCancelBtnClicked()
        {
            Debug.Log("cancel button");
            // 1. 
        }

        private void OnApplyBtnClicked()
        {
            Debug.Log("apply button");
        }

        public void OnDestroy(ref SystemState state)
        {
            ApplyPanelUI.ApplyPanelCancelButton.clicked -= OnCancelBtnClicked;
            ApplyPanelUI.ApplyPanelApplyButton.clicked -= OnApplyBtnClicked;
            BuildingPanelUI.OnBuildSelected -= PlaceBuilding;
        }
    }
}