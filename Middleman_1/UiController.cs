using Middleman_1;
using System;

public class UiController
{
	public UiController()
	{
		
	}

    public static int getIntFromReadLinePrompt(string text)
    {
        Console.Write(text);
        int value;
        while(true)
        {
            try
            {
                if (!int.TryParse(Console.ReadLine(), out value))
                {
                    throw new GameException("Falsche Eingabe, bitte eine Zahl eingeben.");
                }
                else
                {
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
        return Console.ReadLine();
    }

    public static void displayMenu(Middleman middleman, int day)
    {
        Console.WriteLine($"{middleman.Name} von {middleman.CompanyName} | ${middleman.Balance:F2} | Lager: {middleman.StockCount}/{middleman.StockCapacity} | Tag {day}");
        Console.WriteLine("e) Einkaufen");
        Console.WriteLine("v) Verkaufen");
        Console.WriteLine("l) Lagerkapazität erhöhen");
        Console.WriteLine("b) Runde beenden");
    }

    public static void displayBuyingOption(List<Product> products)
    {
        Console.WriteLine($"Verfügbare Produkte:");
        displayProducts(products);
        Console.WriteLine("z) Zurück");
    }

    public static void displaySellingOption(Middleman middleman)
    {
        Console.WriteLine("Produkte im Besitz:");
        displayStock(middleman);
        Console.WriteLine("z) Zurück");
    }

    public static void displayStockOptions()
    {
        Console.WriteLine("Erhöhung der Lagerkapazität kostet 50$ pro Einheit.");
        Console.WriteLine("Um wie viel Einheiten soll Lager vergrößert werden? 1 Einheit = 50 Lager Kapazität.");
    }

    static void displayStock(Middleman middleman)
    {
        for (int i = 0; i < middleman.Stock.Count; i++)
        {
            Product product = middleman.Stock.ElementAt(i).Key;
            float sellingPrice = product.BuyingPrice * 0.8f;

            Console.WriteLine($"{i+1}) {product.Name} ({middleman.Stock.ElementAt(i).Value}) ${sellingPrice:F2}/Stück");
        }
    }

    static void displayProducts(List<Product> products)
    {
        for (int i = 1; i <= products.Count; i++)
        {
            Console.WriteLine($"{i}) {products[i - 1].ToString()}");
        }
    }

    public static void displayProductToBuy(Product product)
    {
        if (null != product)
        {
            Console.WriteLine($"Wie viel von {product.Name} kaufen?");
        }
    }

    public static void displayProductToSell(Middleman middleman, Product product)
    {
        if (middleman.Stock.ContainsKey(product))
        {
            Console.WriteLine($"Wieviel von {product.Name} verkaufen (max. {middleman.Stock[product]})? ");
        }
    }

    public static void displayLosingMiddleman(Middleman middleman)
    {
        Console.WriteLine($"{middleman.ToString()} has lost the game.");
    }
}
