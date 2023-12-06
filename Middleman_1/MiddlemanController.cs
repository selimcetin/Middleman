using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_1
{
    public static class MiddlemanController
    {
        public static void buyProduct(Middleman middleman, Product product, int quantity)
        {
            float cost = product.BuyingPrice * quantity;

            string errorMessage;
            if (isValidPurchase(middleman, product, cost, quantity, out errorMessage))
            {
                middleman.Balance -= cost;
                middleman.BuyingCostPreviousDay += cost;
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
                middleman.SalesPreviousDay += sellPrice;
                middleman.Stock[product] -= quantity;
                middleman.StockCount -= quantity;
            }
            else
            {
                throw new GameException("Kein valider Verkauf. Verkaufszahl übersteigt verfügbare Menge.");
            }
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
                middleman.StorageCostPreviousDay += cost;
            }
            else
            {
                throw new GameException("Nicht genug Geld auf dem Konto für ein Lagerupgrade.");
            }
        }

        public static void payDailyStorageCost(GameInfo gameInfo, Middleman middleman, List<Middleman> middlemanList)
        {
            // Cost for storage is 1$ per free space and 5$ per allocated space
            // Example storage situation: 10/110 --> cost = 10*50 + 100;
            //----------------------------------------------------------
            float cost = middleman.getStockCount() * 5 + (middleman.StockCapacity - middleman.getStockCount());

            if (hasEnoughBalance(middleman, cost))
            {
                middleman.Balance -= cost;
                middleman.StorageCostPreviousDay += cost;
            }
            else
            {
                GameController.removeMiddlemanFromList(gameInfo, middleman, middlemanList);
                UiController.displayLosingMiddleman(middleman);
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

        public static void resetPreviousDayVariables(Middleman middleman)
        {
            middleman.BuyingCostPreviousDay = 0;
            middleman.SalesPreviousDay = 0;
            middleman.StorageCostPreviousDay = 0;
            middleman.BalancePreviousDay = middleman.Balance;
        }
    }
}
