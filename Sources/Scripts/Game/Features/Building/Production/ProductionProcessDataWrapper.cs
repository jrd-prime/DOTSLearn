
using Unity.Entities;
using BuildingDataAspect = Sources.Scripts.CommonComponents.Building.BuildingDataAspect;

namespace Sources.Scripts.Game.Features.Building.Production
{
    public unsafe struct ProductionProcessDataWrapper
    {
        public EntityCommandBuffer Ecb { get; set; }
        public BuildingDataAspect Aspect { get; set; }
        public ProductionMethods* ProductionMethodsPtr { get; set; }
    }
}