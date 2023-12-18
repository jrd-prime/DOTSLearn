using Jrd.DebSet;
using Jrd.GameStates.BuildingState;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.GameStates
{
    public partial struct GameStatesSystem : ISystem
    {
        private GameState _gameState;
        private EntityManager _em;
        private Entity _gameStateEntity;
        private RefRW<GameStateData> _gameStateData;
        private BeginInitializationEntityCommandBufferSystem.Singleton biEcb;
        private EntityCommandBuffer ecb;

        private Entity _buildingStateEntity;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            biEcb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            
            var em = state.EntityManager;
            _gameStateEntity = em.CreateEntity();
            em.SetName(_gameStateEntity, "___ Game State Entity");
            em.AddComponent<GameStateData>(_gameStateEntity);
            em.SetComponentData(_gameStateEntity, new GameStateData
            {
                Self = _gameStateEntity,
                GameState = GameState.GamePlayState,
                BuildingStateEntity = Entity.Null
            });
        }

        public void OnUpdate(ref SystemState state)
        {
            _em = state.EntityManager;
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            ecb = biEcb.CreateCommandBuffer(state.WorldUnmanaged);

            _gameState = SystemAPI.GetComponent<GameStateData>(_gameStateEntity).GameState;
            _gameStateData = SystemAPI.GetComponentRW<GameStateData>(_gameStateEntity); // TODO aspect

            Debug.Log(_gameState);

            switch (_gameState)
            {
                case GameState.BuildingState:
                    if (_buildingStateEntity == Entity.Null)
                    {
                        _buildingStateEntity = InitStateForSystem<BuildingStateSystem>(ecb, new ComponentTypeSet(
                                typeof(BuildingStateComponent)),
                            BSConst.BuildingStateEntityName);
                        _gameStateData.ValueRW.BuildingStateEntity = _buildingStateEntity;
                    }

                    break;
                case GameState.GamePlayState:
                    if (_buildingStateEntity != Entity.Null)
                    {
                        DisposeStateForSystem<BuildingStateSystem>(ecb, _buildingStateEntity, ref state);
                        _buildingStateEntity = Entity.Null;
                        _gameStateData.ValueRW.BuildingStateEntity = Entity.Null;
                    }

                    break;
                default:
                    break;
            }
        }

        private void DisposeStateForSystem<T>(EntityCommandBuffer ecb1, Entity stateEntity, ref SystemState state)
            where T : unmanaged, ISystem
        {
            
            ecb.DestroyEntity(stateEntity);
            SetUnmanagedSystemEnabled<T>(false);
        }

        private Entity InitStateForSystem<T>(EntityCommandBuffer ecb1, ComponentTypeSet typeSet,
            FixedString32Bytes name)
            where T : unmanaged, ISystem
        {
            SetUnmanagedSystemEnabled<T>(true); // system enable
            
            var entity = _em.CreateEntity(); // create entity // TODO
            ecb.AddComponent<InitializeTag>(entity);
            ecb.AddComponent(entity, typeSet); // add components
            ecb.SetName(entity, $"{BSConst.Prefix} {name}");
            
            return entity;
        }

        private void SetUnmanagedSystemEnabled<T>(bool enabled) where T : unmanaged, ISystem
        {
            var handle = World.DefaultGameObjectInjectionWorld.GetExistingSystem<T>();
            World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(handle).Enabled = enabled;
        }
    }
}