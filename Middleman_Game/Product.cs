using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_Game
{
    public class Product
    {
        private string name;
        private int durability;
        private float basePrice;
        private int minProductionRate;
        private int maxProductionRate;
        private int availableAmount;
        private float buyingPrice;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int Durability
        {
            get => durability;
            set => durability = value;
        }

        public float BasePrice
        {
            get => basePrice;
            set => basePrice = value;
        }

        public int MinProductionRate
        {
            get => minProductionRate;
            set => minProductionRate = value;
        }

        public int MaxProductionRate
        {
            get => maxProductionRate;
            set => maxProductionRate = value;
        }

        public int AvailableAmount
        {
            get => availableAmount;
            set => availableAmount = value;
        }

        public float BuyingPrice
        {
            get => buyingPrice;
            set => buyingPrice = value;
        }

        public Product()
        {
            availableAmount = 0;
        }

        public override string? ToString()
        {
            return $"{name} ({durability} Tage) ${buyingPrice:F2}/Stück | Verfügbare Menge: {availableAmount}";
        }
    }
}