using Jrd.GameStates;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.DebSet
{
    [UpdateBefore(typeof(GameStatesSystem))]
    public partial struct DebSetSystem : ISystem
    {
        private Entity _entity;
        private EntityManager _em;
        private EntityCommandBuffer _ecb;
        private bool _isSubscribed;

        private Entity _gameStateEntity;
        private RefRW<GameStateData> _gameStateData;


        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameStateData>();
            _em = state.EntityManager;
            var archetype = _em.CreateArchetype(typeof(DebSetComponent));
            _entity = _em.CreateEntity(archetype);
            _em.SetName(_entity, "_DebSetEntity");

            _gameStateEntity = SystemAPI.GetSingletonEntity<GameStateData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Debug.Log("deb set system");
            _ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            _em = state.EntityManager;
            _gameStateData = SystemAPI.GetComponentRW<GameStateData>(_gameStateEntity); // TODO aspect


            if (_isSubscribed) return;
            DebSetUI.DebSetApplyButton.clicked += ApplyDebSettings;
            DebSetUI.DebSetClearLogButton.clicked += () => DebSetUI.DebSetText.text = "";
            DebSetUI.BModeButton.clicked += StartBuildingMode; // TODO
            DebSetUI.BModeButtonOff.clicked += StopBuildingMode; // TODO
            _isSubscribed = true;
        }

        private void StartBuildingMode()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            ecb.AddComponent<SetStateComponent>(_gameStateEntity);
            ecb.SetComponent(_gameStateEntity, new SetStateComponent { _gameState = GameState.BuildingState });
            ecb.Playback(_em);
            ecb.Dispose();
        }

        private void StopBuildingMode()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            ecb.AddComponent<SetStateComponent>(_gameStateEntity);
            ecb.SetComponent(_gameStateEntity, new SetStateComponent { _gameState = GameState.GamePlayState });
            var e = _em.GetComponentData<GameStateData>(_gameStateEntity);
            ecb.AddComponent<DeactivateTag>(e.BuildingStateEntity); // TODO подумать
            ecb.Playback(_em);
            ecb.Dispose();
        }

        private void ApplyDebSettings()
        {
            H.T("ApplyDebSettings");
            _ecb.SetComponent(_entity, new DebSetComponent
            {
                MouseRaycast = DebSetUI.IsMouseRaycast
            });
        }
    }
}