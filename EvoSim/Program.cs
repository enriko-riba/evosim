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
        const int ENTITIES = 4;
        const int GENS = 1000;
        private Simulator s;

        public void Start()
        {
            var h = new Test1.Testhandler();
            this.s = new Simulator(h, ENTITIES);
            s.InitPopulation();

            ConsoleKeyInfo k;

            Console.WriteLine("Created population...");
            Console.WriteLine("Simulating...");
            var sw = new Stopwatch();
            sw.Start();
            while (s.Generation < GENS)
            {
                s.Advance();
                var st = s.GetGenerationInfo(s.Generation-1);
                Console.WriteLine("finished generation {0} at {1}ms, avg fitness: {2}, best fitness: {3}", s.Generation, sw.ElapsedMilliseconds, st.AvgFitness, st.BestFitness);
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
                else if(k.Key == ConsoleKey.S)
                {
                    Console.WriteLine("Enter entity id to resimulate:");
                    var idStr = Console.ReadLine();
                    var id = int.Parse(idStr);
                    var entity = s.Simulate(id, 5); //  simulate 5 seconds life time
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
            Console.WriteLine("Generation {0,3:G3} avg fitness: {1,8:N4}, <avg: {2,3}, >=avg: {3,3}, best fitness: {4,10:N6}, entity: {5}", generation, gi.AvgFitness, gi.AvgBellow, gi.AvgAbove, gi.BestFitness, gi.BestEntity);
        }
    }
}
