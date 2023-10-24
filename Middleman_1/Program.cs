using Middleman_1;
using System.Collections.Generic;

List<Middleman> liMiddlemen = new List<Middleman>();


while(true)
{
    Console.Write("Wie viele Zwischenhändler nehmen teil? ");
    int numMiddleman = int.Parse(Console.ReadLine());

    // Save all middlemen
    //-------------------
    for(int i=1; i <= numMiddleman; i++)
    {
        Console.Write($"Name von Zwischenhänder {i}: ");
        string name = Console.ReadLine();
        Console.Write($"Name der Firma von {name}: ");
        string companyName = Console.ReadLine();

        liMiddlemen.Add(new Middleman(name,companyName));
    }

    // Hotseat gameplay
    //-----------------
    int idx = 0;
    int day = 1;
    while(true)
    {
        Console.WriteLine($"{liMiddlemen[idx].getName()} von {liMiddlemen[idx].getCompanyName()} | Tag {day}");
        Console.WriteLine("b) Runde beenden");

        while ("b" != Console.ReadLine()) { }

        idx++;

        // Increment day, reset index and switch order
        //--------------------------------------------
        if (idx == liMiddlemen.Count())
        {
            day++;
            idx = 0;

            Utils.switchListOrder(liMiddlemen);
        }

        
    }
}