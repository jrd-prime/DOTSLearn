using Unity.Entities;
using UnityEngine.SocialPlatforms;

namespace Jrd.UserInput
{
    public partial struct FollowSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var query in SystemAPI.Query<RefRW<FollowComponent>>())
            {
                // query.ValueRW.FollowTarget
                
                
            }
        }
    }
}