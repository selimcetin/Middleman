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

                    // There are two breaking case:
                    // Break: Next State is calculated from input
                    // Continue: Next State is already given inside the case without input
                    //--------------------------------------------------------------------
                    switch (gameInfo.GameState)
                    {
                        case GameState.TurnStart:
                            gameInfo.GameState = GameState.Menu;
                            handleTurnStart(gameInfo);
                            continue;
                        case GameState.Menu:
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
                            gameInfo.GameState = GameState.Menu;
                            handleTransaction(gameInfo);
                            continue;
                        case GameState.TurnEnd:
                            gameInfo.GameState = GameState.TurnStart;
                            handleTurnEnd(gameInfo);
                            continue;
                    }
                    // After break; (continue; wont reach until here):
                    //------------------------------------------------
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
            Console.ReadKey();
        }

        static GameState getNextStateFromInput(GameInfo gameInfo)
        {
            string input = UiController.getStringFromReadLinePrompt("Eingabe: ");

            switch (gameInfo.GameState)
            {
                case GameState.Menu:
                    return getNextStateDuringMenu(input);
                case GameState.Buying_Product_Selection:
                case GameState.Selling_Product_Selection:
                    return getNextStateDuringProductSelection(gameInfo, input);
                case GameState.Buying_Amount:
                case GameState.Selling_Amount:
                case GameState.UpgradeStorage_Amount:
                    return getNextStateDuringAmountSelection(gameInfo, input);
                case GameState.Transaction:
                    return GameState.Menu;
                default:
                    throw new GameException("Etwas ist schief gelaufen :S");
            }
        }

        static GameState getNextStateDuringMenu(string input)
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
                    return GameState.Menu;
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
                    MiddlemanController.buyProduct(gameInfo.CurrentMiddleman, gameInfo.SelectedProduct,
                        gameInfo.SelectedAmount);
                    break;
                case TransactionType.Selling:
                    MiddlemanController.sellProduct(gameInfo.CurrentMiddleman, gameInfo.SelectedProduct,
                        gameInfo.SelectedAmount);
                    break;
                case TransactionType.StorageUpgrade:
                    MiddlemanController.buyStockUpgrade(gameInfo.CurrentMiddleman, gameInfo.SelectedAmount);
                    break;
            }
        }

        static void handleTurnStart(GameInfo gameInfo)
        {
            UiController.displayReport(gameInfo.CurrentMiddleman);
            MiddlemanController.resetPreviousDayVariables(gameInfo.CurrentMiddleman);
        }

        static void handleTurnEnd(GameInfo gameInfo)
        {
            gameInfo.CurrentPlayerIndex++;
            

            // Day is over, start new Day
            //---------------------------
            if (GameController.isNextDay(gameInfo.MiddlemanList, gameInfo.CurrentPlayerIndex))
            {
                executeChangesForNextDay(gameInfo);
            }

            MiddlemanController.payDailyStorageCost(gameInfo, gameInfo.CurrentMiddleman, gameInfo.MiddlemanList);
        }

        static void executeChangesForNextDay(GameInfo gameInfo)
        {
            GameController.prepareNextDay(gameInfo);
            MarketController.adjustDailyProductionRate(gameInfo.ProductList);
            MarketController.adjustDailyProductPrice(gameInfo.ProductList);
        }
    }
}