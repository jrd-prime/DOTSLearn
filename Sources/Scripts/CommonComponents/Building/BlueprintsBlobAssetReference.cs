using System;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.CommonComponents.Production;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonComponents.Building
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
                RequiredPtr = GetProductsFromReference(ref required),
                ManufacturedPtr = GetProductsFromReference(ref manufactured)
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