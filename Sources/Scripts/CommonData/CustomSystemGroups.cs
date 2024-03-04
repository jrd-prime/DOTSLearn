using Unity.Entities;

namespace Sources.Scripts.CommonData
{
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    public partial class JSimulationSystemGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(JSimulationSystemGroup))]
    public partial class JInitSimulationSystemGroup : ComponentSystemGroup
    {
    }

    [UpdateInGroup(typeof(JSimulationSystemGroup))]
    [UpdateAfter(typeof(JInitSimulationSystemGroup))]
    public partial class JDefaultSimulationSystemGroup : ComponentSystemGroup
    {
    }
}