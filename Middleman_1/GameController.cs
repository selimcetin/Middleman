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
        enum GameState
        {
            NewTurn,
            SelectNextMove,
            Buying,
            Selling,
            StockAdjustment,
            GameEnd
        }

        static string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        public static List<Middleman> liMiddlemen = new List<Middleman>();
        public static List<Product> liProducts = Utils.parseYamlFile($"{projectDirectory}\\produkte.yml");

        static GameState state = GameState.NewTurn;
        static int currPlayerIdx = 0;
        static int day = 1;

        public GameController()
        {

        }

        public static void init()
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
        }

        public static void startStateMachine()
        {
            while (true)
            {
                try
                {
                    string input = "";
                    Middleman currMiddleman = GameController.liMiddlemen[currPlayerIdx];

                    switch (state)
                    {
                        case GameState.NewTurn:
                            UiController.displayNewTurn(currMiddleman, day);

                            state = GameState.SelectNextMove;
                            break;
                        case GameState.SelectNextMove:
                            state = getNextStateFromInput(Console.ReadLine());
                            
                            break;
                        case GameState.Buying:
                            state = executeBuyingState(currMiddleman);
                            break;
                        case GameState.Selling:
                            state = executeSellingState(currMiddleman);
                            break;
                        case GameState.StockAdjustment:
                            state = executeStockAdjustmentState(currMiddleman);
                            break;
                        default:
                            break;
                    }
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


        public static void buyStockUpgrade(Middleman m, int quantity)
        {
            int upgradeCost = 50;

            if (quantity < 0)
                return;

            if (isValidPurchase(m, upgradeCost, quantity))
            {
                int cost = quantity * upgradeCost;
                m.StockCapacity += quantity;
                m.Balance -= cost;
            }
            else
            {
                throw new GameException("Nicht genug Geld auf dem Konto für ein Lagerupgrade.");
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

        static GameState getNextStateFromInput(string input)
        {
            switch (input)
            {
                case "b":
                    currPlayerIdx++;
                    state = GameState.NewTurn;

                    // Day is over, start new day
                    //---------------------------
                    if (currPlayerIdx == GameController.liMiddlemen.Count())
                    {
                        day++;
                        currPlayerIdx = 0;

                        Utils.switchListOrder(GameController.liMiddlemen);
                        GameController.dailyRandomProductionRate();
                        GameController.dailyRandomPriceAdjustment();
                    }
                    break;
                case "e":
                    state = GameState.Buying;
                    break;
                case "l":
                    state = GameState.StockAdjustment;
                    break;
                case "v":
                    state = GameState.Selling;
                    break;
                default:
                    state = GameState.SelectNextMove;
                    break;
            }

            return state;
        }

        static GameState executeBuyingState(Middleman currMiddleman)
        {
            UiController.displayBuyingOption(liProducts);

            string input = Console.ReadLine();

            switch (input)
            {
                case "z":
                    state = GameState.NewTurn;
                    break;
                default:
                    int inputIdx = Utils.convertStringToInt(input);
                    Product p = getProductByIdx(inputIdx);
                    UiController.displayProductToBuy(p);

                    int quantity = Utils.convertStringToInt(Console.ReadLine());
                    buyProductViaInput(currMiddleman, inputIdx, quantity);

                    state = GameState.NewTurn;
                    break;
            }

            return state;
        }

        static GameState executeSellingState(Middleman currMiddleman)
        {
            UiController.displaySellingOption(currMiddleman);

            string input = Console.ReadLine();

            switch (input)
            {
                case "z":
                    state = GameState.NewTurn;
                    break;
                default:
                    int inputIdx = Utils.convertStringToInt(input);
                    Product p = getProductByIdx(inputIdx);
                    UiController.displayProductToSell(currMiddleman, p);

                    int quantity = Utils.convertStringToInt(Console.ReadLine());
                    sellProductViaInput(currMiddleman, inputIdx, quantity);

                    state = GameState.NewTurn;
                    break;
            }

            return state;
        }

        static GameState executeStockAdjustmentState(Middleman currMiddleman)
        {
            UiController.displayStockOptions();

            string input = Console.ReadLine();

            switch (input)
            {
                case "z":
                    state = GameState.NewTurn;
                    break;
                default:
                    int inputQuantity = Utils.convertStringToInt(input);

                    GameController.buyStockUpgrade(currMiddleman, inputQuantity);

                    state = GameState.NewTurn;
                    break;
            }

            return state;
        }
    }

    
}
