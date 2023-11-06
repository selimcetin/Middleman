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
    public class GameController
    {
        static string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        public static List<Middleman> liMiddlemen = new List<Middleman>();
        public static List<Product> liProducts = Utils.parseYamlFile($"{projectDirectory}\\produkte.yml");

        public GameController()
        {

        }

        public static void init()
        {

            while (true)
            {
                try
                {
                    int numMiddleman = UiController.getIntFromReadLinePrompt("Wie viele Zwischenhändler nehmen teil? ");

                    // Save all middlemen
                    //-------------------
                    for (int i = 1; i <= numMiddleman; i++)
                    {
                        string middlemanName = UiController.getStringFromReadLinePrompt($"Name von Zwischenhänder {i}: ");
                        string companyName = UiController.getStringFromReadLinePrompt($"Name der Firma von {middlemanName}: ");
                        int difficulty = UiController.getIntFromReadLinePrompt("Schwierigkeitsgrad auswählen: (1) Einfach, (2) Normal, (3) Schwer");

                        liMiddlemen.Add(new Middleman(middlemanName, companyName, difficulty));
                    }

                    dailyRandomProductionRate();

                    break;
                }
                catch (GameException ex)
                {
                    Console.WriteLine($"Spielfehler: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Anwendungsfehler: {ex.Message}");
                }
            }
        }

        public static void buyProduct(Middleman m, Product p, int quantity)
        {
            int cost = p.BuyingPrice * quantity;
            if (isValidPurchase(m, cost, quantity))
            {
                m.Balance -= cost;
                m.StockCount += quantity;

                // If already in stock, increase quantity
                if (m.Stock.ContainsKey(p))
                {
                    m.Stock[p] += quantity;
                }
                // Otherwise add it to stock
                else
                {
                    m.Stock.Add(p, quantity);
                }
            }
            else
            {
                throw new GameException("Kein valider Einkauf. Nicht genug Geld auf dem Konto oder Lager voll.");
            }
        }

        public static void sellProduct(Middleman m, Product p, int quantity)
        {
            int sellPrice = (int)(p.BasePrice * 0.8 * quantity);

            if (isValidSelling(m.Stock[p], quantity))
            {
                m.Balance += sellPrice;
            }
            else
            {
                throw new GameException("Kein valider Verkauf. Verkaufszahl übersteigt verfügbare Menge.");
            }
        }

        public static void buyProductViaInput(Middleman m, int idx, int quantity)
        {
            if (idx <= liProducts.Count)
            {
                buyProduct(m, liProducts[idx - 1], quantity);
            }
        }

        public static void sellProductViaInput(Middleman m, int idx, int quantity)
        {
            if (idx <= liProducts.Count)
            {
                sellProduct(m, liProducts[idx - 1], quantity);
            }
        }

        public static Product getProductByIdx(int idx)
        {
            if (idx <= liProducts.Count)
            {
                return liProducts[idx - 1];
            }

            return null;
        }

        public static void dailyRandomProductionRate()
        {
            Random rnd = new Random();

            foreach (Product p in liProducts)
            {
                int randomValue = rnd.Next(p.MinProductionRate, p.MaxProductionRate);
                p.AvailableAmount += randomValue;
            }
        }

        public static void dailyRandomPriceAdjustment()
        {
            Random rnd = new Random();

            foreach (Product p in liProducts)
            {
                adjustProductPriceByRandomPercentage(p);
            }
        }

        static void adjustProductPrice(Product p, int priceBasis, int percentage)
        {
            if (percentage > 0)
                increaseProductPrice(p, priceBasis, percentage);
            else
                decreaseProductPrice(p, priceBasis, percentage);
        }
        static void increaseProductPrice(Product p, int priceBasis, int percentage)
        {
            int newPrice = (int) Math.Round(priceBasis * (1 + (percentage / 100.0) ), MidpointRounding.AwayFromZero);

            if (newPrice > 3 * p.BasePrice)
            {
                p.BuyingPrice = 3 * p.BasePrice;
            }

            p.BuyingPrice = newPrice;
        }

        static void decreaseProductPrice(Product p, int priceBasis, int percentage)
        {
            int newPrice = (int)Math.Round(priceBasis * (1 + (percentage / 100.0)), MidpointRounding.AwayFromZero);

            if (newPrice < 0.25 * p.BasePrice)
            {
                p.BuyingPrice = (int) Math.Round(0.25 * p.BasePrice, MidpointRounding.AwayFromZero);
            }

            p.BuyingPrice = newPrice;
        }

        static void adjustProductPriceByRandomPercentage(Product p)
        {
            Random rnd = new Random();
            int maxAvailableAmount = p.MaxProductionRate * p.Durability;
            float percentage = 100 * (p.AvailableAmount / maxAvailableAmount);
            int randomPercentage = 0;

            if (percentage < 25)
            {
                randomPercentage = rnd.Next(-10, 30);
                adjustProductPrice(p, p.BasePrice, randomPercentage);
            }
            else if (percentage > 25 && percentage < 80)
            {
                randomPercentage = rnd.Next(-5, 5);
                adjustProductPrice(p, p.BasePrice, randomPercentage);
            }
            else
            {
                randomPercentage = rnd.Next(-10, 6);
                adjustProductPrice(p, p.BuyingPrice, randomPercentage);
            }
        }

        static bool isValidPurchase(Middleman m, int cost, int quantity)
        {
            if ( cost < m.Balance && (m.StockCount + quantity) <= m.StockCapacity)
            {
                return true;
            }

            return false;
        }

        static bool isValidSelling(int stockCount, int quantity)
        {
            if (quantity < stockCount)
                return false;

            return true;
        }
    }
}
