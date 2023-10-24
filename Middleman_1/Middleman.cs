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

        public Middleman(string name, string companyName)
        {
            this.name = name;
            this.companyName = companyName;
        }

        public string getName()
        {
            return name;
        }

        public string getCompanyName()
        {
            return companyName;
        }
    }
}