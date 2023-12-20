using NUnit.Framework.Legacy;
using Middleman_Game;

namespace Middleman_Test
{
    public class SellingTests
    {
        private Middleman middleman;
        private Product product;
        private int sellingQuantity;
        private int stockAmount;
        private int initialStorageSpace;
        private float sellingReducerFactor;

        [SetUp]
        public void Setup()
        {
            middleman = new Middleman("Test", "TestCompany", 2);
            product = new Product();
            product.Name = "Gurke";
            product.BuyingPrice = 10;
            product.BasePrice = 10;
            sellingQuantity = 5;
            stockAmount = 10;
            initialStorageSpace = 100;
            sellingReducerFactor = 0.8f;

            middleman.Stock.Add(product, stockAmount);
            middleman.StockCount = stockAmount;
        }

        [Test]
        public void successfulSellingCheckMiddlemanBalanceAndStock()
        {
            // Assign
            //-------
            float sellingPrice = product.BuyingPrice * sellingQuantity * sellingReducerFactor;
            float newBalance = middleman.Balance + sellingPrice;
            int newStockCount = stockAmount - sellingQuantity;

            // Act
            //----
            MiddlemanController.sellProduct(middleman, product, sellingQuantity);

            // Assert
            //-------
            ClassicAssert.AreEqual(newBalance, middleman.Balance);
            ClassicAssert.AreEqual(newStockCount, middleman.Stock[product]);
            ClassicAssert.AreEqual(newStockCount, middleman.StockCount);
        }

        [Test]
        public void successfulSellingCheckMiddlemanPreviousSales()
        {
            // Assign
            //-------
            float sellingPrice = product.BuyingPrice * sellingQuantity * sellingReducerFactor;

            // Act
            //----
            MiddlemanController.sellProduct(middleman, product, sellingQuantity);

            // Assert
            //-------
            ClassicAssert.AreEqual(sellingPrice, middleman.SalesPreviousDay);
        }

        [Test]
        public void failedSellingDueToNotInStock()
        {
            // Assign
            //-------
            Product differentProduct = new Product();
            differentProduct.Name = "Tomate";
            differentProduct.BuyingPrice = 10;
            differentProduct.BasePrice = 10;
            float newBalance = middleman.Balance;
            int newStockCount = stockAmount;

            // Act
            //----
            try
            {
                MiddlemanController.sellProduct(middleman, differentProduct, sellingQuantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught exception: {ex.Message}");
            }

            // Assert
            //-------
            ClassicAssert.AreEqual(newBalance, middleman.Balance);
            ClassicAssert.AreEqual(newStockCount, middleman.Stock[product]);
            ClassicAssert.AreEqual(newStockCount, middleman.StockCount);
        }
    }
}