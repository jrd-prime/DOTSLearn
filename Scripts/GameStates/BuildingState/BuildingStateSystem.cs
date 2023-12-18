using Jrd.JUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.GameStates.BuildingState
{
    [UpdateAfter(typeof(GameStatesSystem))]
    public partial struct BuildingStateSystem : ISystem
    {
        private EntityManager _em;
        private Entity _self;
        private NativeList<Entity> _stateComponents;
        private EntityCommandBuffer _eiEcb;

        private Entity _buildingPanel;
        private Entity _confirmationPanel;
        private Entity _gameStateEntity;
        private RefRW<GameStateData> _gameStateData;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameStateData>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.Enabled = false; // TODO под вопросом
            _em = state.EntityManager;
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!_stateComponents.IsCreated)
                _stateComponents = new NativeList<Entity>(2, Allocator.Persistent); // TODO подумать

            _gameStateEntity = SystemAPI.GetSingletonEntity<GameStateData>();
            _gameStateData = SystemAPI.GetComponentRW<GameStateData>(_gameStateEntity); // TODO aspect

            _eiEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);


            // Init by tag
            foreach (var (_, entity) in SystemAPI
                         .Query<InitializeTag>()
                         .WithAll<BuildingStateComponent>()
                         .WithEntityAccess())
            {
                _eiEcb.RemoveComponent<InitializeTag>(entity);
                _self = entity;

                _buildingPanel = GetCustomEntity<BuildingPanelComponent>(BSConst.BuildingPanelEntityName);
                _confirmationPanel = GetCustomEntity<ConfirmationPanelComponent>(BSConst.ConfirmationPanelEntityName);

                if (_stateComponents.Length == 0)
                    Debug.LogWarning("We have a problem with create entities for Building State");

                _eiEcb.AddComponent<ShowVisualElementTag>(_buildingPanel);
                _eiEcb.AddComponent<VisibilityComponent>(_buildingPanel);
                _eiEcb.SetComponent(_buildingPanel, new VisibilityComponent { IsVisible = false });


                BuildingPanelUI.OnBuildSelected += BuildSelected;
            }

            if (_gameStateData.ValueRO.GameState != GameState.BuildingState)
            {
                foreach (var t in _stateComponents)
                {
                    _eiEcb.DestroyEntity(t);
                }

                _stateComponents.Dispose();
                _eiEcb.DestroyEntity(_self);

                BuildingPanelUI.OnBuildSelected -= BuildSelected;
            }
        }

        private void BuildSelected(Button button, int index)
        {
            Debug.Log($"Build Selected : {button.name} + {index}");
        }

        private Entity GetCustomEntity<T>(FixedString64Bytes entityName)
            where T : unmanaged, IComponentData
        {
            var entity = _em.CreateEntity(); // TODO
            _stateComponents.Add(entity);
            var nameWithPrefix = BSConst.Prefix + " " + entityName;
            Debug.Log("new entity " + nameWithPrefix + " / " + entity);
            _eiEcb.AddComponent<T>(entity);
            _eiEcb.SetName(entity, nameWithPrefix);

            return entity;
        }
    }
}