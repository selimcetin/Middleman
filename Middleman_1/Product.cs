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

        public string Name { get => name; set => name = value; }
        public int Durability { get => durability; set => durability = value; }
        public int Baseprice { get => baseprice; set => baseprice = value; }

        public Product(string name, int durability, int price)
        {
            this.name = name;
            this.durability = durability;
            this.baseprice = price;
        }

        public Product()
        {
           
        }


        public static Product getProductByIdx(string strIdx, List<Product> liProducts)
        {
            if (Utils.IsNumeric(strIdx))
            {
                int tempIdx = int.Parse(strIdx);

                if(tempIdx <= liProducts.Count)
                {
                    return liProducts[tempIdx-1];
                }
            }
            return null;
        }



        public override string? ToString()
        {
            return $"{name} ({durability} Tage) ${baseprice}/Stück";
        }
    }
}
