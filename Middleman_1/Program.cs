using Middleman_1;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Program
{
    enum GameState
    {
        NewTurn,
        WaitForInput,
        Buying,
        Selling,
        StockAdjustment,
        GameEnd
    }

    static void Main()
    {
        GameState state = GameState.NewTurn;
        int currPlayerIdx = 0;
        int day = 1;

        GameController.init();

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

                        state = GameState.WaitForInput;
                        break;
                    case GameState.WaitForInput:
                        input = Console.ReadLine();

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
                                break;
                        }
                        break;
                    case GameState.Buying:
                        UiController.displayBuyingOption(GameController.liProducts);

                        input = Console.ReadLine();

                        switch (input)
                        {
                            case "z":
                                state = GameState.NewTurn;
                                break;
                            default:
                                int inputIdx = Utils.convertStringToInt(input);
                                Product p = GameController.getProductByIdx(inputIdx);
                                UiController.displayProductToBuy(p);

                                int quantity = Utils.convertStringToInt(Console.ReadLine());
                                GameController.buyProductViaInput(currMiddleman, inputIdx, quantity);

                                state = GameState.NewTurn;
                                break;
                        }
                        break;
                    case GameState.Selling:
                        Console.WriteLine("Produkte im Besitz:");
                        UiController.displayStock(currMiddleman);
                        Console.WriteLine("z) Zurück");

                        input = Console.ReadLine();

                        switch (input)
                        {
                            case "z":
                                state = GameState.NewTurn;
                                break;
                            default:
                                int inputIdx = Utils.convertStringToInt(input);
                                Product p = GameController.getProductByIdx(inputIdx);
                                UiController.displayProductToSell(currMiddleman, p);
                                
                                int quantity = Utils.convertStringToInt(Console.ReadLine());
                                GameController.sellProductViaInput(currMiddleman, inputIdx, quantity);

                                state = GameState.NewTurn;
                                break;
                        }
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
}






