using Unity.Entities;

namespace Jrd.GameStates.SharedComponentAndSystems
{
    public struct VisibilityComponent : IComponentData
    {
        public bool IsVisible;
    }
}