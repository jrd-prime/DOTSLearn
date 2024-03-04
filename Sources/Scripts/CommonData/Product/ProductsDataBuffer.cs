using Unity.Entities;

namespace Sources.Scripts.CommonData.Product
{
    public struct ProductsDataBuffer : IBufferElementData
    {
        public Product Product;
        public int PackSize;
        public float MoveTimeMultiplier;
    }
}