using System;
using Jrd.Gameplay.Building.Production.Component;
using Jrd.Gameplay.Storage.InProductionBox.Component;
using Jrd.Gameplay.Storage.Service;
using Jrd.Gameplay.Timers;
using Jrd.Gameplay.Timers.Component;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Building.Production
{
    public partial struct ProductionSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private Entity _buildingEntity;
        private BuildingDataAspect _buildingData;

        private ProductionProcessData _processDataR;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var aspect in SystemAPI.Query<BuildingDataAspect>())
            {
                _buildingData = aspect;
                _buildingEntity = _buildingData.Self;

                _processDataR = _buildingData.ProductionProcessData.ValueRO;
                
                ProductionState productionState = _buildingData.BuildingData.ProductionState;

                // test
                // productionState = ProductionState.EnoughProducts;

                switch (productionState)
                {
                    case ProductionState.Init:
                        NotEnoughProducts();
                        break;
                    case ProductionState.NotEnoughProducts:
                        NotEnoughProducts();
                        break;
                    case ProductionState.EnoughProducts:
                        EnoughProducts();
                        break;
                    case ProductionState.Started:
                        Started();
                        break;
                    case ProductionState.InProgress:
                        InProgress();
                        break;
                    case ProductionState.Stopped:
                        Stopped();
                        break;
                    case ProductionState.Finished:
                        Finished();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void NotEnoughProducts()
        {
            // Debug.Log("NOT ENOUGH PRODUCTS " + _buildingData.BuildingData.Name);
            // показывать в панели билдинга что нету продуктов для производства в инпродакшн боксе
        }

        private void EnoughProducts()
        {
            Debug.Log("ENOUGH PRODUCTS " + _buildingData.BuildingData.Name);

            // 0 проверить есть ли доступное место в боксе произведенных для старта нового цикла
            // если нет уведомить/показать что надо освободить место в боксе произведенных
            if (!ManufacturedService.IsEnoughSpaceInManufacturedBox())
            {
                Debug.LogWarning("Not enough space in manufactured box!");
            }

            // 1 рассчитать продукты
            // var loadedProducts = _buildingData.ProductionProcessData.PreparedProducts;

            // 2 понять за сколько раз переработаем
            // var maxLoads = _buildingData.ProductionProcessData.MaxLoads;

            // 3 какие тайминги
            int oneProductTime = _buildingData.GetOneProductManufacturingTime();
            Debug.Log("oneProductTime " + oneProductTime + " sec");

            int loadedProductsTime = _buildingData.GetLoadedProductsManufacturingTime();
            Debug.Log("loadedTime " + loadedProductsTime + " sec");

            _buildingData.SetProductionState(ProductionState.Started);
        }

        private void Started()
        {
            Debug.Log("STARTED " + _buildingData.BuildingData.Name);
            // Set timers
            _buildingData.SetFullLoadedProductsTimer();
            _buildingData.SetOneProductTimer();
            // Start timers
            new JTimer().StartNewTimer(
                _buildingEntity,
                TimerType.OneProduct,
                _buildingData.GetOneProductManufacturingTime(),
                _ecb);

            new JTimer().StartNewTimer(
                _buildingEntity,
                TimerType.AllProducts,
                _buildingData.GetLoadedProductsManufacturingTime(),
                _ecb);

            // взять часть продуктов для 1 прогона
            _buildingData.ChangeProductsQuantity(new ChangeProductsQuantityData
            {
                StorageType = StorageType.InProduction,
                ChangeType = ChangeType.Reduce,
                ProductsData = _buildingData.RequiredProductsData.Required
            });

            // обновить юай inproduction
            _buildingData.AddEvent(BuildingEvent.InProductionBoxDataUpdated);

            // обновить юай таймеров
            // TODO handle
            _ecb.AddComponent<ProductionTimersUpdatedEvent>(_buildingEntity);

            // таймер юай на 0


            const int cycle = 1;
            _buildingData.ProductionProcessData.ValueRW.CurrentCycle = cycle;
            _buildingData.ProductionProcessData.ValueRW.RemainingCycles = _processDataR.MaxLoads - cycle;

            _buildingData.SetProductionState(ProductionState.InProgress);
        }

        private void InProgress()
        {
            Debug.Log("IN PROGRESS " + _buildingData.BuildingData.Name);
            Debug.Log("current = " + _processDataR.CurrentCycle);
            Debug.Log("remaining = " + _processDataR.RemainingCycles);

            _ecb.AddComponent<ProductionTimersUpdatedEvent>(_buildingEntity);

            

            // сетить и обновлять таймеры
            // когда заканчивается один цикл - обновить юай произмеденных
            // - взять еще продукты из продакшн бокса для наового цикла и обновить юай

            // проверить есть ли мето в произведенных для нвого цикла

            // если нету места для нового цикла, то стэйт в стоппед и показать кведомление над зданием
            // СОХРАНИТЬ ТАЙМЕРЫ И КОЛВО ПРОДУКТОВ ЕСЛИ ПЕРЕХОДИМ В СТОП ИЗ-ЗА ПОЛНОГО СКЛАДА
        }

        private void Stopped()
        {
            Debug.Log("STOPPED " + _buildingData.BuildingData.Name);

            // ожидание когда освободится место на складе и как-то продолжить цикл
        }

        private void Finished()
        {
            Debug.Log("FINISHED " + _buildingData.BuildingData.Name);

            // обновить все юаи продакшена 
            // показать/уведомить что задание завершено
        }
    }
}