using Jrd.GameStates.BuildingState;
using Jrd.JUI;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.GameStates
{
    public partial struct BuildingPanelSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // SHOW
            foreach (var (q, entity) in SystemAPI.Query<RefRW<VisibilityComponent>>()
                         .WithAll<BuildingPanelComponent, ShowVisualElementTag>()
                         .WithEntityAccess())
            {
                BuildingPanelUI.ShowBPanel();
                q.ValueRW.IsVisible = true;
                ecb.RemoveComponent<ShowVisualElementTag>(entity);
            }

            // HIDE
            foreach (var (q, entity) in SystemAPI.Query<RefRW<VisibilityComponent>>()
                         .WithAll<BuildingPanelComponent, HideVisualElementTag>()
                         .WithEntityAccess())
            {
                BuildingPanelUI.HideBPanel();
                q.ValueRW.IsVisible = false;
                ecb.RemoveComponent<HideVisualElementTag>(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}