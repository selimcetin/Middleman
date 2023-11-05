using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_1
{
    public class Middleman
    {
        private string name;
        private string companyName;
        private int balance;
        private Dictionary<Product, int> stock;

        public string Name { get => name; set => name = value; }
        public string CompanyName { get => companyName; set => companyName = value; }
        public int Balance { get => balance; set => balance = value; }
        public Dictionary<Product, int> Stock { get => stock; set => stock = value; }

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
    }
}