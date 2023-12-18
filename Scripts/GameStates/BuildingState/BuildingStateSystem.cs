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
        private NativeList<Entity> _createdComponents;
        private EntityCommandBuffer ecb;

        private Entity _buildingPanel;
        private Entity _confirmationPanel;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<BuildingStateComponent>();
            state.Enabled = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            Debug.Log("bs system");
            if (!_createdComponents.IsCreated)
                _createdComponents = new NativeList<Entity>(2, Allocator.Persistent); // TODO подумать

            ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            _em = state.EntityManager;

            foreach (var (_, entity) in SystemAPI
                         .Query<InitializeTag>()
                         .WithAll<BuildingStateComponent>()
                         .WithEntityAccess())
            {
                Debug.Log("BuildingStateComponent + InitializeTag");
                ecb.RemoveComponent<InitializeTag>(entity);


                // create panel entity
                _buildingPanel = GetCustomEntity<BuildingPanelComponent>(ecb, BSConst.BuildingPanelEntityName);
                // create confirmation panel entity
                _confirmationPanel =
                    GetCustomEntity<ConfirmationPanelComponent>(ecb, BSConst.ConfirmationPanelEntityName);

                BuildingPanelUI.OnBuildSelected += BuildSelected;
            }

            foreach (var (_, entity) in SystemAPI
                         .Query<DeactivateTag>()
                         .WithAll<BuildingStateComponent>()
                         .WithEntityAccess())
            {
                Debug.Log("BuildingStateComponent + DeactivateTag");

                DestroyEntities(ecb);

                _createdComponents.Dispose();

                BuildingPanelUI.OnBuildSelected -= BuildSelected;
            }

            if (_createdComponents.IsCreated)
                Debug.Log(_createdComponents.Length);
        }

        private void DestroyEntities(EntityCommandBuffer ecb1)
        {
            foreach (var t in _createdComponents)
            {
                Debug.Log("destroy = " + t);
                ecb.DestroyEntity(t);
            }
        }

        private void BuildSelected(Button button, int index)
        {
            Debug.Log($"Build Selected : {button.name} + {index}");
        }

        private Entity GetCustomEntity<T>(EntityCommandBuffer ecb1, FixedString64Bytes entityName)
            where T : unmanaged, IComponentData
        {
            var entity = _em.CreateEntity(); // TODO
            _createdComponents.Add(entity);
            var nameWithPrefix = BSConst.Prefix + " " + entityName;
            Debug.Log("new entity " + nameWithPrefix + " / " + entity);
            ecb.AddComponent<T>(entity);
            ecb.SetName(entity, nameWithPrefix);

            return entity;
        }
    }
}