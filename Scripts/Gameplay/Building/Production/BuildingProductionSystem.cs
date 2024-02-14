using System;
using Jrd.Gameplay.Building.Production.Component;
using Jrd.Gameplay.Storage.Service;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Building.Production
{
    public partial struct BuildingProductionSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private Entity _entity;

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
                _entity = aspect.Self;

                ProductionState productionState = aspect.BuildingData.ProductionState;

                switch (productionState)
                {
                    case ProductionState.Init:
                        Debug.Log("Init " + aspect.BuildingData.Name + " " + aspect.BuildingData.Guid);
                        return;
                    case ProductionState.NotEnoughProducts:
                        Debug.Log("NotEnoughProducts " + aspect.BuildingData.Name);

                        // показывать в панели билдинга что нету продуктов для производства в инпродакшн боксе

                        break;
                    case ProductionState.EnoughProducts:
                        Debug.Log("EnoughProducts " + aspect.BuildingData.Name);

                        if (!ManufacturedService.IsEnoughSpaceInManufacturedBox() )
                        {
                        
                            Debug.LogWarning("Not enough space in manufactured box!");
                        }

                        // 0 проверить есть ли доступное место в боксе произведенных для старта нового цикла
                        // если нет уведомить/показать что надо освободить место в боксе произведенных

                        // 1 рассчитать продукты
                        // 2 понять за сколько раз переработаем
                        // 3 какие тайминги


                        // запустить производство
                        StartProduction();
                        aspect.SetProductionState(ProductionState.Started);
                        break;
                    case ProductionState.Started:
                        Debug.Log("Started " + aspect.BuildingData.Name);
                        // запустить таймеры
                        // взять часть продуктов для 1 прогона
                        // обновить юай
                        // таймер юай на 0

                        // стэйт что в процессе

                        break;
                    case ProductionState.InProgress:
                        Debug.Log("InProgress " + aspect.BuildingData.Name);

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

        private bool IsEnoughSpaceInManufacturedBox()
        {
        }

        private void StartProduction()
        {
            // 1 рассчитать продукты
            // 2 понять за сколько раз переработаем
            // 3 какие тайминги


            // запустить производство


            _ecb.AddComponent<StartProductionTag>(_entity);
        }
    }
}