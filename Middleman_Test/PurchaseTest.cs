using NUnit.Framework.Legacy;
using Middleman_Game;

namespace Middleman_Test
{
    public class PurchaseTest
    {
        private Middleman middleman;
        private Product product;
        private int quantity;

        [SetUp]
        public void Setup()
        {
            middleman = new Middleman("Test", "TestCompany", 2);
            product = new Product();
            product.BuyingPrice = 10;
            product.AvailableAmount = 20;
            quantity = 10;
        }

        [Test]
        public void successfulPurchaseCheckMiddleman()
        {
            // Assign
            //-------
            float cost = product.BuyingPrice * quantity;
            float newStockCount = quantity;
            float newBalance = middleman.Balance - cost;

            // Act
            //----
            MiddlemanController.buyProduct(middleman, product, quantity);

            // Assert
            //-------
            ClassicAssert.AreEqual(newBalance, middleman.Balance);
            ClassicAssert.AreEqual(newStockCount, middleman.StockCount);
            ClassicAssert.IsTrue(middleman.Stock.ContainsKey(product));
        }

        [Test]
        public void successfulPurchaseCheckProductAvailability()
        {
            // Assign
            //-------
            float newProductAvailableAmount = product.AvailableAmount - quantity;

            // Act
            //----
            MiddlemanController.buyProduct(middleman, product, quantity);

            // Assert
            //-------
            ClassicAssert.AreEqual(newProductAvailableAmount, product.AvailableAmount);
        }

        [Test]
        public void successfulPurchaseCheckMiddlemanReport()
        {
            // Assign
            //-------
            float cost = product.BuyingPrice * quantity;
            float newPreviousBuyingCost = cost;

            // Act
            //----
            MiddlemanController.buyProduct(middleman, product, quantity);

            // Assert
            //-------
            ClassicAssert.AreEqual(newPreviousBuyingCost, middleman.BuyingCostPreviousDay);
        }

        [Test]
        public void failedPurchaseDueToInsufficientBalance()
        {
            // Assign
            //-------
            middleman.Balance = 100;
            product.BuyingPrice = 1000000;
            float newStockCount = 0;
            float newBalance = middleman.Balance;

            // Act
            //----
            try
            {
                MiddlemanController.buyProduct(middleman, product, quantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught exception: {ex.Message}");
            }

            // Assert
            //-------
            ClassicAssert.AreEqual(newBalance, middleman.Balance);
            ClassicAssert.AreEqual(newStockCount, middleman.StockCount);
            ClassicAssert.IsFalse(middleman.Stock.ContainsKey(product));
        }

        [Test]
        public void failedPurchaseDueToInsufficientProductAmount()
        {
            // Assign
            //-------
            float newStockCount = 0;
            float newBalance = middleman.Balance;
            product.AvailableAmount = 0;

            // Act
            //----
            try
            {
                MiddlemanController.buyProduct(middleman, product, quantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught exception: {ex.Message}");
            }

            // Assert
            //-------
            ClassicAssert.AreEqual(newBalance, middleman.Balance);
            ClassicAssert.AreEqual(newStockCount, middleman.StockCount);
            ClassicAssert.IsFalse(middleman.Stock.ContainsKey(product));
        }
    }
}