using System;
using System.Diagnostics;

namespace EvoSim
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Start();
            
        }

        const int GENS = 1000;
        private Simulator s;

        public void Start()
        {
            var h = new Test1.Testhandler();
            this.s = new Simulator(h, GENS);
            s.InitPopulation();

            ConsoleKeyInfo k;

            Console.WriteLine("Created population...");
            //Console.WriteLine("Press a to advance, esc to exit");
            var sw = new Stopwatch();
            sw.Start();
            while (s.Generation < 1000)
            {
                s.Advance();
                //var k = Console.ReadKey(true);
                //if (k.Key == ConsoleKey.A)
                //{
                //    s.Advance();
                //}
                //else if (k.Key == ConsoleKey.Escape)
                //{
                //    break;
                //}
            }

            sw.Stop();
            Console.WriteLine("{0} gen simulation time: {1}ms", GENS, sw.ElapsedMilliseconds);
            int statGeneration = 0;
            while (true)
            {
                k = Console.ReadKey(true);
                if (k.Key == ConsoleKey.Escape)
                {
                    return;
                }
                else if (k.Key == ConsoleKey.DownArrow)
                {
                    if(++statGeneration >= GENS) statGeneration = GENS-1 ;
                }
                else if (k.Key == ConsoleKey.PageDown)
                {
                    statGeneration += 10;
                    if (statGeneration >= GENS) statGeneration = GENS - 1;
                }
                else if (k.Key == ConsoleKey.UpArrow)
                {
                    if(--statGeneration < 0 ) statGeneration = 0;
                }
                else if (k.Key == ConsoleKey.PageUp)
                {
                    statGeneration -= 10;
                    if (statGeneration < 0) statGeneration = 0;
                }
                else
                {
                    continue;
                }
                DisplayStats(statGeneration);
            }
        }

        private void DisplayStats(int generation)
        {
            GenerationInfo gi = s.GetGenerationInfo(generation);
            Console.WriteLine("Generation {0} avg fitness: {1}, < avg: {2}, >= avg: {3}", generation, gi.AvgFitness, gi.AvgBellow, gi.AvgAbove);
        }
    }
}
