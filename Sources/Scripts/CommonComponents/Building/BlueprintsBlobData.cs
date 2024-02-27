using System;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.CommonComponents.Production;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonComponents.Building
{
    public struct BlueprintsBlobData : IComponentData
    {
        public BlobAssetReference<BlobArray<BlobArray<BlobArray<ProductData>>>> Reference;

        public unsafe ProductionLineData GetProductionLineProducts(BuildingNameId buildingNameId)
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

        private static unsafe NativeList<ProductData>* GetProductsForBuildingFromReference(
            ref BlobArray<ProductData> reference)
        {
            NativeList<ProductData> data = new NativeList<ProductData>(0, Allocator.Persistent);

            NativeList<ProductData>* pointerToData = &data;

            if (reference.Length <= 0) return pointerToData;

            for (var i = 0; i < reference.Length; i++)
            {
                pointerToData->Add(reference[i]);
            }

            return pointerToData;
        }
    }
}