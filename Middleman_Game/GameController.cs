using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Middleman_Game
{
    public static class GameController
    {
        public static void initializeGameParameters(GameInfo gameInfo)
        {
            setNumberOfPlayers(gameInfo);
            setNumberOfTurnsToPlay(gameInfo);

            initializePlayerList(gameInfo);

            MarketController.adjustDailyProductionRate(gameInfo.ProductList);
        }

        static void initializePlayerList(GameInfo gameInfo)
        {
            for (int i = 0; i < gameInfo.NumberOfPlayers; i++)
            {
                string middlemanName = UiController.getStringFromReadLinePrompt($"Name von Zwischenhänder {i + 1}: ");
                string companyName = UiController.getStringFromReadLinePrompt($"Name der Firma von {middlemanName}: ");
                int difficulty =
                    UiController.getIntFromReadLinePrompt(
                        "Schwierigkeitsgrad auswählen (1) Einfach, (2) Normal, (3) Schwer: ");

                gameInfo.MiddlemanList.Add(new Middleman(middlemanName, companyName, difficulty));
            }

            gameInfo.CurrentMiddleman = gameInfo.MiddlemanList[0];
        }

        static void setNumberOfPlayers(GameInfo gameInfo)
        {
            gameInfo.NumberOfPlayers = UiController.getIntFromReadLinePrompt("Wie viele Zwischenhändler nehmen teil? ");
        }

        static void setNumberOfTurnsToPlay(GameInfo gameInfo)
        {
            gameInfo.LastDay =
                UiController.getIntFromReadLinePrompt("Wie viele Runden bzw. Tage sollen gespielt werden? ");
        }

        public static Product getProductFromList(List<Product> productList, int index)
        {
            if (index <= productList.Count)
            {
                return productList[index - 1];
            }

            throw new GameException(
                "Falsche Indexangabe für das Produkt. Bitte Index aus angezeigter Produktliste wählen.");
        }

        public static Credit getCreditFromList(List<Credit> creditList, int index)
        {
            if (index <= creditList.Count)
            {
                return creditList[index - 1];
            }

            throw new GameException(
                "Falsche Indexangabe für das Kredit. Bitte Index aus angezeigter Kreditliste wählen.");
        }

        public static Product getProductFromStock(Middleman middleman, int index)
        {
            if (index <= middleman.Stock.Count())
            {
                return middleman.Stock.ElementAt(index - 1).Key;
            }

            throw new GameException(
                "Falsche Indexangabe für das Produkt. Index muss <= Anzahl unterschiedlicher Produkte im Lager sein.");
        }

        public static void removeMiddlemanFromList(GameInfo gameInfo, Middleman middleman,
            List<Middleman> middlemanList)
        {
            gameInfo.CurrentPlayerIndex--;
            middlemanList.Remove(middleman);
        }

        public static void processBankruptMiddleman(GameInfo gameInfo, Middleman middleman,
            List<Middleman> middlemanList)
        {
            removeMiddlemanFromList(gameInfo, middleman, middlemanList);
            UiController.displayLosingMiddleman(middleman);
        }

        public static bool isNextDay(List<Middleman> middlemanList, int currentPlayerIndex)
        {
            if (currentPlayerIndex == middlemanList.Count())
            {
                return true;
            }

            return false;
        }

        public static bool isGameOver(GameInfo gameInfo)
        {
            if (gameInfo.Day > gameInfo.LastDay) return true;
            if (gameInfo.MiddlemanList.Count == 0) return true;

            return false;
        }

        public static void prepareNextDay(GameInfo gameInfo)
        {
            gameInfo.Day++;
            gameInfo.CurrentPlayerIndex = 0;
            Utils.leftShiftListOrder(gameInfo.MiddlemanList);
        }
    }
}