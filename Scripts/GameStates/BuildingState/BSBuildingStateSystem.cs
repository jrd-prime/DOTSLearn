using Jrd.Build.EditModePanel;
using Jrd.GameStates.BuildingState.Tag;
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

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingStateTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _gameStateEntity = SystemAPI
                .GetComponent<GameStateData>(state.World.GetExistingSystem(typeof(GameStatesSystem)))
                .GameStateEntity;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var unused in SystemAPI.Query<BuildingStateTag, InitializeTag>())
            {
                Debug.Log("initialize ".ToUpper() + GetType());

                ecb.AddComponent(_gameStateEntity,
                    new ComponentTypeSet(
                        typeof(BSBuildingsPanelComponent),
                        typeof(BSBuildingsPanelShowTag),
                        typeof(BSApplyPanelComponent),
                        typeof(BSApplyPanelShowTag)
                    ));
                ecb.RemoveComponent<InitializeTag>(_gameStateEntity);
            }

            foreach (var unused in SystemAPI.Query<BuildingStateTag, DeactivateStateTag>())
            {
                Debug.Log("deactivate ".ToUpper() + GetType());

                ecb.AddComponent<BSBuildingsPanelHideTag>(_gameStateEntity); // TODO
                ecb.AddComponent<BSApplyPanelHideTag>(_gameStateEntity); // TODO
                ecb.RemoveComponent<BuildingStateTag>(_gameStateEntity); // TODO
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
        }
    }
}