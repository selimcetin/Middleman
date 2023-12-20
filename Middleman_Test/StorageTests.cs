using NUnit.Framework.Legacy;
using Middleman_Game;

namespace Middleman_Test
{
    public class StorageTests
    {
        private Middleman middleman;
        private int initialStorageSize;
        private int storageUpgradeCost;

        [SetUp]
        public void Setup()
        {
            middleman = new Middleman("Test", "TestCompany", 2);
            middleman.Balance = 10000;
            initialStorageSize = 100;
            storageUpgradeCost = 50;
        }

        [Test]
        public void succesfulStorageUpgrade()
        {
            // Assign
            //-------
            float currentBalance = middleman.Balance;
            int storageUpgradeAmount = 10;
            int newStorageSize = initialStorageSize + storageUpgradeAmount;
            float newBalance = currentBalance - (storageUpgradeAmount * storageUpgradeCost);

            // Act
            //----
            MiddlemanController.buyStockUpgrade(middleman, storageUpgradeAmount);

            // Assert
            //-------
            ClassicAssert.AreEqual(newBalance, middleman.Balance);
            ClassicAssert.AreEqual(newStorageSize, middleman.StockCapacity);
        }

        [Test]
        public void failedStorageUpgrade()
        {
            // Assign
            //-------
            float currentBalance = middleman.Balance;
            int storageUpgradeAmount = 10000;

            // Act
            //----
            try
            {
                MiddlemanController.buyStockUpgrade(middleman, storageUpgradeAmount);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Caught exception: {ex.Message}");
            }

            // Assert
            //-------
            ClassicAssert.AreEqual(currentBalance, middleman.Balance);
            ClassicAssert.AreEqual(initialStorageSize, middleman.StockCapacity);
        }
    }
}