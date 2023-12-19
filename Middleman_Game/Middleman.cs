using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_Game
{
    public class Middleman
    {
        private string name;
        private string companyName;
        private float balance;
        private Dictionary<Product, int> stock;
        private int stockCount;
        private int stockCapacity;
        private float balancePreviousDay;
        private float buyingCostPreviousDay;
        private float salesPreviousDay;
        private float storageCostPreviousDay;
        private Credit? credit;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string CompanyName
        {
            get => companyName;
            set => companyName = value;
        }

        public float Balance
        {
            get => balance;
            set => balance = value;
        }

        public Dictionary<Product, int> Stock
        {
            get => stock;
            set => stock = value;
        }

        public int StockCount
        {
            get => stockCount;
            set => stockCount = value;
        }

        public int StockCapacity
        {
            get => stockCapacity;
            set => stockCapacity = value;
        }

        public float BalancePreviousDay
        {
            get => balancePreviousDay;
            set => balancePreviousDay = value;
        }

        public float BuyingCostPreviousDay
        {
            get => buyingCostPreviousDay;
            set => buyingCostPreviousDay = value;
        }

        public float SalesPreviousDay
        {
            get => salesPreviousDay;
            set => salesPreviousDay = value;
        }

        public float StorageCostPreviousDay
        {
            get => storageCostPreviousDay;
            set => storageCostPreviousDay = value;
        }

        public Credit? Credit
        {
            get => credit;
            set => credit = value;
        }

        public Middleman(string name, string companyName, int difficulty)
        {
            this.name = name;
            this.companyName = companyName;
            stock = new Dictionary<Product, int>();
            stockCount = 0;
            stockCapacity = 100;
            credit = null;
            balancePreviousDay = balance;

            MiddlemanController.setInitialBalance(this, difficulty);
        }

        public override string? ToString()
        {
            return $"{name} der Firma {companyName}";
        }
    }
}