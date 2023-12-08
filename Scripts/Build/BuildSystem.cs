using Jrd.Build.EditModePanel;
using Jrd.JUI.EditModeUI;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Build
{
    public partial class BuildSystem : SystemBase
    {
        private EntityCommandBuffer _ecb;
        private EntityManager _em;
        private bool _isSubscribed;

        private Entity _buildPanelComponent;

        protected override void OnCreate()
        {
            // Debug.Log("BuildSystem cre");
            _ecb = new EntityCommandBuffer(Allocator.Temp);
            _em = EntityManager;

            var buildPanelArchetype = _em.CreateArchetype(typeof(EditModePanelComponent));
            var buildPanelEntity = _ecb.CreateEntity(buildPanelArchetype);
            _ecb.SetName(buildPanelEntity, "_BuildPanelEntity");
            _ecb.Playback(_em);
            _ecb.Dispose();
        }


        protected override void OnStartRunning()
        {
            _buildPanelComponent = SystemAPI.GetSingletonEntity<EditModePanelComponent>();
        }

        protected override void OnUpdate()
        {
            
            // Debug.Log("BuildSystem up");
            if (_isSubscribed) return;
            BuildingPanelUI.Building1.clicked += EnterInEditMode;
            EditModeUI.EditModeCancelButton.clicked += ExitFromEditMode; // LOOK rename
            _isSubscribed = true;
        }

        private void ExitFromEditMode()
        {
            H.T("ExitFromEditMode");
            // Hide edit mode panel
            SystemAPI.SetComponent(_buildPanelComponent, new EditModePanelComponent { ShowPanel = false});
            // Destroy temp building TODO
            // Exit from edit mode state TODO
        }

        private void EnterInEditMode()
        {
            H.T("EnterInEditMode");
            // Enter in edit mode state TODO
            // Show edit mode panel
            SystemAPI.SetComponent(_buildPanelComponent, new EditModePanelComponent { ShowPanel = true});
            // Place temp building TODO
        }
    }
}