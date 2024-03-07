using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonData.Product
{
    public struct ProductsDataBuffer : IBufferElementData
    {
        public Product Product;
        public FixedString64Bytes Name;
        public int PackSize;
        public float MoveTimeMultiplier;
    }
}