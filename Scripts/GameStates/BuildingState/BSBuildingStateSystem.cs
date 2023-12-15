using Jrd.Build.EditModePanel;
using Jrd.GameStates.BuildingState.Tag;
using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingStateComponent>();
            _isSubscribed = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            _gameStateEntity = SystemAPI
                .GetComponent<GameStateData>(state.World.GetExistingSystem(typeof(GameStatesSystem)))
                .GameStateEntity;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

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
            ApplyPanelUI.ApplyPanelCancelButton.clicked += OnCancelBtnClicked;
            ApplyPanelUI.ApplyPanelApplyButton.clicked += OnApplyBtnClicked;
            _isSubscribed = true;
        }
        
        
        private void OnCancelBtnClicked()
        {
            // 1. 
        }

        private void OnApplyBtnClicked()
        {
        }

        public void OnDestroy(ref SystemState state)
        {
            ApplyPanelUI.ApplyPanelCancelButton.clicked -= OnCancelBtnClicked;
            ApplyPanelUI.ApplyPanelApplyButton.clicked -= OnApplyBtnClicked;
        }
    }
}