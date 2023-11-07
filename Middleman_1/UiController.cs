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
                    throw new GameException("Falsche Eingabe, bitte eine Zahl eingeben");
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

    public static void displayNewTurn(Middleman m, int day)
    {
        Console.WriteLine($"{m.Name} von {m.CompanyName} | ${m.Balance} | Lager: {m.StockCount}/{m.StockCapacity} | Tag {day}");
        Console.WriteLine("e) Einkaufen");
        Console.WriteLine("v) Verkaufen");
        Console.WriteLine("l) Lagerkapazität erhöhen");
        Console.WriteLine("b) Runde beenden");
    }

    public static void displayBuyingOption(List<Product> liProducts)
    {
        Console.WriteLine($"Verfügbare Produkte:");
        displayProducts(liProducts);
        Console.WriteLine("z) Zurück");
    }

    public static void displaySellingOption(Middleman m)
    {
        Console.WriteLine("Produkte im Besitz:");
        displayStock(m);
        Console.WriteLine("z) Zurück");
    }

    public static void displayStockOptions()
    {
        Console.WriteLine("Erhöhung der Lagerkapazität kostet 50$ pro Einheit.");
        Console.WriteLine("Um wie viel Einheiten soll Lager vergrößert werden? 1 Einheit = 50 Lager Kapazität.");
    }

    static void displayStock(Middleman m)
    {
        for (int i = 0; i < m.Stock.Count; i++)
        {
            Product temp = m.Stock.ElementAt(i).Key;
            int tempBasePrice = (int)(temp.BuyingPrice * 0.8);

            Console.WriteLine($"{i+1}) {temp.Name} ({m.Stock.ElementAt(i).Value}) ${tempBasePrice}/Stück");
        }
    }

    public static void displayProducts(List<Product> liProducts)
    {
        for (int i = 1; i <= liProducts.Count; i++)
        {
            Console.WriteLine($"{i}) {liProducts[i - 1].ToString()}");
        }
    }

    public static void displayProductToBuy(Product p)
    {
        if (null != p)
        {
            Console.WriteLine($"Wie viel von {p.Name} kaufen?");
        }
    }

    public static void displayProductToSell(Middleman m, Product p)
    {
        if (m.Stock.ContainsKey(p))
        {
            Console.WriteLine($"Wieviel von {p.Name} verkaufen (max. {m.Stock[p]})? ");
        }
    }



}
