using System;
using CommonComponents.Product;
using CommonComponents.Production;
using Unity.Collections;
using Unity.Entities;

namespace CommonComponents.Building
{
    public struct BlueprintsBlobData : IComponentData
    {
        public BlobAssetReference<BlobArray<BlobArray<BlobArray<ProductData>>>> Reference;

        public ProductionLineData GetProductionLineProducts(BuildingNameId buildingNameId)
        {
            ref BlobArray<ProductData> required = ref Reference.Value[(int)buildingNameId][0];
            ref BlobArray<ProductData> manufactured = ref Reference.Value[(int)buildingNameId][1];
            
            if (buildingNameId != BuildingNameId.Default)
            {
                return new ProductionLineData
                {
                    Required = GetProductsForBuildingFromReference(ref required),
                    Manufactured = GetProductsForBuildingFromReference(ref manufactured)
                };
            }

            throw new Exception($"Wrong building name id {nameof(BuildingNameId.Default)}!");
        }

        private static NativeList<ProductData> GetProductsForBuildingFromReference(ref BlobArray<ProductData> reference)
        {
            var data = new NativeList<ProductData>(0, Allocator.Persistent);

            if (reference.Length <= 0) return data;
            
            for (var i = 0; i < reference.Length; i++)
            {
                data.Add(reference[i]);
            }

            return data;
        }
    }
}