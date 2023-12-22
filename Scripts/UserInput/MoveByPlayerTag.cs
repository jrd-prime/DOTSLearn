using Unity.Entities;

namespace Jrd.UserInput
{
    /// <summary>
    /// Этот тег говорит о том, что данная сущность ловит юзер инпут для передвижения
    /// </summary>
    public struct MoveByPlayerTag : IComponentData
    {
    }
}