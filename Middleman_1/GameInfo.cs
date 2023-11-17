using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace Middleman_1
{
    public static class GameInfo
    {
        private static int currentPlayerIndex;
        private static int day;
        private static GameState gameState;
        private static TransactionType transactionType;
        private static Middleman currentMiddleman;

        private static string projectDirectory;
        private static List<Middleman> middlemanList;
        private static List<Product> productList;
        private static Product selectedProduct;
        private static int selectedAmount;
        

        public static int CurrentPlayerIndex { get => currentPlayerIndex; set => currentPlayerIndex = value; }
        public static int Day { get => day; set => day = value; }
        public static GameState GameState { get => gameState; set => gameState = value; }
        public static Middleman CurrentMiddleman { get => currentMiddleman; set => currentMiddleman = value; }
        public static string ProjectDirectory { get => projectDirectory; set => projectDirectory = value; }
        public static List<Middleman> MiddlemanList { get => middlemanList; set => middlemanList = value; }
        public static List<Product> ProductList { get => productList; set => productList = value; }
        public static Product SelectedProduct { get => selectedProduct; set => selectedProduct = value; }
        public static int SelectedAmount { get => selectedAmount; set => selectedAmount = value; }
        public static TransactionType TransactionType { get => transactionType; set => transactionType = value; }

        public static void init()
        {
            currentPlayerIndex = 0;
            day = 1;
            gameState = GameState.TurnStart;
            selectedProduct = null;
            selectedAmount = -1;
            
            projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            middlemanList = new List<Middleman>();
            productList = Utils.parseYamlFile($"{projectDirectory}\\produkte.yml");
        }
    }
}