using Sources.Scripts.CommonComponents.test;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Production
{
    public unsafe struct ProductionProcessDataWrapper
    {
        public EntityCommandBuffer Ecb { get; set; }
        public BuildingDataAspect Aspect { get; set; }
        public ProductionMethods* ProductionMethodsPtr { get; set; }
    }
}