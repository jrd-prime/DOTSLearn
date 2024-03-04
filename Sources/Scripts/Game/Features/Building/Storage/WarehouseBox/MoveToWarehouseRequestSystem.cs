using Sources.Scripts.CommonComponents;
using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.CommonComponents.Storage;
using Sources.Scripts.CommonComponents.Storage.Data;
using Sources.Scripts.CommonComponents.Storage.Service;
using Sources.Scripts.Timer;
using Sources.Scripts.UI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Storage.WarehouseBox
{
    /// <summary>
    /// Moving products from main storage to building warehouse<br/>
    /// <see cref="MoveToWarehouseRequestTag"/>
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial class MoveToWarehouseRequestSystem : SystemBase
    {
        private BuildingDataAspect _aspect;
        private EntityCommandBuffer _ecb;
        private MainStorageBoxData _mainStorageBox;
        private NativeList<ProductData> _matchingProducts;

        protected override void OnCreate()
        {
            RequireForUpdate<MainStorageBoxData>();
            RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<MoveToWarehouseRequestTag>();
        }

        protected override void OnDestroy()
        {
            _matchingProducts.Dispose();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            _ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);

            foreach (var aspect in SystemAPI
                         .Query<BuildingDataAspect>()
                         .WithAll<MoveToWarehouseRequestTag>())
            {
                Debug.LogWarning("___ REQUEST: Move To Warehouse");
                _ecb.RemoveComponent<MoveToWarehouseRequestTag>(aspect.Self);

                _aspect = aspect;
                _mainStorageBox = SystemAPI.GetSingleton<MainStorageBoxData>();

                _matchingProducts = StorageService.GetMatchingProducts(
                    _aspect.RequiredProductsData.Value,
                    _mainStorageBox.Value, out bool isEnough);

                if (!isEnough)
                {
                    TextPopUpMono.Instance.ShowPopUp("not enough products to move!".ToUpper());
                    return;
                }

                SetProductsToDelivery();
                SetDeliveryTimer();
                ReduceProductsInMainStorage();

                aspect.BuildingData.BuildingEvents.Enqueue(BuildingEvent.MoveToWarehouseTimerStarted);
            }
        }

        private void SetProductsToDelivery() =>
            _ecb.AddComponent(_aspect.Self, new ProductsToDeliveryData { Value = _matchingProducts });

        private void SetDeliveryTimer()
        {
            int productsQuantity = StorageService.GetProductsQuantity(_matchingProducts);
            int moveDuration = productsQuantity / 5;

            new global::Sources.Scripts.Timer.JTimer().StartNewTimer(_aspect.Self, TimerType.MoveToWarehouse,
                moveDuration, _ecb);
        }

        private void ReduceProductsInMainStorage() =>
            _aspect.ChangeProductsQuantityData.Value.Enqueue(
                new ChangeProductsQuantityData
                {
                    StorageType = StorageType.Main,
                    ChangeType = ChangeType.Reduce,
                    ProductsData = _matchingProducts
                });
    }
}