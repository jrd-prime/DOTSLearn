using Unity.Entities;

namespace Sources.Scripts.CommonComponents.Product
{
    public struct ProductsDataBuffer : IBufferElementData
    {
        public Product Product;
        public int PackSize;
        public float MoveTimeMultiplier;
    }
}