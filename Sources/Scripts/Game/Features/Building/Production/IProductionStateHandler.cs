namespace Sources.Scripts.Game.Features.Building.Production
{
    public interface IProductionStateHandler
    {
        public void Run(ProductionProcessDataWrapper data);
    }
}