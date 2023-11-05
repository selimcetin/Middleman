using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_1
{
    public class Product
    {
        private string name;
        private int durability;
        private int baseprice;
        private int minProductionRate;
        private int maxProductionRate;

        public string Name { get => name; set => name = value; }
        public int Durability { get => durability; set => durability = value; }
        public int Baseprice { get => baseprice; set => baseprice = value; }
        public int MinProductionRate { get => minProductionRate; set => minProductionRate = value; }
        public int MaxProductionRate { get => maxProductionRate; set => maxProductionRate = value; }

        public Product(string name, int durability, int price)
        {
            this.name = name;
            this.durability = durability;
            this.baseprice = price;
        }

        public Product()
        {
           
        }

        public override string? ToString()
        {
            return $"{name} ({durability} Tage) ${baseprice}/Stück";
        }
    }
}
