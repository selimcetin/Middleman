using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_1
{
    public static class StateMachine
    {
        public static void startStateMachine()
        {
            while (true)
            {
                try
                {
                    GameInfo.CurrentMiddleman = GameInfo.MiddlemanList[GameInfo.CurrentPlayerIndex];

                    // Handle the current state afterwards
                    //------------------------------------
                    switch (GameInfo.GameState)
                    {
                        case GameState.TurnStart:
                            UiController.displayMenu(GameInfo.CurrentMiddleman, GameInfo.Day);
                            break;
                        case GameState.Buying_Product_Selection:
                            UiController.displayBuyingOption(GameInfo.ProductList);
                            GameInfo.TransactionType = TransactionType.Buying;
                            break;
                        case GameState.Selling_Product_Selection:
                            UiController.displaySellingOption(GameInfo.CurrentMiddleman);
                            GameInfo.TransactionType = TransactionType.Selling;
                            break;
                        case GameState.Buying_Amount:
                            UiController.displayProductToBuy(GameInfo.SelectedProduct);
                            break;
                        case GameState.Selling_Amount:
                            UiController.displayProductToSell(GameInfo.CurrentMiddleman, GameInfo.SelectedProduct);
                            break;
                        case GameState.UpgradeStorage_Amount:
                            UiController.displayStockOptions();
                            GameInfo.TransactionType = TransactionType.StorageUpgrade;
                            break;
                        case GameState.Transaction:
                            GameInfo.GameState = GameState.TurnStart;
                            handleTransaction();
                            continue;
                        case GameState.TurnEnd:
                            handleTurnEnd(GameInfo.CurrentMiddleman);
                            break;
                    }

                    GameInfo.GameState = getNextStateFromInput(GameInfo.GameState);
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

        static GameState getNextStateFromInput(GameState currentState)
        {
            string input = Console.ReadLine();

            switch (currentState)
            {
                case GameState.TurnStart:
                    return getNextStateDuringTurnStart(input);
                case GameState.Buying_Product_Selection:
                case GameState.Selling_Product_Selection:
                    return getNextStateDuringProductSelection(currentState, input);
                case GameState.Buying_Amount:
                case GameState.Selling_Amount:
                case GameState.UpgradeStorage_Amount:
                    return getNextStateDuringAmountSelection(input);
                case GameState.Transaction:
                    return GameState.TurnStart;
                default:
                    throw new GameException("Etwas ist schief gelaufen :S");
            }
        }

        static GameState getNextStateDuringTurnStart(string input)
        {
            switch (input)
            {
                case "e":
                    return GameState.Buying_Product_Selection;
                case "v":
                    return GameState.Selling_Product_Selection;
                case "l":
                    return GameState.UpgradeStorage_Amount;
                case "b":
                    return GameState.TurnEnd;
                default:
                    throw new GameException("Falsche Eingabe. Bitte eines der oberen Optionen auswählen.");
            }
        }

        static GameState getNextStateDuringProductSelection(GameState currentState, string input)
        {
            switch (input)
            {
                case "z":
                    return GameState.TurnStart;
                default:
                    int inputValue = Utils.convertStringToInt(input);

                    // After selecting product, go to selecting amount
                    //------------------------------------------------
                    if (GameState.Buying_Product_Selection == currentState)
                    {
                        GameInfo.SelectedProduct = GameController.getProductFromList(inputValue);
                        return GameState.Buying_Amount;
                    }
                    else
                    {
                        GameInfo.SelectedProduct = GameController.getProductFromStock(GameInfo.CurrentMiddleman, inputValue);
                        return GameState.Selling_Amount;
                    }  
            }
        }

        static GameState getNextStateDuringAmountSelection(string input)
        {
            int inputValue = Utils.convertStringToInt(input);

            if (inputValue > 0)
            {
                GameInfo.SelectedAmount = inputValue;
                return GameState.Transaction;
            }
            else
            {
                return GameState.TurnEnd;
            }
        }

        static void handleTransaction()
        {
            switch (GameInfo.TransactionType)
            {
                case TransactionType.Buying:
                    GameController.buyProduct(GameInfo.CurrentMiddleman, GameInfo.SelectedProduct, GameInfo.SelectedAmount);
                    break;
                case TransactionType.Selling:
                    GameController.sellProduct(GameInfo.CurrentMiddleman, GameInfo.SelectedProduct, GameInfo.SelectedAmount);
                    break;
                case TransactionType.StorageUpgrade:
                    GameController.buyStockUpgrade(GameInfo.CurrentMiddleman, GameInfo.SelectedAmount);
                    break;
            }
        }

        static void handleTurnEnd(Middleman middleman)
        {
            GameInfo.CurrentPlayerIndex++;
            GameInfo.GameState = GameState.TurnStart;
            GameController.payDailyStorageCost(middleman);

            // Day is over, start new Day
            //---------------------------
            if (GameController.isNextDay())
            {
                executeChangesForNextDay();
            }
        }

        static void executeChangesForNextDay()
        {
            GameController.updateGameInfoForNextDay();

            Utils.leftShiftListOrder(GameInfo.MiddlemanList);
            GameController.handleDailyProductionRateAdjustment();
            GameController.handleDailyPriceAdjustment();
        }
    }
}

