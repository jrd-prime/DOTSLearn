using Unity.Entities;
using Unity.Transforms;

public struct CameraComponent : IComponentData
{
    public LocalTransform tr;
    public Entity prefab;
}
