using System;
using Jrd.Gameplay.Building.Production.Component;
using Jrd.Gameplay.Storage;
using Jrd.Gameplay.Storage._3_InProduction.Component;
using Jrd.Gameplay.Storage.Service;
using Jrd.Gameplay.Timers;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Building.Production
{
    public partial struct BuildingProductionSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private Entity _entity;
        private BuildingDataAspect _aspect;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI
                .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var aspect in SystemAPI.Query<BuildingDataAspect>())
            {
                _entity = aspect.Self;
                _aspect = aspect;

                ProductionState productionState = aspect.BuildingData.ProductionState;

                // test
                // productionState = ProductionState.EnoughProducts;

                switch (productionState)
                {
                    case ProductionState.Init:
                    case ProductionState.NotEnoughProducts:
                        Debug.Log("NotEnoughProducts " + aspect.BuildingData.Name);
                        // показывать в панели билдинга что нету продуктов для производства в инпродакшн боксе
                        break;

                    case ProductionState.EnoughProducts:
                        EnoughProducts();
                        aspect.SetProductionState(ProductionState.Started);
                        break;

                    case ProductionState.Started:
                        Started();
                        aspect.SetProductionState(ProductionState.InProgress);
                        break;

                    case ProductionState.InProgress:
                        Debug.Log("InProgress " + aspect.BuildingData.Name);

                        _ecb.AddComponent<ProductionTimersUpdatedEvent>(_entity);

                        // сетить и обновлять таймеры
                        // когда заканчивается один цикл - обновить юай произмеденных
                        // - взять еще продукты из продакшн бокса для наового цикла и обновить юай

                        // проверить есть ли мето в произведенных для нвого цикла

                        // если нету места для нового цикла, то стэйт в стоппед и показать кведомление над зданием
                        // СОХРАНИТЬ ТАЙМЕРЫ И КОЛВО ПРОДУКТОВ ЕСЛИ ПЕРЕХОДИМ В СТОП ИЗ-ЗА ПОЛНОГО СКЛАДА


                        break;
                    case ProductionState.Stopped:
                        Debug.Log("Stopped " + aspect.BuildingData.Name);

                        // ожидание когда освободится место на складе и как-то продолжить цикл
                        break;
                    case ProductionState.Finished:
                        Debug.Log("Finished " + aspect.BuildingData.Name);

                        // обновить все юаи продакшена 
                        // показать/уведомить что задание завершено
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Started()
        {
            Debug.Log("Started " + _aspect.BuildingData.Name);
            // Set timers
            _aspect.SetAllProductsTimer();
            _aspect.SetOneProductTimer();

            // Start timers
            _ecb.AddComponent(_entity, new OneLoadedProductTimerData
            {
                Value = _aspect.GetOneProductManufacturingTime()
            });
            _ecb.AddComponent(_entity, new AllLoadedProductsTimerData
            {
                Value = _aspect.GetLoadedProductsManufacturingTime()
            });

            // взять часть продуктов для 1 прогона
            _ecb.AddComponent(_entity, new ChangeProductsQuantityData
            {
                StorageType = StorageType.InProduction,
                ChangeType = ChangeType.Reduce,
                ProductsData = _aspect.RequiredProductsData.Required
            });

            // обновить юай inproduction
            _ecb.AddComponent<InProductionDataUpdatedEvent>(_entity);

            // обновить юай таймеров
            // TODO handle
            _ecb.AddComponent<ProductionTimersUpdatedEvent>(_entity);


            // таймер юай на 0

            // стэйт что в процессе
        }

        private void EnoughProducts()
        {
            Debug.Log("EnoughProducts " + _aspect.BuildingData.Name);
            {
                // 0 проверить есть ли доступное место в боксе произведенных для старта нового цикла
                // если нет уведомить/показать что надо освободить место в боксе произведенных
                if (!ManufacturedService.IsEnoughSpaceInManufacturedBox())
                {
                    Debug.LogWarning("Not enough space in manufactured box!");
                }

                Debug.Log(
                    $"req for manuf = {StorageService.GetProductsQuantity(_aspect.ManufacturedProductsData.Manufactured)}");
                Debug.Log(
                    $"manuf capacity = {_aspect.BuildingData.MaxStorage}");
            }

            // 1 рассчитать продукты
            var loadedProducts = _aspect.ProductionProcessData.PreparedProducts;

            // 2 понять за сколько раз переработаем
            var maxLoads = _aspect.ProductionProcessData.MaxLoads;

            // 3 какие тайминги
            int oneProductTime = _aspect.GetOneProductManufacturingTime();
            Debug.Log("oneProductTime " + oneProductTime + " sec");

            int loadedProductsTime = _aspect.GetLoadedProductsManufacturingTime();
            Debug.Log("loadedTime " + loadedProductsTime + " sec");

            // запустить производство
            _ecb.AddComponent<StartProductionTag>(_entity);
        }
    }
}