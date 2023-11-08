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

        public static List<Middleman> middlemen = new List<Middleman>();
        public static List<Product> products = Utils.parseYamlFile($"{projectDirectory}\\produkte.yml");

        static GameState state = GameState.NewTurn;
        static int currentPlayerIndex = 0;
        static int day = 1;

        public GameController()
        {

        }

        public static void init()
        {
            int numberOfMiddleman = UiController.getIntFromReadLinePrompt("Wie viele Zwischenhändler nehmen teil? ");

            // Save all middlemen
            //-------------------
            for (int i = 1; i <= numberOfMiddleman; i++)
            {
                string middlemanName = UiController.getStringFromReadLinePrompt($"Name von Zwischenhänder {i}: ");
                string companyName = UiController.getStringFromReadLinePrompt($"Name der Firma von {middlemanName}: ");
                int difficulty = UiController.getIntFromReadLinePrompt("Schwierigkeitsgrad auswählen (1) Einfach, (2) Normal, (3) Schwer: ");

                middlemen.Add(new Middleman(middlemanName, companyName, difficulty));
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
                    Middleman currentMiddleman = middlemen[currentPlayerIndex];

                    switch (state)
                    {
                        case GameState.NewTurn:
                            UiController.displayNewTurn(currentMiddleman, day);
                            state = GameState.SelectNextMove;
                            break;
                        case GameState.SelectNextMove:
                            state = getNextStateFromInput(Console.ReadLine());
                            break;
                        case GameState.Buying:
                            state = executeBuyingState(currentMiddleman);
                            break;
                        case GameState.Selling:
                            state = executeSellingState(currentMiddleman);
                            break;
                        case GameState.StockAdjustment:
                            state = executeStockAdjustmentState(currentMiddleman);
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


        static void buyStockUpgrade(Middleman middleman, int quantity)
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

        static void buyProduct(Middleman middleman, Product product, int quantity)
        {
            float cost = product.BuyingPrice * quantity;
            if (isValidPurchase(middleman, product, cost, quantity))
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
                throw new GameException("Kein valider Einkauf. Kauf wurde abgebrochen");
            }
        }

        static void sellProduct(Middleman middleman, Product product, int quantity)
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


        static Product getProductFromList(int index)
        {
            if (index <= products.Count)
            {
                return products[index - 1];
            }

            throw new GameException("Falsche Indexangabe für das Produkt. Bitte Index aus angezeigter Produktliste wählen.");
        }

        static Product getProductFromStock(Middleman middleman, int index)
        {
            if (index <= middleman.Stock.Count())
            {
                return middleman.Stock.ElementAt(index - 1).Key;
            }

            throw new GameException("Falsche Indexangabe für das Produkt. Index muss <= Anzahl unterschiedlicher Produkte im Lager sein.");
        }

        static void dailyRandomProductionRate()
        {
            Random random = new Random();

            foreach (Product product in products)
            {
                int randomValue = random.Next(product.MinProductionRate, product.MaxProductionRate);
                product.AvailableAmount += randomValue;

                // AvailableAmount cannot go below 0
                //----------------------------------
                if (product.AvailableAmount < 0)
                    product.AvailableAmount = 0;
            }
        }

        static void dailyRandomPriceAdjustment()
        {
            Random random = new Random();

            foreach (Product product in products)
            {
                adjustProductPriceByRandomPercentage(product);
            }
        }

        static void adjustProductPrice(Product product, float priceBasis, int percentage)
        {
            if (percentage > 0)
                increaseProductPrice(product, priceBasis, percentage);
            else
                decreaseProductPrice(product, priceBasis, percentage);
        }
        static void increaseProductPrice(Product product, float priceBasis, int percentage)
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

        static void decreaseProductPrice(Product product, float priceBasis, int percentage)
        {
            float newPrice = priceBasis * (1 + (percentage / 100.0f));

            // Check if newPrice is fallen below lower limit
            // BuyingPrice must always be at least 25% of BasePrice
            //-----------------------------------------------------
            if (newPrice < 0.25 * product.BasePrice)
            {
                product.BuyingPrice = (int) Math.Round(0.25 * product.BasePrice, MidpointRounding.AwayFromZero);
            }

            product.BuyingPrice = newPrice;
        }

        static void adjustProductPriceByRandomPercentage(Product product)
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

        static bool hasEnoughBalance(Middleman middleman, float cost)
        {
            if (cost < middleman.Balance)
                return true;

            throw new GameException("Kontostand ist für diese Transaktion zu niedrig.");
        }

        static bool hasEnoughStockSpace(Middleman middleman, int quantity)
        {
            if (middleman.StockCount + quantity <= middleman.StockCapacity)
                return true;

            throw new GameException("Es gibt nicht genügend Platz auf Lager. Niedrige Anzahl auswählen.");
        }

        static bool hasEnoughAmountToBuy(Product product, int quantity)
        {
            if (quantity <= product.AvailableAmount)
                return true;

            throw new GameException("Es gibt nicht genügend Einheiten zum Kaufen. Niedrige Anzahl auswählen.");
        }

        static bool isValidPurchase(Middleman middleman, Product product, float cost, int quantity)
        {
            if (hasEnoughBalance(middleman, cost) &&
                hasEnoughStockSpace(middleman, quantity) &&
                hasEnoughAmountToBuy(product, quantity))
            {
                return true;
            }

            return false;
        }

        static bool isValidSelling(int stockCount, int quantity)
        {
            if (quantity <= stockCount)
                return true;

            return false;
        }

        static GameState getNextStateFromInput(string input)
        {
            switch (input)
            {
                case "b":
                    currentPlayerIndex++;
                    state = GameState.NewTurn;

                    // Day is over, start new day
                    //---------------------------
                    if (currentPlayerIndex == middlemen.Count())
                    {
                        day++;
                        currentPlayerIndex = 0;

                        Utils.leftShiftListOrder(middlemen);
                        dailyRandomProductionRate();
                        dailyRandomPriceAdjustment();
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

        static GameState executeBuyingState(Middleman currentMiddleman)
        {
            UiController.displayBuyingOption(products);

            string input = Console.ReadLine();

            switch (input)
            {
                case "z":
                    state = GameState.NewTurn;
                    break;
                default:
                    int inputIndex = Utils.convertStringToInt(input);
                    Product product = getProductFromList(inputIndex);
                    UiController.displayProductToBuy(product);

                    int quantity = Utils.convertStringToInt(Console.ReadLine());
                    buyProduct(currentMiddleman, product, quantity);

                    state = GameState.NewTurn;
                    break;
            }

            return state;
        }

        static GameState executeSellingState(Middleman currentMiddleman)
        {
            UiController.displaySellingOption(currentMiddleman);

            string input = Console.ReadLine();

            switch (input)
            {
                case "z":
                    state = GameState.NewTurn;
                    break;
                default:
                    int inputIndex = Utils.convertStringToInt(input);
                    Product product = getProductFromStock(currentMiddleman, inputIndex);
                    UiController.displayProductToSell(currentMiddleman, product);

                    int quantity = Utils.convertStringToInt(Console.ReadLine());
                    sellProduct(currentMiddleman, product, quantity);

                    state = GameState.NewTurn;
                    break;
            }

            return state;
        }

        static GameState executeStockAdjustmentState(Middleman currentMiddleman)
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

                    GameController.buyStockUpgrade(currentMiddleman, inputQuantity);

                    state = GameState.NewTurn;
                    break;
            }

            return state;
        }
    }
}
