using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_1
{
    public static class MarketController
    {
        public static void handleDailyProductionRateAdjustment(List<Product> productList)
        {
            Random random = new Random();

            foreach (Product product in productList)
            {
                int randomValue = random.Next(product.MinProductionRate, product.MaxProductionRate);
                product.AvailableAmount += randomValue;

                // AvailableAmount cannot go below 0
                //----------------------------------
                if (product.AvailableAmount < 0)
                    product.AvailableAmount = 0;
            }
        }

        public static void handleDailyPriceAdjustment(List<Product> productList)
        {
            Random random = new Random();

            foreach (Product product in productList)
            {
                adjustProductPriceByRandomPercentage(product);
            }
        }
        public static void adjustProductPrice(Product product, float priceBasis, int percentage)
        {
            if (percentage > 0)
                increaseProductPrice(product, priceBasis, percentage);
            else
                decreaseProductPrice(product, priceBasis, percentage);
        }

        public static void increaseProductPrice(Product product, float priceBasis, int percentage)
        {
            float newPrice = priceBasis * (1 + (percentage / 100.0f));

            // Check if newPrice is exceeding upper limit
            // BuyingPrice mustn't be more than 3 times the BasePrice
            //-------------------------------------------------------
            if (newPrice > 3 * product.BasePrice)
            {
                product.BuyingPrice = 3 * product.BasePrice;
            }

            product.BuyingPrice = newPrice;
        }

        public static void decreaseProductPrice(Product product, float priceBasis, int percentage)
        {
            float newPrice = priceBasis * (1 + (percentage / 100.0f));

            // Check if newPrice is fallen below lower limit
            // BuyingPrice must always be at least 25% of BasePrice
            //-----------------------------------------------------
            if (newPrice < 0.25 * product.BasePrice)
            {
                product.BuyingPrice = (int)Math.Round(0.25 * product.BasePrice, MidpointRounding.AwayFromZero);
            }

            product.BuyingPrice = newPrice;
        }

        public static void adjustProductPriceByRandomPercentage(Product product)
        {
            Random random = new Random();
            int maxAvailableAmount = product.MaxProductionRate * product.Durability;
            float percentage = 100 * (product.AvailableAmount / maxAvailableAmount);
            int randomPercentage;

            if (percentage < 25)
            {
                randomPercentage = random.Next(-10, 30);
                adjustProductPrice(product, product.BasePrice, randomPercentage);
            }
            else if (percentage > 25 && percentage < 80)
            {
                randomPercentage = random.Next(-5, 5);
                adjustProductPrice(product, product.BasePrice, randomPercentage);
            }
            else
            {
                randomPercentage = random.Next(-10, 6);
                adjustProductPrice(product, product.BasePrice, randomPercentage);
            }
        }
    }
}
