using System;
using Jrd.Gameplay.Building.Production.Component;
using Jrd.Gameplay.Storage.InProductionBox.Component;
using Jrd.Gameplay.Storage.Service;
using Jrd.Gameplay.Timers;
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

        private int _tempCycle;

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
                var production = new ProductionManager(_buildingData);

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
                        StartProduction();
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


        public void StartProduction()
        {
            Debug.Log("--- START PRODUCTION --- " + _buildingData.BuildingData.Name);

            _tempCycle = 1;

            // Timer
            StartFullLoadTimer();
            StartOneLoadTimer();

            // Product
            TakeProductsForOneLoad();

            _buildingData.ProductionProcessData.ValueRW.CurrentCycle = _tempCycle;
            _buildingData.ProductionProcessData.ValueRW.RemainingCycles =
                _buildingData.ProductionProcessData.ValueRO.MaxLoads - _tempCycle;

            // State
            SetNewState(ProductionState.InProgress);
        }

        private void InProgress()
        {
            Debug.Log("--- IN PROGRESS PRODUCTION --- " + _buildingData.BuildingData.Name);
            Debug.Log("temp cycle = " + _tempCycle);
            Debug.Log("current cycle = " + _processDataR.CurrentCycle);

            if (_buildingData.ProductionProcessData.ValueRW.LastCycleEnd) SetNewState(ProductionState.Finished);
            
            if (_tempCycle != _processDataR.CurrentCycle)
            {
                PutProductsFromOneLoad();

                Debug.Log("temp != curr / new cycle?");

                if (_tempCycle != _buildingData.ProductionProcessData.ValueRO.MaxLoads)
                {
                    Debug.Log("new timer");
                    TakeProductsForOneLoad();
                    StartOneLoadTimer();
                    _tempCycle = _processDataR.CurrentCycle;
                }
                else
                {
                    Debug.Log("temp = max load");
                }
            }

            // if (_tempCycle != _processDataR.CurrentCycle)
            // {
            //     Debug.Log("temp != curr / new cycle?");
            //
            //     if (_tempCycle != _buildingData.ProductionProcessData.ValueRO.MaxLoads)
            //     {
            //         Debug.Log("new timer");
            //         StartOneLoadTimer();
            //         _tempCycle = _processDataR.CurrentCycle;
            //     }
            //     else
            //     {
            //         Debug.Log("temp = max load");
            //     }
            // }
            // _tempCycle = _processDataR.CurrentCycle;
            //
            //
            // _ecb.AddComponent(_buildingEntity,
            //     new AddEventToBuildingData { Value = BuildingEvent.ProductionTimersInProgressUpdate });
            //
            // #region CYCLE
            //
            // if (_tempCycle == _processDataR.MaxLoads)
            // {
            //     Debug.Log(_processDataR.CurrentCycle + "// LAST CYCLE in progress!");
            //     // Last cycle
            //     PutProductsFromOneLoad();
            //     _buildingData.SetProductionState(ProductionState.Finished);
            // }
            // else if (_tempCycle != 1 && _tempCycle != _processDataR.MaxLoads)
            // {
            //     Debug.Log(_processDataR.CurrentCycle + "// CYCLE in progress!");
            //     // Middle cycle
            //
            //     StartOneLoadTimer();
            //     PutProductsFromOneLoad();
            //     TakeProductsForOneLoad();
            // }

            // _buildingData.AddEvent(BuildingEvent.InProductionBoxDataUpdated);
            // _buildingData.AddEvent(BuildingEvent.ManufacturedBoxDataUpdated);

            // #endregion

            // сетить и обновлять таймеры
            // когда заканчивается один цикл - обновить юай произмеденных
            // - взять еще продукты из продакшн бокса для наового цикла и обновить юай

            // проверить есть ли мето в произведенных для нвого цикла

            // если нету места для нового цикла, то стэйт в стоппед и показать кведомление над зданием
            // СОХРАНИТЬ ТАЙМЕРЫ И КОЛВО ПРОДУКТОВ ЕСЛИ ПЕРЕХОДИМ В СТОП ИЗ-ЗА ПОЛНОГО СКЛАДА
        }

        private void SetNewState(ProductionState value) => _buildingData.SetProductionState(value);

        private void TakeProductsForOneLoad()
        {
            _buildingData.ChangeProductsQuantity(new ChangeProductsQuantityData
            {
                StorageType = StorageType.InProduction,
                ChangeType = ChangeType.Reduce,
                ProductsData = _buildingData.RequiredProductsData.Required
            });
            _buildingData.AddEvent(BuildingEvent.InProductionBoxDataUpdated);
        }

        private void StartOneLoadTimer()
        {
            new JTimer().StartNewTimer(
                _buildingEntity,
                TimerType.OneLoadCycle,
                _buildingData.GetOneProductManufacturingTime(),
                _ecb);
        }

        private void StartFullLoadTimer()
        {
            new JTimer().StartNewTimer(
                _buildingEntity,
                TimerType.FullLoadCycle,
                _buildingData.GetLoadedProductsManufacturingTime(),
                _ecb);
        }

        private void PutProductsFromOneLoad()
        {
            _buildingData.ChangeProductsQuantity(new ChangeProductsQuantityData
            {
                StorageType = StorageType.Manufactured,
                ChangeType = ChangeType.Increase,
                ProductsData = _buildingData.ManufacturedProductsData.Manufactured
            });

            _buildingData.AddEvent(BuildingEvent.ManufacturedBoxDataUpdated);
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
            Debug.Log("oneProductTime " + _buildingData.GetOneProductManufacturingTime() + " sec");
            Debug.Log("loadedTime " + _buildingData.GetLoadedProductsManufacturingTime() + " sec");

            // Set timers
            _buildingData.SetFullLoadedProductsTimer();
            _buildingData.SetOneProductTimer();

            _buildingData.SetProductionState(ProductionState.Started);
        }


        //     _tempCycle = 1;
        //     _buildingData.ProductionProcessData.ValueRW.CurrentCycle = _tempCycle;
        //     _buildingData.ProductionProcessData.ValueRW.RemainingCycles = _processDataR.MaxLoads - _tempCycle;
        //
        //
        //     // FIRST CYCLE
        //     Debug.Log(_tempCycle + "// FIRST CYCLE in progress!");
        //
        //     StartFullLoadTimer();
        //     StartOneLoadTimer();
        //     TakeProductsForOneLoad();
        //
        //     // обновить юай inproduction
        //     _buildingData.AddEvent(BuildingEvent.InProductionBoxDataUpdated);
        //
        //     // обновить юай таймеров
        //     // TODO handle
        //     _ecb.AddComponent(_buildingEntity,
        //         new AddEventToBuildingData { Value = BuildingEvent.ProductionTimersStarted });
        //
        //     _buildingData.SetProductionState(ProductionState.InProgress);


        private void Stopped()
        {
            Debug.Log("STOPPED " + _buildingData.BuildingData.Name);

            // ожидание когда освободится место на складе и как-то продолжить цикл
        }

        private void Finished()
        {
            Debug.Log("FINISHED " + _buildingData.BuildingData.Name);
            PutProductsFromOneLoad();

            SetNewState(ProductionState.NotEnoughProducts);

            // обновить все юаи продакшена 
            // показать/уведомить что задание завершено
        }
    }
}