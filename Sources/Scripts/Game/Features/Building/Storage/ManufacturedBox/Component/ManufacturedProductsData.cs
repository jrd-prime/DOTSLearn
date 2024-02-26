﻿using Sources.Scripts.CommonComponents.Product;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Storage.ManufacturedBox.Component
{
    /// <summary>
    /// Required and manufactured
    /// </summary>
    public struct ManufacturedProductsData : IComponentData
    {
        public NativeList<ProductData> Manufactured;
    }
}