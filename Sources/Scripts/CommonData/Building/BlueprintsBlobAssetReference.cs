using System;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.Production;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.CommonData.Building
{
    public struct BlueprintsBlobAssetReference : IComponentData
    {
        public BlobAssetReference<BlobArray<BlobArray<BlobArray<ProductData>>>> Reference;

        public const int RequiredCellId = 0;
        public const int ManufacturedCellId = 1;

        public ProductionLineData GetProductionLineProducts(BuildingNameId buildingName)
        {
            ref BlobArray<ProductData> required = ref Reference.Value[(int)buildingName][RequiredCellId];
            ref BlobArray<ProductData> manufactured = ref Reference.Value[(int)buildingName][ManufacturedCellId];
            
            if (buildingName == BuildingNameId.Default)
                throw new Exception($"Wrong building name id {nameof(BuildingNameId.Default)}!");

            return new ProductionLineData
            {
                Required = GetProductsFromReference(ref required),
                Manufactured = GetProductsFromReference(ref manufactured)
            };
        }

        private static NativeList<ProductData> GetProductsFromReference(ref BlobArray<ProductData> reference)
        {
            NativeList<ProductData> data = new(0, Allocator.Persistent);

            if (reference.Length == 0) return data;

            for (var i = 0; i < reference.Length; i++)
            {
                data.Add(reference[i]);
            }

            return data;
        }
    }
}