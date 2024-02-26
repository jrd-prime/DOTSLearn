using Unity.Entities;

namespace CommonComponents.Product
{
    public struct ProductsDataBuffer : IBufferElementData
    {
        public Product Product;
        public int PackSize;
        public float MoveTimeMultiplier;
    }
}