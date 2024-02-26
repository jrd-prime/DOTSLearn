using CommonComponents.Product;
using Unity.Collections;

namespace CommonComponents.Production
{
    public struct ProductionLineData
    {
        public NativeList<ProductData> Required;
        public NativeList<ProductData> Manufactured;
    }
}