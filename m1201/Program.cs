using System;

using m1201.Library;

namespace m1201
{
    class Program
    {
        static void Main()
        {
            Console.Write("Enter site url: ");

            var url = Console.ReadLine();

            var scraper = new Scraper(url);

            scraper.Do();

            Console.WriteLine("Download completed.");

            Console.ReadKey();
        }
    }
}
