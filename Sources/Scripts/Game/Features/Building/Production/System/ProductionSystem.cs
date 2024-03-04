using System;
using Sources.Scripts.CommonComponents;
using Sources.Scripts.CommonComponents.Production;
using Sources.Scripts.Game.Features.Building.Events;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox;
using Sources.Scripts.Game.Features.Building.Storage.Service;
using Sources.Scripts.Timer;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Production.System
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial struct ProductionSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private Entity _buildingEntity;
        private BuildingDataAspect _buildingData;
        private ProductionProcessData _processDataR;
        private ProductionProcessData _processDataW;
        private int _cycleInProgress;

        private Production _production;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            _production = new Production();
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
                

                // productionState = ProductionState.EnoughProducts; // test

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

            _cycleInProgress = 1;

            StartFullLoadTimer();
            StartOneLoadTimer();

            _buildingData.AddEvent(BuildingEvent.ProductionTimersStarted);

            TakeProductsForOneLoad();
            UpdateProductionUI();

            _buildingData.SetCurrentCycle(_cycleInProgress);

            SetNewState(ProductionState.InProgress);
        }


        private void InProgress()
        {
            Debug.Log("--- IN PROGRESS PRODUCTION --- " + _buildingData.BuildingData.Name);

            Debug.LogWarning("last cycle end " + _processDataR.LastCycleEnd);
            if (_processDataR.LastCycleEnd) SetNewState(ProductionState.Finished);

            Debug.LogWarning("curr cycle " + _cycleInProgress);

            if (_cycleInProgress != _processDataR.CurrentCycle)
            {
                PutProductsFromOneLoad();
                UpdateManufacturedUI();

                if (_cycleInProgress != _processDataR.MaxLoads)
                {
                    Debug.Log("new timer");
                    TakeProductsForOneLoad();
                    UpdateProductionUI();
                    StartOneLoadTimer();
                    _cycleInProgress = _processDataR.CurrentCycle;
                }
                else
                {
                    Debug.Log("temp = max load");
                }
            }

            // сетить и обновлять таймеры
            // когда заканчивается один цикл - обновить юай произмеденных
            // - взять еще продукты из продакшн бокса для наового цикла и обновить юай

            // проверить есть ли мето в произведенных для нвого цикла

            // если нету места для нового цикла, то стэйт в стоппед и показать кведомление над зданием
            // СОХРАНИТЬ ТАЙМЕРЫ И КОЛВО ПРОДУКТОВ ЕСЛИ ПЕРЕХОДИМ В СТОП ИЗ-ЗА ПОЛНОГО СКЛАДА
        }


        private void NotEnoughProducts()
        {
            Debug.Log("NOT ENOUGH PRODUCTS " + _buildingData.BuildingData.Name);
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

        // ожидание когда освободится место на складе и как-то продолжить цикл
        private void Stopped()
        {
            Debug.Log("STOPPED " + _buildingData.BuildingData.Name);
        }

        private void Finished()
        {
            Debug.Log("FINISHED " + _buildingData.BuildingData.Name);

            PutProductsFromOneLoad();
            UpdateManufacturedUI();

            SetNewState(ProductionState.NotEnoughProducts);

            // обновить все юаи продакшена 
            // показать/уведомить что задание завершено
        }

        #region Methods

        private void SetNewState(ProductionState value) => _buildingData.SetProductionState(value);

        private void TakeProductsForOneLoad() =>
            _buildingData.ChangeProductsQuantity(new ChangeProductsQuantityData
            {
                StorageType = StorageType.InProduction,
                ChangeType = ChangeType.Reduce,
                ProductsData = _buildingData.RequiredProductsData.Value
            });

        private void PutProductsFromOneLoad() =>
            _buildingData.ChangeProductsQuantity(new ChangeProductsQuantityData
            {
                StorageType = StorageType.Manufactured,
                ChangeType = ChangeType.Increase,
                ProductsData = _buildingData.ManufacturedProductsData.Value
            });

        private void UpdateProductionUI() => _buildingData.AddEvent(BuildingEvent.InProductionBoxDataUpdated);
        private void UpdateManufacturedUI() => _buildingData.AddEvent(BuildingEvent.ManufacturedBoxDataUpdated);

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

        #endregion
    }
}