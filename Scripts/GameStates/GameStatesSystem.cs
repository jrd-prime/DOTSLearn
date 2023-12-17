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

        private Entity _buiildingStateEntity;

        public void OnCreate(ref SystemState state)
        {
            var em = state.EntityManager;
            _gameStateEntity = em.CreateEntity();
            em.SetName(_gameStateEntity, "___ Game State Entity");
            em.AddComponent<GameStateData>(_gameStateEntity);
            em.SetComponentData(_gameStateEntity, new GameStateData
            {
                self = _gameStateEntity,
                GameState = GameState.GamePlayState
            });
        }

        public void OnUpdate(ref SystemState state)
        {
            _em = state.EntityManager;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            _gameState = SystemAPI.GetComponent<GameStateData>(_gameStateEntity).GameState;

            Debug.Log(_gameState);

            var buildingStateTypeSet = new ComponentTypeSet(
                typeof(BuildingStateComponent),
                typeof(InitializeTag));

            switch (_gameState)
            {
                case GameState.BuildingState:
                    if (_buiildingStateEntity == Entity.Null)
                        _buiildingStateEntity = InitStateForSystem<BuildingStateSystem>(ecb, buildingStateTypeSet,
                            BSConst.BuildingStateEntityName);

                    break;
                case GameState.GamePlayState:
                    if (_buiildingStateEntity != Entity.Null)
                    {
                        DisposeStateForSystem<BuildingStateSystem>(ecb, _buiildingStateEntity, ref state);
                        _buiildingStateEntity = Entity.Null;
                    }

                    break;
                default:
                    break;
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private void DisposeStateForSystem<T>(EntityCommandBuffer ecb, Entity stateEntity, ref SystemState state)
            where T : unmanaged, ISystem
        {
            ecb.DestroyEntity(stateEntity);
            SetUnmanagedSystemEnabled<T>(false);
        }

        private Entity InitStateForSystem<T>(EntityCommandBuffer ecb, ComponentTypeSet typeSet,
            FixedString32Bytes name)
            where T : unmanaged, ISystem
        {
            SetUnmanagedSystemEnabled<T>(true); // system enable
            var entity = _em.CreateEntity(); // create entity
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