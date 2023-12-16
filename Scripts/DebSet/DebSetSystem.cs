using Jrd.GameStates;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.DebSet
{
    public partial struct DebSetSystem : ISystem
    {
        private Entity _entity;
        private EntityManager _em;
        private EntityCommandBuffer _ecb;
        private bool _isSubscribed;

        private Entity bmodeEntity;

        private Entity _gameStateEntity;


        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameStateData>();
            _em = state.EntityManager;
            bmodeEntity = _em.CreateEntity();
            var archetype = _em.CreateArchetype(typeof(DebSetComponent));
            _entity = _em.CreateEntity(archetype);
            _em.SetName(_entity, "_DebSetEntity");

            _gameStateEntity = SystemAPI
                .GetComponent<GameStateData>(state.World.GetExistingSystem(typeof(GameStatesSystem)))
                .GameStateEntity;
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = new EntityCommandBuffer(Allocator.Temp);

            _ecb.Playback(_em);
            _ecb.Dispose();
            if (_isSubscribed) return;
            DebSetUI.DebSetApplyButton.clicked += ApplyDebSettings;
            DebSetUI.DebSetClearLogButton.clicked += () => DebSetUI.DebSetText.text = "";
            DebSetUI.BModeButton.clicked += StartBuildingMode; // TODO
            DebSetUI.BModeButtonOff.clicked += StopBuildingMode; // TODO
            _isSubscribed = true;
        }

        private void StartBuildingMode()
        {
            _ecb = new EntityCommandBuffer(Allocator.Temp);
            // _ecb.AddComponent<BuildingStateComponent>(_gameStateEntity); // TODO
            // _ecb.AddComponent<InitializeTag>(_gameStateEntity); // TODO
            _ecb.Playback(_em);
        }

        private void StopBuildingMode()
        {
            _ecb = new EntityCommandBuffer(Allocator.Temp);
            // _ecb.AddComponent<DeactivateStateTag>(_gameStateEntity); // TODO
            _ecb.Playback(_em);
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