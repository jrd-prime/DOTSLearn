using Sources.Scripts.CommonComponents.Product;
using Unity.Collections;

namespace Sources.Scripts.CommonComponents.Production
{
    public struct ProductionLineData
    {
        public unsafe NativeList<ProductData>* Required;
        public unsafe NativeList<ProductData>* Manufactured;
    }
}