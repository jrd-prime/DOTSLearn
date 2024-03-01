using Sources.Scripts.CommonComponents.Product;
using Unity.Collections;

namespace Sources.Scripts.CommonComponents.Production
{
    public unsafe struct ProductionLineData
    {
        public NativeList<ProductData> RequiredPtr;
        public NativeList<ProductData> ManufacturedPtr;
    }
}