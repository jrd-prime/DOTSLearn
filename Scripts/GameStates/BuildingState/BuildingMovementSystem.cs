﻿namespace Jrd.GameStates.BuildingState
{
    // public partial struct BuildingMovementSystem : ISystem
    // {
    //     public void OnCreate(ref SystemState state)
    //     {
    //         state.RequireForUpdate<TempBuildPrefabComponent>();
    //     }
    //
    //     public void OnUpdate(ref SystemState state)
    //     {
    //         var em = state.EntityManager;
    //
    //         var a = SystemAPI.GetSingletonEntity<TempBuildPrefabComponent>();
    //         var e = em.GetComponentData<LocalTransform>(a);
    //         
    //
    //         foreach (var q in SystemAPI.Query<RefRW<MovingEventComponent>, TempBuildPrefabComponent>())
    //         {
    //             Debug.Log("Building system");
    //
    //             var an = (Quaternion.AngleAxis(30f, Vector3.up)) * q.Item1.ValueRO.direction;
    //             var pos = e.Position + (float3)an * -1.2f;
    //
    //             pos = new float3(Mathf.Round(pos.x), 0, Mathf.Round(pos.z));
    //             Debug.Log("NEW POS " + pos);
    //
    //             em.SetComponentData(a,
    //                 new LocalTransform
    //                 {
    //                     Position = pos,
    //                     Rotation = quaternion.identity,
    //                     Scale = 1
    //                 });
    //         }
    //     }
    // }
}