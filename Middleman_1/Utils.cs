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
    public class Utils
    {
        public static void leftShiftListOrder<T>(List<T> list)
        {
            // This method shifts the order to the left
            // Left edge loops back to right edge
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

                Product product = getProductFromYamlItem(yamlItem);

                if (product != null)
                    products.Add(product);
            }

            return products;
        }

        public static Product getProductFromYamlItem(string yamlItem)
        {
            Product product = new Product();

            // Split yaml file by all possible new line expressions
            //-----------------------------------------------------
            string[] yamlLines = yamlItem.Split(
                new string[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);

            foreach (string yamlLine in yamlLines)
            {
                string yamlLineCopy = yamlLine;

                yamlLineCopy = yamlLineCopy.Trim();

                if (yamlLineCopy != "")
                {
                    string description = yamlLineCopy.Substring(0, yamlLineCopy.IndexOf(':')).Trim();
                    string value = yamlLineCopy.Substring(yamlLineCopy.IndexOf(':') + 2).Trim();

                    passPropertyValuesToProduct(product, description, value);
                }
            }

            product.BuyingPrice = product.BasePrice;

            return product;
        }

        static void passPropertyValuesToProduct(Product product, string propertyName, string value)
        {
            Type type = product.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (propertyName.ToLower() == property.Name.ToLower())
                {
                    if (isNumeric(value))
                        property.SetValue(product, convertStringToInt(value));
                    else
                        property.SetValue(product, value);
                }
            }
        }

        public static int convertStringToInt(string str)
        {
            if (isNumeric(str))
            {
                return int.Parse(str);
            }

            throw new GameException("Falsche Eingabe. Bitte eine Zahl eingeben.");
        }

        public static bool isNumeric(string input)
        {
            // Try to parse the input as a number (int, double, etc.)
            // If successful, it's a number; otherwise, it's not
            return double.TryParse(input, out _);
        }
    }
}