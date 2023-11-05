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
            string filePath = pathToFile;
            List<Product> list = new List<Product>();

            try
            {
                if (File.Exists(filePath))
                {
                    string text = File.ReadAllText(filePath);
                    string pattern = "- [aA-zZ]*: ([aA-zZ]|[0-9])*(\r\n)(  ([aA-zZ])*: ([aA-zZ]|[0-9])*(\r\n)*)*";

                    MatchCollection matches = Regex.Matches(text, pattern);

                    if (matches.Count > 0)
                    {
                        foreach(Match match in matches)
                        {
                            list.Add(getProductFromYamlObject(match.Value));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("The product.yml file does not exist.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return list;
        }

        public static Product getProductFromYamlObject(string yamlObject)
        {
            string patternKey = "[aA-zZ]*:";
            string patternValue = ": ([aA-zZ]|[0-9])*";
            Product product = new Product();

            MatchCollection matchesKey = Regex.Matches(yamlObject, patternKey);
            MatchCollection matchesValue = Regex.Matches(yamlObject, patternValue);

            if (matchesKey.Count > 0 && (matchesKey.Count == matchesValue.Count))
            {
                Type type = product.GetType();
                PropertyInfo[] properties = type.GetProperties();
                
                for(int i=0; i < matchesKey.Count; i++)
                {
                    foreach (PropertyInfo property in properties)
                    {
                        if (matchesKey[i].Value.Contains(property.Name.ToLower()))
                        {
                            string yamlValue = matchesValue[i].Value.Remove(0, 2);

                            if (IsNumeric(yamlValue))
                                property.SetValue(product, int.Parse(yamlValue));
                            else
                                property.SetValue(product, yamlValue);
                        }
                    }
                }
            }

            return product;
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
