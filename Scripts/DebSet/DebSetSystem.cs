using Jrd.GameStates;
using Jrd.GameStates.MainGameState;
using Unity.Burst;
using Unity.Entities;
using GameStatesSystem = Jrd.GameStates.MainGameState.GameStatesSystem;

namespace Jrd.DebSet
{
    [UpdateBefore(typeof(GameStatesSystem))]
    [BurstCompile]
    public partial class DebSetSystem : SystemBase
    {
        private Entity _entity;
        private EntityCommandBuffer _bsEcb;
        private bool _isSubscribed;
        private Entity _gameStateEntity;
        private RefRW<GameStateData> _gameStateData;

        [BurstCompile]
        protected override void OnCreate()
        {
            RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<GameStateData>(); // LOOK xzxz

            var em = EntityManager;
            _entity = em.CreateEntity(); // LOOK
            em.AddComponent<DebSetComponent>(_entity);
            em.SetName(_entity, "___ Deb Set Entity");
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            _bsEcb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);

            _gameStateEntity = SystemAPI.GetSingletonEntity<GameStateData>();
            _gameStateData = SystemAPI.GetComponentRW<GameStateData>(_gameStateEntity); // TODO aspect

            if (_isSubscribed) return;

            // building mode start
            DebSetUI.BModeButton.clicked += (() =>
            {
                _bsEcb.SetComponent(_gameStateEntity, new ChangeGameStateComponent { GameState = GameState.BuildingState });
            }); // TODO

            // building mode stop
            DebSetUI.BModeButtonOff.clicked += () =>
            {
                _bsEcb.SetComponent(_gameStateEntity, new ChangeGameStateComponent { GameState = GameState.GamePlayState });
            }; // TODO

            DebSetUI.DebSetApplyButton.clicked += () =>
            {
                H.T("ApplyDebSettings");
                _bsEcb.SetComponent(_entity, new DebSetComponent
                {
                    MouseRaycast = DebSetUI.IsMouseRaycast
                });
            };

            DebSetUI.DebSetClearLogButton.clicked += () => { DebSetUI.DebSetText.text = ""; };
            _isSubscribed = true;
        }
    }
}