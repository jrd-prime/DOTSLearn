using Unity.Entities;

namespace Jrd.GameStates
{
    public struct VisibilityComponent : IComponentData
    {
        public bool IsVisible;
    }
}