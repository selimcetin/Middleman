using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_1
{
    public class GameController
    {
        static string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        public static List<Middleman> liMiddlemen = new List<Middleman>();
        public static List<Product> liProducts = Utils.parseYamlFile($"{projectDirectory}\\produkte.yml");

        public GameController()
        {
           
        }

        public static void init()
        {
            int numMiddleman = UiController.getIntFromReadLinePrompt("Wie viele Zwischenhändler nehmen teil? ");

            // Save all middlemen
            //-------------------
            for (int i = 1; i <= numMiddleman; i++)
            {
                string middlemanName = UiController.getStringFromReadLinePrompt($"Name von Zwischenhänder {i}: ");
                string companyName = UiController.getStringFromReadLinePrompt($"Name der Firma von {middlemanName}: ");
                int difficulty = UiController.getIntFromReadLinePrompt("Schwierigkeitsgrad auswählen: (1) Einfach, (2) Normal, (3) Schwer");

                liMiddlemen.Add(new Middleman(middlemanName, companyName, difficulty));
            }
        }

        public static void buyProduct(Middleman m, Product p, int quantity)
        {
            int cost = p.Baseprice * quantity;
            if (m.Balance > cost)
            {
                m.Balance -= cost;

                // If already in stock, increase quantity
                if (m.Stock.ContainsKey(p))
                {
                    m.Stock[p] += quantity;
                }
                // Otherwise add it to stock
                else
                {
                    m.Stock.Add(p, quantity);
                }
            }
        }

        public static void sellProduct(Middleman m,Product p, int quantity)
        {
            int sellPrice = (int)(p.Baseprice * 0.8 * quantity);

            if (quantity < m.Stock[p])
            {
                m.Balance += sellPrice;
            }
        }

        public static void buyProductViaInput(Middleman m, int idx, int quantity)
        {
            if (idx <= liProducts.Count)
            {
                buyProduct(m, liProducts[idx - 1], quantity);
            }
        }

        public static void sellProductViaInput(Middleman m, int  idx, int quantity)
        {
            if (idx <= liProducts.Count)
            {
                sellProduct(m, liProducts[idx - 1], quantity);
            }
        }

        public static Product getProductByIdx(Middleman m, int idx)
        {
            if (idx < m.Stock.Count)
            {
                return m.Stock.ElementAt(idx).Key;
            }
            return null;
        }
    }
}
