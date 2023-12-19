using Unity.Entities;

namespace Jrd.GameStates.SharedComponentAndSystems
{
    public partial struct VisibilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var _em = state.EntityManager;

            foreach (var q in SystemAPI
                         .Query<RefRW<VisibilityComponent>>()
                         // .WithAll<UIRootElementComponent>()
                         .WithEntityAccess())
            {
                
            }
        }
    }
}