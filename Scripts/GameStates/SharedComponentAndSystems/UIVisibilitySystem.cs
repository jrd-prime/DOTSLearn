using Unity.Entities;

namespace Jrd.GameStates.BuildingState
{
    public partial struct UIVisibilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var _em = state.EntityManager;

            foreach (var q in SystemAPI
                         .Query<RefRW<UIVisibilityComponent>>()
                         // .WithAll<UIRootElementComponent>()
                         .WithEntityAccess())
            {
                
            }
        }
    }
}