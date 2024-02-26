using GamePlay.Common;
using GamePlay.GameStates.BuildingState;
using GamePlay.InitSystems;
using Unity.Collections;
using Unity.Entities;

namespace GamePlay.GameStates.MainGameState
{
    [UpdateAfter(typeof(MyInitSystemGroup))]
    public partial struct GameStatesSystem : ISystem
    {
        private GameState _gameState;
        private EntityManager _em;
        private Entity _gameStateEntity;
        private RefRW<GameStateData> _gameStateData;
        private BeginInitializationEntityCommandBufferSystem.Singleton _ecbSystem;
        private EntityCommandBuffer _biEcb;
        public static readonly FixedString32Bytes Prefix = "___";
        private Entity _buildingStateEntity;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();

            _em = state.EntityManager;
        }

        public void OnUpdate(ref SystemState state)
        {
            // _ecbSystem = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            // _biEcb = _ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            // _gameState = SystemAPI.GetComponent<GameStateData>(_gameStateEntity).CurrentGameState;
            // _gameStateData = SystemAPI.GetComponentRW<GameStateData>(_gameStateEntity); // TODO aspect

            // switch (_gameState)
            // {
            //     case GameState.BuildingState:
            //         if (_buildingStateEntity == Entity.Null)
            //         {
            //             // Debug.Log("we create");
            //             // _buildingStateEntity = InitState(new ComponentTypeSet(
            //             //         typeof(BuildingStateComponent), typeof(BuildingStateData)),
            //             //     BSConst.BuildingStateEntityName);
            //             _gameStateData.ValueRW.BuildingStateEntity = _buildingStateEntity;
            //         }
            //
            //         break;
            //     case GameState.GamePlayState:
            //         if (_buildingStateEntity != Entity.Null)
            //         {
            //             _buildingStateEntity = Entity.Null;
            //             _gameStateData.ValueRW.BuildingStateEntity = Entity.Null;
            //         }
            //
            //         break;
            //     default:
            //         break;
            // }
        }


        private Entity InitState(ComponentTypeSet typeSet, FixedString32Bytes name)
        {
            var entity = _em.CreateEntity(); // create entity // TODO
            _biEcb.AddComponent<InitializeTag>(entity);
            _biEcb.AddComponent(entity, typeSet); // add components
            _biEcb.SetName(entity, $"{Prefix} {name}");
            return entity;
        }

        private void SetUnmanagedSystemEnabled<T>(bool enabled) where T : unmanaged, ISystem
        {
            var handle = World.DefaultGameObjectInjectionWorld.GetExistingSystem<T>();
            World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(handle).Enabled = enabled;
        }


        private void DisposeStateForSystem(EntityCommandBuffer ecb1, Entity stateEntity, ref SystemState state)
        {
            foreach (var e in SystemAPI.GetComponent<BuildingStateComponent>(stateEntity)
                         .BuildingStateComponentEntities)
            {
                _biEcb.DestroyEntity(e);
            }

            SystemAPI.GetComponent<BuildingStateComponent>(stateEntity)
                .BuildingStateComponentEntities.Dispose();
            // _biEcb.AddComponent<DeactivateTag>(stateEntity);
            _biEcb.DestroyEntity(stateEntity);
        }
    }
}