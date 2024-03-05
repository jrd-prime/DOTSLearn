namespace Sources.Scripts.Game.Features.Building.Production
{
    public interface IProductionStateProvider
    {
        public void Run(ProductionProcessDataWrapper data);
    }
}