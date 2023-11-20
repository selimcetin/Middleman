using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Middleman_1
{
    public static class GameController
    {
        public static void init(GameInfo gameInfo)
        {
            int numberOfMiddleman = UiController.getIntFromReadLinePrompt("Wie viele Zwischenhändler nehmen teil? ");

            // Save all Middlemen
            //-------------------
            for (int i = 1; i <= numberOfMiddleman; i++)
            {
                string middlemanName = UiController.getStringFromReadLinePrompt($"Name von Zwischenhänder {i}: ");
                string companyName = UiController.getStringFromReadLinePrompt($"Name der Firma von {middlemanName}: ");
                int difficulty =
                    UiController.getIntFromReadLinePrompt(
                        "Schwierigkeitsgrad auswählen (1) Einfach, (2) Normal, (3) Schwer: ");

                gameInfo.MiddlemanList.Add(new Middleman(middlemanName, companyName, difficulty));
            }

            gameInfo.CurrentMiddleman = gameInfo.MiddlemanList[0];

            handleDailyProductionRateAdjustment(gameInfo.ProductList);
        }

        public static void buyStockUpgrade(Middleman middleman, int quantity)
        {
            int upgradeCost = 50;

            if (quantity < 0)
                return;

            if (hasEnoughBalance(middleman, upgradeCost * quantity))
            {
                int cost = quantity * upgradeCost;
                middleman.StockCapacity += quantity;
                middleman.Balance -= cost;
            }
            else
            {
                throw new GameException("Nicht genug Geld auf dem Konto für ein Lagerupgrade.");
            }
        }

        public static void buyProduct(Middleman middleman, Product product, int quantity)
        {
            float cost = product.BuyingPrice * quantity;

            string errorMessage;
            if (isValidPurchase(middleman, product, cost, quantity, out errorMessage))
            {
                middleman.Balance -= cost;
                middleman.StockCount += quantity;
                product.AvailableAmount -= quantity;

                // If already in stock, increase quantity
                if (middleman.Stock.ContainsKey(product))
                {
                    middleman.Stock[product] += quantity;
                }
                // Otherwise add it to stock
                else
                {
                    middleman.Stock.Add(product, quantity);
                }
            }
            else
            {
                throw new GameException(errorMessage);
            }
        }

        public static void sellProduct(Middleman middleman, Product product, int quantity)
        {
            float sellPrice = product.BasePrice * 0.8f * quantity;

            if (isValidSelling(middleman.Stock[product], quantity))
            {
                middleman.Balance += sellPrice;
                middleman.Stock[product] -= quantity;
                middleman.StockCount -= quantity;
            }
            else
            {
                throw new GameException("Kein valider Verkauf. Verkaufszahl übersteigt verfügbare Menge.");
            }
        }

        public static Product getProductFromList(List<Product> productList, int index)
        {
            if (index <= productList.Count)
            {
                return productList[index - 1];
            }

            throw new GameException(
                "Falsche Indexangabe für das Produkt. Bitte Index aus angezeigter Produktliste wählen.");
        }

        public static Product getProductFromStock(Middleman middleman, int index)
        {
            if (index <= middleman.Stock.Count())
            {
                return middleman.Stock.ElementAt(index - 1).Key;
            }

            throw new GameException(
                "Falsche Indexangabe für das Produkt. Index muss <= Anzahl unterschiedlicher Produkte im Lager sein.");
        }

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

        public static bool hasEnoughBalance(Middleman middleman, float cost)
        {
            if (cost < middleman.Balance)
                return true;

            return false;
        }

        public static bool hasEnoughStockSpace(Middleman middleman, int quantity)
        {
            if (middleman.StockCount + quantity <= middleman.StockCapacity)
                return true;

            return false;
        }

        public static bool hasEnoughAmountToBuy(Product product, int quantity)
        {
            if (quantity <= product.AvailableAmount)
                return true;

            return false;
        }

        public static bool isValidPurchase(Middleman middleman, Product product, float cost, int quantity,
            out string errorMessage)
        {
            errorMessage = null;

            if (!hasEnoughBalance(middleman, cost))
            {
                errorMessage = "Der Kontostand für die Transaktion ist zu niedrig.";
                return false;
            }

            if (!hasEnoughStockSpace(middleman, quantity))
            {
                errorMessage = "Es gibt nicht genügend Platz auf Lager. Niedrigere Anzahl auswählen.";
                return false;
            }

            if (!hasEnoughAmountToBuy(product, quantity))
            {
                errorMessage = "Es gibt nicht genügend Einheiten zum Kaufen. Niedrige Anzahl auswählen.";
                return false;
            }

            return true;
        }

        public static bool isValidSelling(int stockCount, int quantity)
        {
            if (quantity <= stockCount)
                return true;

            return false;
        }

        public static void payDailyStorageCost(Middleman middleman, List<Middleman> middlemanList)
        {
            // Cost for storage is 1$ per free space and 5$ per allocated space
            // Example storage situation: 10/110 --> cost = 10*50 + 100;
            //----------------------------------------------------------
            float cost = middleman.getStockCount() * 5 + (middleman.StockCapacity - middleman.getStockCount());

            if (hasEnoughBalance(middleman, cost))
            {
                middleman.Balance -= cost;
            }
            else
            {
                removeMiddlemanFromList(middleman, middlemanList);
                UiController.displayLosingMiddleman(middleman);
            }
        }

        public static void removeMiddlemanFromList(Middleman middleman, List<Middleman> middlemanList)
        {
            middlemanList.Remove(middleman);
        }

        public static bool isNextDay(List<Middleman> middlemanList, int currentPlayerIndex)
        {
            if (currentPlayerIndex == middlemanList.Count())
            {
                return true;
            }

            return false;
        }

        public static void updateGameInfoForNextDay(GameInfo gameInfo)
        {
            gameInfo.Day++;
            gameInfo.CurrentPlayerIndex = 0;
        }
    }
}