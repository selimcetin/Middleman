using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_1
{
    public static class StateMachine
    {
        public static void startStateMachine(GameInfo gameInfo)
        {
            while (!GameController.isGameOver(gameInfo))
            {
                try
                {
                    gameInfo.CurrentMiddleman = gameInfo.MiddlemanList[gameInfo.CurrentPlayerIndex];

                    // Handle the current state afterwards
                    //------------------------------------
                    switch (gameInfo.GameState)
                    {
                        case GameState.TurnStart:
                            UiController.displayMenu(gameInfo.CurrentMiddleman, gameInfo.Day);
                            break;
                        case GameState.Buying_Product_Selection:
                            UiController.displayBuyingOption(gameInfo.ProductList);
                            gameInfo.TransactionType = TransactionType.Buying;
                            break;
                        case GameState.Selling_Product_Selection:
                            UiController.displaySellingOption(gameInfo.CurrentMiddleman);
                            gameInfo.TransactionType = TransactionType.Selling;
                            break;
                        case GameState.Buying_Amount:
                            UiController.displayProductToBuy(gameInfo.SelectedProduct);
                            break;
                        case GameState.Selling_Amount:
                            UiController.displayProductToSell(gameInfo.CurrentMiddleman, gameInfo.SelectedProduct);
                            break;
                        case GameState.UpgradeStorage_Amount:
                            UiController.displayStockOptions();
                            gameInfo.TransactionType = TransactionType.StorageUpgrade;
                            break;
                        case GameState.Transaction:
                            gameInfo.GameState = GameState.TurnStart;
                            handleTransaction(gameInfo);
                            continue;
                        case GameState.TurnEnd:
                            handleTurnEnd(gameInfo);
                            continue;
                    }

                    gameInfo.GameState = getNextStateFromInput(gameInfo);
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

            // Game over
            //----------
            UiController.displayScoreboard(gameInfo.MiddlemanList);
        }

        static GameState getNextStateFromInput(GameInfo gameInfo)
        {
            string input = Console.ReadLine();

            switch (gameInfo.GameState)
            {
                case GameState.TurnStart:
                    return getNextStateDuringTurnStart(input);
                case GameState.Buying_Product_Selection:
                case GameState.Selling_Product_Selection:
                    return getNextStateDuringProductSelection(gameInfo, input);
                case GameState.Buying_Amount:
                case GameState.Selling_Amount:
                case GameState.UpgradeStorage_Amount:
                    return getNextStateDuringAmountSelection(gameInfo, input);
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

        static GameState getNextStateDuringProductSelection(GameInfo gameInfo, string input)
        {
            switch (input)
            {
                case "z":
                    return GameState.TurnStart;
                default:
                    int inputValue = Utils.convertStringToInt(input);

                    // After selecting product, go to selecting amount
                    //-----------------------------------------------------------
                    if (GameState.Buying_Product_Selection == gameInfo.GameState)
                    {
                        gameInfo.SelectedProduct = GameController.getProductFromList(gameInfo.ProductList, inputValue);
                        return GameState.Buying_Amount;
                    }
                    else
                    {
                        gameInfo.SelectedProduct =
                            GameController.getProductFromStock(gameInfo.CurrentMiddleman, inputValue);
                        return GameState.Selling_Amount;
                    }
            }
        }

        static GameState getNextStateDuringAmountSelection(GameInfo gameInfo, string input)
        {
            int inputValue = Utils.convertStringToInt(input);

            if (inputValue > 0)
            {
                gameInfo.SelectedAmount = inputValue;
                return GameState.Transaction;
            }

            return GameState.TurnEnd;
        }

        static void handleTransaction(GameInfo gameInfo)
        {
            switch (gameInfo.TransactionType)
            {
                case TransactionType.Buying:
                    GameController.buyProduct(gameInfo.CurrentMiddleman, gameInfo.SelectedProduct,
                        gameInfo.SelectedAmount);
                    break;
                case TransactionType.Selling:
                    GameController.sellProduct(gameInfo.CurrentMiddleman, gameInfo.SelectedProduct,
                        gameInfo.SelectedAmount);
                    break;
                case TransactionType.StorageUpgrade:
                    GameController.buyStockUpgrade(gameInfo.CurrentMiddleman, gameInfo.SelectedAmount);
                    break;
            }
        }

        static void handleTurnEnd(GameInfo gameInfo)
        {
            gameInfo.CurrentPlayerIndex++;
            gameInfo.GameState = GameState.TurnStart;

            // Day is over, start new Day
            //---------------------------
            if (GameController.isNextDay(gameInfo.MiddlemanList, gameInfo.CurrentPlayerIndex))
            {
                executeChangesForNextDay(gameInfo);
            }

            GameController.payDailyStorageCost(gameInfo, gameInfo.CurrentMiddleman, gameInfo.MiddlemanList);
        }

        static void executeChangesForNextDay(GameInfo gameInfo)
        {
            gameInfo.Day++;
            gameInfo.CurrentPlayerIndex = 0;

            Utils.leftShiftListOrder(gameInfo.MiddlemanList);
            GameController.handleDailyProductionRateAdjustment(gameInfo.ProductList);
            GameController.handleDailyPriceAdjustment(gameInfo.ProductList);
        }
    }
}