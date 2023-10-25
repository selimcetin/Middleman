using Middleman_1;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Program
{
    enum GameState
    {
        Initializing,
        NewTurn,
        WaitForInput,
        Buying,
        Selling,
        GameEnd
    }

    private static List<Middleman> liMiddlemen = new List<Middleman>();
    private static List<Product> liProducts = Utils.parseYamlFile("C:\\Users\\Seroru\\source\\repos\\Middleman_1\\Middleman_1\\produkte.yml");

    static void Main()
    {
        GameState state = GameState.Initializing;
        int idx = 0;
        int day = 1;

        while(true)
        {
            string input = "";

            switch (state)
            {
                case GameState.Initializing:
                    Console.Write("Wie viele Zwischenhändler nehmen teil? ");
                    int numMiddleman = int.Parse(Console.ReadLine());

                    // Save all middlemen
                    //-------------------
                    for (int i = 1; i <= numMiddleman; i++)
                    {
                        Console.Write($"Name von Zwischenhänder {i}: ");
                        string name = Console.ReadLine();
                        Console.Write($"Name der Firma von {name}: ");
                        string companyName = Console.ReadLine();
                        Console.Write($"Schwierigkeitsgrad auswählen: (1) Einfach, (2) Normal, (3) Schwer");
                        int difficulty = int.Parse(Console.ReadLine());

                        liMiddlemen.Add(new Middleman(name, companyName, difficulty));
                    }
                    state = GameState.NewTurn;
                    break;
                case GameState.NewTurn:
                    Console.WriteLine($"{liMiddlemen[idx].Name} von {liMiddlemen[idx].CompanyName} | ${liMiddlemen[idx].Balance} | Tag {day}");
                    Console.WriteLine("e) Einkaufen");
                    Console.WriteLine("v) Verkaufen");
                    Console.WriteLine("b) Runde beenden");

                    state = GameState.WaitForInput;
                    break;
                case GameState.WaitForInput:
                    input = Console.ReadLine();

                    switch (input)
                    {
                        case "b":
                            idx++;
                            state = GameState.NewTurn;

                            // Increment day, reset index and switch order
                            //--------------------------------------------
                            if (idx == liMiddlemen.Count())
                            {
                                day++;
                                idx = 0;

                                Utils.switchListOrder(liMiddlemen);
                            }
                            break;
                        case "e":
                            state = GameState.Buying;
                            break;
                        case "v":
                            state = GameState.Selling;
                            break;
                        default:
                            break;
                    }
                    break;
                case GameState.Buying:
                    Console.WriteLine($"Verfügbare Produkte:");
                    Product.displayProducts(liProducts);
                    Console.WriteLine("z) Zurück");

                    input = Console.ReadLine();

                    switch (input)
                    {
                        case "z":
                            state = GameState.NewTurn;
                            break;
                        default:
                            Product p = Product.getProductByIdx(input, liProducts);
                            Product.displayProductToBuy(p);
                            string quantity = Console.ReadLine();
                            liMiddlemen[idx].buyProductViaInput(input, quantity, liProducts);

                            state = GameState.NewTurn;
                            break;
                    }
                    break;
                case GameState.Selling:
                    Console.WriteLine("Produkte im Besitz:");
                    liMiddlemen[idx].displayStock();
                    Console.WriteLine("z) Zurück");

                    input = Console.ReadLine();

                    switch (input)
                    {
                        case "z":
                            state = GameState.NewTurn;
                            break;
                        default:
                            Product p = Product.getProductByIdx(input, liProducts);
                            liMiddlemen[idx].displayProductToSell(p);
                            string quantity = Console.ReadLine();
                            liMiddlemen[idx].sellProductViaInput(input, quantity, liProducts);

                            state = GameState.NewTurn;
                            break;
                    }
                    break;
            }
        }
    }
}






