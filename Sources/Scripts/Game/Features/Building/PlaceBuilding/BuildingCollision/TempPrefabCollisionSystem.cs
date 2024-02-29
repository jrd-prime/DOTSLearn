using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding.BuildingCollision
{
    // [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    // [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial struct TempPrefabCollisionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // state.RequireForUpdate<CollisionEventImpulse>();
            state.RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new CollisionEventImpulseJob
            {
                // CollisionEventImpulseData = SystemAPI.GetComponentLookup<CollisionEventImpulse>(),
                PhysicsVelocityData = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        }

        [BurstCompile]
        struct CollisionEventImpulseJob : ICollisionEventsJob
        {
            // [ReadOnly] public ComponentLookup<CollisionEventImpulse> CollisionEventImpulseData;
            public ComponentLookup<PhysicsVelocity> PhysicsVelocityData;

            [BurstDiscard]
            public void Execute(CollisionEvent collisionEvent)
            {
                Entity entityA = collisionEvent.EntityA;
                Entity entityB = collisionEvent.EntityB;

                bool isBodyADynamic = PhysicsVelocityData.HasComponent(entityA);
                bool isBodyBDynamic = PhysicsVelocityData.HasComponent(entityB);

                Debug.Log(isBodyADynamic);
                Debug.Log(isBodyBDynamic);

                // bool isBodyARepulser = CollisionEventImpulseData.HasComponent(entityA);
                // bool isBodyBRepulser = CollisionEventImpulseData.HasComponent(entityB);

                // if (isBodyARepulser && isBodyBDynamic)
                // {
                //     var impulseComponent = CollisionEventImpulseData[entityA];
                //     var velocityComponent = PhysicsVelocityData[entityB];
                //     velocityComponent.Linear = impulseComponent.Impulse;
                //     PhysicsVelocityData[entityB] = velocityComponent;
                // }

                // if (isBodyBRepulser && isBodyADynamic)
                // {
                //     var impulseComponent = CollisionEventImpulseData[entityB];
                //     var velocityComponent = PhysicsVelocityData[entityA];
                //     velocityComponent.Linear = impulseComponent.Impulse;
                //     PhysicsVelocityData[entityA] = velocityComponent;
                // }
            }
        }
    }
}