using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace Middleman_Game
{
    public class GameInfo
    {
        private static GameInfo instance; // Singleton

        private int currentPlayerIndex;
        private int day;
        private int lastDay;
        private GameState gameState;
        private TransactionType transactionType;
        private Middleman currentMiddleman;

        private string projectDirectory;
        private List<Middleman> middlemanList;
        private List<Product> productList;
        private List<Credit> creditList;
        private Product selectedProduct;
        private Credit selectedCredit;
        private int selectedAmount;
        private int numberOfPlayers;
        


        public int CurrentPlayerIndex
        {
            get => currentPlayerIndex;
            set => currentPlayerIndex = value;
        }

        public int Day
        {
            get => day;
            set => day = value;
        }

        public GameState GameState
        {
            get => gameState;
            set => gameState = value;
        }

        public Middleman CurrentMiddleman
        {
            get => currentMiddleman;
            set => currentMiddleman = value;
        }

        public List<Middleman> MiddlemanList
        {
            get => middlemanList;
            set => middlemanList = value;
        }

        public List<Product> ProductList
        {
            get => productList;
            set => productList = value;
        }

        public List<Credit> CreditList
        {
            get => creditList;
            set => creditList = value;
        }

        public Product SelectedProduct
        {
            get => selectedProduct;
            set => selectedProduct = value;
        }

        public int SelectedAmount
        {
            get => selectedAmount;
            set => selectedAmount = value;
        }

        public Credit SelectedCredit
        {
            get => selectedCredit;
            set => selectedCredit = value;
        }

        public TransactionType TransactionType
        {
            get => transactionType;
            set => transactionType = value;
        }

        public int LastDay
        {
            get => lastDay;
            set => lastDay = value;
        }

        public int NumberOfPlayers
        {
            get => numberOfPlayers;
            set => numberOfPlayers = value;
        }


        private GameInfo()
        {
            currentPlayerIndex = 0;
            day = 1;
            gameState = GameState.TurnStart;
            selectedProduct = null;
            selectedAmount = -1;
            selectedCredit = null;

            projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            middlemanList = new List<Middleman>();
            creditList = new List<Credit>();
            creditList.Add(new Credit(5000, 3));
            creditList.Add(new Credit(10000, 5));
            creditList.Add(new Credit(25000, 8));
            productList = Utils.parseYamlFile($"{projectDirectory}\\produkte.yml");
        }

        public static GameInfo Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new GameInfo();
                }

                return instance;
            }
        }
    }
}