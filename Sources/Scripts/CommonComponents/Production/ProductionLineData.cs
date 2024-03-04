using Sources.Scripts.CommonComponents.Product;
using Unity.Collections;

namespace Sources.Scripts.CommonComponents.Production
{
    public struct ProductionLineData
    {
        public NativeList<ProductData> RequiredPtr;
        public NativeList<ProductData> ManufacturedPtr;

        public NativeList<ProductData> Required;
        public NativeList<ProductData> Manufactured;
    }
}