using System.Collections.Generic;
using CommonComponents;
using CommonComponents.Product;
using CommonComponents.ScriptableObjects;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ProductsDataAuthoring : MonoBehaviour
    {
        [SerializeField] public List<ProductDataSO> _products;

        private class Baker : Baker<ProductsDataAuthoring>
        {
            public override void Bake(ProductsDataAuthoring authoring)
            {
                Entity entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);

                DynamicBuffer<ProductsDataBuffer> buffer = AddBuffer<ProductsDataBuffer>(entity);

                foreach (var product in authoring._products)
                {
                    buffer.Add(new ProductsDataBuffer
                    {
                        Product = product.Product,
                        PackSize = product.Size,
                        MoveTimeMultiplier = product.MoveTimeMultiplier
                    });
                }
            }
        }
    }
}