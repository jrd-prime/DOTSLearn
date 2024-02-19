using Jrd.Gameplay.Products;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage.InProductionBox.Component;
using Jrd.Gameplay.Storage.MainStorage.Component;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities.Tests;

namespace Tests.EditMode
{
    [TestFixture]
    [Category("StorageTests")]
    public class StorageTests : ECSTestsFixture
    {
        private NativeList<ProductData> _oneProductProductDataList =
            new(0, Allocator.Persistent)
            {
                new ProductData { Name = (Product)1, Quantity = 10 }
            };

        private readonly NativeParallelHashMap<int, int> _emptyStorageMap =
            new(0, Allocator.Persistent);

        [Test]
        public void ChangeQuantity_MainStorage_AddProductToStorage_IfStorageEmpty()
        {
            // arrange
            MainStorageData mainStorageData = new() { Value = _emptyStorageMap };

            // act
            mainStorageData.ChangeProductsQuantity(ChangeType.Increase, _oneProductProductDataList);

            // assert
            var expected = _oneProductProductDataList[0].Quantity;
            var actual = mainStorageData.Value[(int)_oneProductProductDataList[0].Name];

            Assert.AreEqual(expected, actual);
        }
    }
}