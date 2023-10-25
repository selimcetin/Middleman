using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_1
{
    internal class Middleman
    {
        private string name;
        private string companyName;
        private int balance;
        private Dictionary<Product, int> stock;

        public string Name { get => name; set => name = value; }
        public string CompanyName { get => companyName; set => companyName = value; }
        public int Balance { get => balance; set => balance = value; }

        public Middleman(string name, string companyName, int difficulty)
        {
            this.name = name;
            this.companyName = companyName;
            stock = new Dictionary<Product, int>();

            switch (difficulty)
            {
                case 1:
                    balance = 15000;
                    break;
                case 2:
                    balance = 10000;
                    break;
                case 3:
                    balance = 7000;
                    break;
                default:
                    balance = 10000;
                    break;
            }
        }

        public int buyProduct(Product p, int quantity)
        {
            int cost = p.Baseprice * quantity;
            if (balance > cost)
            {
                balance -= cost;

                // If already in stock, increase quantity
                if (stock.ContainsKey(p))
                {
                    stock[p] += quantity;
                }
                // Otherwise add it to stock
                else
                {
                    stock.Add(p, quantity);
                }
                return 0;
            }

            // Error
            return 1;
        }

        public int sellProduct(Product p, int quantity)
        {
            int sellPrice = (int) (p.Baseprice * 0.8 * quantity);

            if(quantity < stock[p])
            {
                balance += sellPrice;
                return 0;
            }

            return 1;
        }

        public int buyProductViaInput(string strIdx, string quantity, List<Product> liProducts)
        {
            int tempIdx;
            int tempQuantity;
            if (Utils.IsNumeric(strIdx) && Utils.IsNumeric(quantity))
            {
                tempIdx = int.Parse(strIdx);
                tempQuantity = int.Parse(quantity);
            }
            else
            {
                return 1;
            }

            if(tempIdx <= liProducts.Count)
            {
                buyProduct(liProducts[tempIdx - 1], tempQuantity);
                return 0;
            }

            return 2;
        }

        public int sellProductViaInput(string strIdx, string quantity, List<Product> liProducts)
        {
            int tempIdx;
            int tempQuantity;
            if (Utils.IsNumeric(strIdx) && Utils.IsNumeric(quantity))
            {
                tempIdx = int.Parse(strIdx);
                tempQuantity = int.Parse(quantity);
            }
            else
            {
                return 1;
            }

            if (tempIdx <= liProducts.Count)
            {
                sellProduct(liProducts[tempIdx - 1], tempQuantity);
                return 0;
            }

            return 2;
        }

        public void displayStock()
        {
            for (int i = 1; i <= stock.Count; i++)
            {
                Product temp = stock.ElementAt(i - 1).Key;
                int tempBasePrice = (int) (temp.Baseprice * 0.8);

                Console.WriteLine($"{i}) {temp.Name} ({stock.ElementAt(i - 1).Value}) ${tempBasePrice}/Stück");
            }
        }

        public void displayProductToSell(Product p)
        {
            if(stock.ContainsKey(p))
            {
                Console.WriteLine($"Wieviel von {p.Name} verkaufen (max. {stock[p]})? ");
            }
        }

        public Product getProductByIdx(string strIdx)
        {
            if (Utils.IsNumeric(strIdx))
            {
                int tempIdx = int.Parse(strIdx);

                if (tempIdx < stock.Count)
                {
                    return stock.ElementAt(tempIdx).Key;
                }
            }
            return null;
        }
    }
}