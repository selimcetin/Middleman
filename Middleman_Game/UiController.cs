using Middleman_Game;
using System;
using System.Security.Cryptography.X509Certificates;

public class UiController
{
    public UiController()
    {
    }

    public static int getIntFromReadLinePrompt(string text)
    {
        Console.Write(text);
        int value;
        while (true)
        {
            try
            {
                if (!int.TryParse(Console.ReadLine(), out value))
                {
                    throw new GameException("Falsche Eingabe, bitte eine Zahl eingeben.");
                }
                else
                {
                    printSeparator();
                    return value;
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

    public static string getStringFromReadLinePrompt(string text)
    {
        Console.Write(text);
        string readLineText = Console.ReadLine();
        printSeparator();
        return readLineText;
    }

    static void printSeparator()
    {
        Console.WriteLine("-----------------------------------------------");
    }

    public static void displayReport(Middleman middleman)
    {
        Console.WriteLine($"Kontostand des letzten Tages: {middleman.BalancePreviousDay:F2}");
        Console.WriteLine($"Ausgaben für Einkaufe des letzten Tages: {middleman.BuyingCostPreviousDay:F2}");
        Console.WriteLine($"Einnahmen für Verkäufe des letzten Tages: {middleman.SalesPreviousDay:F2}");
        Console.WriteLine($"Lagerkosten des letzten Tages: {middleman.StorageCostPreviousDay:F2}");
        Console.WriteLine($"Aktueller Kontostand: {middleman.Balance:F2}");
        printSeparator();

        while (Console.ReadKey().Key != ConsoleKey.Enter) ;
    }

    public static void displayMenu(Middleman middleman, int day)
    {
        Console.WriteLine(
            $"{middleman.Name} von {middleman.CompanyName} | ${middleman.Balance:F2} | Lager: {middleman.StockCount}/{middleman.StockCapacity} | Tag {day}");
        Console.WriteLine("e) Einkaufen");
        Console.WriteLine("v) Verkaufen");
        Console.WriteLine("l) Lagerkapazität erhöhen");
        Console.WriteLine("k) Kredit aufnehmen");
        Console.WriteLine("b) Runde beenden");
        printSeparator();
    }

    public static void displayBuyingOption(List<Product> products)
    {
        Console.WriteLine($"Verfügbare Produkte:");
        displayProducts(products);
        Console.WriteLine("z) Zurück");
        printSeparator();
    }

    public static void displaySellingOption(Middleman middleman)
    {
        Console.WriteLine("Produkte im Besitz:");
        displayStock(middleman);
        Console.WriteLine("z) Zurück");
        printSeparator();
    }

    public static void displayStockOptions()
    {
        Console.WriteLine("Erhöhung der Lagerkapazität kostet 50$ pro Einheit.");
        Console.WriteLine("Um wie viel Einheiten soll Lager vergrößert werden? 1 Einheit = 50 Lager Kapazität.");
        printSeparator();
    }

    static void displayStock(Middleman middleman)
    {
        for (int i = 0; i < middleman.Stock.Count; i++)
        {
            Product product = middleman.Stock.ElementAt(i).Key;
            float sellingPrice = product.BuyingPrice * 0.8f;

            Console.WriteLine(
                $"{i + 1}) {product.Name} ({middleman.Stock.ElementAt(i).Value}) ${sellingPrice:F2}/Stück");
        }
        printSeparator();
    }

    static void displayProducts(List<Product> products)
    {
        for (int i = 0; i < products.Count; i++)
        {
            Console.WriteLine($"{i+1}) {products[i].ToString()}");
        }
        printSeparator();
    }

    public static void displayProductToBuy(Product product)
    {
        if (null != product)
        {
            Console.WriteLine($"Wie viel von {product.Name} kaufen?");
        }
        printSeparator();
    }

    public static void displayProductToSell(Middleman middleman, Product product)
    {
        if (middleman.Stock.ContainsKey(product))
        {
            Console.WriteLine($"Wieviel von {product.Name} verkaufen (max. {middleman.Stock[product]})? ");
        }
        printSeparator();
    }

    public static void displayLosingMiddleman(Middleman middleman)
    {
        Console.WriteLine($"{middleman.ToString()} has lost the game.");
        printSeparator();
    }

    public static void displayScoreboard(List<Middleman> middlemanList)
    {
        middlemanList.Sort((middleman1, middleman2) => middleman2.Balance.CompareTo(middleman1.Balance));

        for (int i = 0; i < middlemanList.Count; i++)
        {
            Console.WriteLine($"Platz {i + 1}: {middlemanList[i].ToString()}");
        }
        printSeparator();
    }

    public static void displayCreditOptions(List<Credit> creditList)
    {
        for (int i = 0; i < creditList.Count; i++)
        {
            Credit credit = creditList[i];
            Console.WriteLine($"{i + 1}) {credit.Sum} mit {credit.Percentage}. Rückzahlung nach 7 Tagen: {credit.Repayment}");
        }
        printSeparator();
    }
}