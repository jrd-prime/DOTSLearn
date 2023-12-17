using Jrd.JUI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.GameStates.BuildingState
{
    public partial struct BuildingStateSystem : ISystem
    {
        private EntityManager _em;

        private Entity _buildingPanel;
        private Entity _confirmationPanel;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingStateComponent>();
            state.Enabled = false;
            BuildingPanelUI.OnBuildSelected += BuildSelected;
        }

        public void OnUpdate(ref SystemState state)
        {
            _em = state.EntityManager;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entity) in SystemAPI
                         .Query<InitializeTag>()
                         .WithAll<BuildingStateComponent>()
                         .WithEntityAccess())
            {
                Debug.Log("BuildingStateComponent + InitializeTag");

                // create panel entity
                _buildingPanel = GetCustomEntity<BuildingPanelComponent>(ecb, BSConst.BuildingPanelEntityName);

                // create confirmation panel entity
                _confirmationPanel =
                    GetCustomEntity<ConfirmationPanelComponent>(ecb, BSConst.ConfirmationPanelEntityName);

                ecb.RemoveComponent<InitializeTag>(entity);
            }


            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            BuildingPanelUI.OnBuildSelected -= BuildSelected;
        }

        private void BuildSelected(Button button, int index)
        {
            Debug.Log($"Build Selected : {button.name} + {index}");
        }

        private Entity GetCustomEntity<T>(EntityCommandBuffer ecb, FixedString64Bytes entityName)
            where T : unmanaged, IComponentData
        {
            var entity = ecb.CreateEntity();
            var nameWithPrefix = BSConst.Prefix + " " + entityName;
            ecb.AddComponent<T>(entity);
            ecb.SetName(entity, nameWithPrefix);
            return entity;
        }
    }
}