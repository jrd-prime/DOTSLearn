using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Product
{
    public partial struct ProductsMoverSystem : ISystem
    {
        private EntityCommandBuffer _ecb;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MoveRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (moveRequestComponent, entity) in SystemAPI
                         .Query<RefRO<MoveRequestComponent>>()
                         .WithEntityAccess())
            {
                _ecb.RemoveComponent<MoveRequestComponent>(entity);
                Debug.LogWarning(
                    "move request from: " + state.EntityManager.GetName(moveRequestComponent.ValueRO.Value));
            }
        }
    }
}