using System;

namespace Jrd.Gameplay.Products
{
    [Serializable]
    public struct ProductData
    {
        public Product Name;
        public int Quantity;

        /// <summary>
        /// +- based on weight
        /// </summary>
        public float MoveTimeMultiplier;
    }
}