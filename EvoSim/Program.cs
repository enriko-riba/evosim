using System;

namespace EvoSim
{
    class Program
    {
        static void Main(string[] args)
        {
            var h = new Test1.Testhandler();
            var s = new Simulator(h, 1000);
            s.InitPopulation();

            Console.WriteLine("Created population.");
            Console.WriteLine("Press a to advance, esc to exit");
            while (true)
            {
                var k = Console.ReadKey(true);
                if (k.Key == ConsoleKey.A)
                {
                    s.Advance();
                }
                else if (k.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }
    }
}
