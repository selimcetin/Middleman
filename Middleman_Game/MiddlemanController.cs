using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_Game
{
    public static class MiddlemanController
    {
        public static void buyProduct(Middleman middleman, Product product, int quantity)
        {
            float cost = getBuyingPriceAfterDiscount(middleman, product) * quantity;

            string errorMessage;
            if (isValidPurchase(middleman, product, cost, quantity, out errorMessage))
            {
                middleman.Balance -= cost;
                middleman.BuyingCostPreviousDay += cost;
                middleman.StockCount += quantity;
                product.AvailableAmount -= quantity;

                putProductToStock(middleman, product, quantity);
            }
            else
            {
                throw new GameException(errorMessage);
            }
        }

        private static void putProductToStock(Middleman middleman, Product product, int quantity)
        {
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

        private static float getDiscountValueInDecimal(Middleman middleman, Product product)
        {
            if (middleman.Stock.ContainsKey(product))
            {
                int quantity = middleman.Stock[product];

                // 0% Discount
                if (quantity > 0 && quantity <= 24)
                {
                    return 0;
                }

                // 2% Discount
                if (quantity > 24 && quantity <= 50)
                {
                    return 0.02f;
                }

                // 5% Discount
                if (quantity > 50 && quantity <= 74)
                {
                    return 0.05f;
                }

                // 10% Discount
                return 0.1f;
            }

            return 0;
        }

        private static float getBuyingPriceAfterDiscount(Middleman middleman, Product product)
        {
            float discountDecimal = 1 - getDiscountValueInDecimal(middleman, product);
            return product.BuyingPrice * discountDecimal;
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
            float cost = middleman.StockCount * 5 + (middleman.StockCapacity - middleman.StockCount);

            if (hasEnoughBalance(middleman, cost))
            {
                middleman.Balance -= cost;
                middleman.StorageCostPreviousDay += cost;
            }
            else
            {
                GameController.processBankruptMiddleman(gameInfo, middleman, middlemanList);
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

        public static void setMiddlemanCredit(Middleman middleman, Credit credit)
        {
            middleman.Credit = credit;
            middleman.Balance += credit.Sum;
        }

        static void payCredit(GameInfo gameInfo, Middleman middleman, List<Middleman> middlemanList)
        {
            float cost = middleman.Credit.Repayment;

            if (hasEnoughBalance(middleman, cost))
            {
                middleman.Balance -= cost;
            }
            else
            {
                GameController.processBankruptMiddleman(gameInfo, middleman, middlemanList);
            }
        }

        public static void updateCreditDueDay(GameInfo gameInfo, Middleman middleman, List<Middleman> middlemanList)
        {
            middleman.Credit.DayDue--;

            if (0 == middleman.Credit.DayDue)
            {
                payCredit(gameInfo, middleman, middlemanList);
            }
        }
    }
}