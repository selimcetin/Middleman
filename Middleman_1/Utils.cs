using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Middleman_1
{
    internal class Utils
    {
        public static void switchListOrder<T>(List<T> list)
        {
            // This puts the first to the last index
            // Example:
            // Input:  1, 2, 3, 4
            // Output: 2, 3, 4, 1
            for (int i = 0; i < list.Count - 1; i++)
            {
                T temp = list[i];
                list[i] = list[i + 1];
                list[i + 1] = temp;
            }
        }

        public static List<Product> parseYamlFile(string pathToFile)
        {
            List<Product> products = new List<Product>();
            string text = File.ReadAllText(pathToFile);
            string[] yamlItems = text.Split("- ");

            foreach (string yamlItem in yamlItems)
            {
                if (yamlItem.Trim() == "")
                    continue;

                Product p = getProductFromYamlItem(yamlItem);

                if (p != null )
                    products.Add(p);
            }

            return products;
        }

        public static Product getProductFromYamlItem(string yamlItem)
        {
            Product p = new Product();
            string[] yamlLines = yamlItem.Split(
                            new string[] { "\r\n", "\r", "\n" },
                            StringSplitOptions.None);

            foreach (string yamlLine in yamlLines)
            {
                string temp = yamlLine;

                temp = temp.Trim();

                if (temp != "")
                {
                    string description = temp.Substring(0, temp.IndexOf(':')).Trim();
                    string value = temp.Substring(temp.IndexOf(':') + 2).Trim();

                    passPropertyValuesToProduct(p, description, value);
                }
            }

            p.BuyingPrice = p.BasePrice;

            return p;
        }

        static void passPropertyValuesToProduct(Product p, string propertyName, string value)
        {
            Type type = p.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if(propertyName.ToLower() == property.Name.ToLower())
                {
                    if (IsNumeric(value))
                        property.SetValue(p, convertStringToInt(value));
                    else
                        property.SetValue(p, value);
                }
            }
        }

        public static int convertStringToInt(string str)
        {
            if(IsNumeric(str))
            {
                return int.Parse(str);
            }
            else
            {
                // TODO Error handling
                return 0;
            }
        }

        public static bool IsNumeric(string input)
        {
            // Try to parse the input as a number (int, double, etc.)
            // If successful, it's a number; otherwise, it's not
            return double.TryParse(input, out _);
        }
    }
}
