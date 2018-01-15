using System;
using System.Collections.Generic;
using System.Linq;

namespace EvoSim
{
    class Simulator
    {
        private IEntityHandler handler;
        private SimEntity[] entities;
        private List<GenerationInfo> stats;

        private int entityCount;

        public Simulator(IEntityHandler handler, int entityCount)
        {
            this.handler = handler;
            this.entityCount = entityCount;
        }

        public GenerationInfo GetGenerationInfo(int generation)
        {
            return stats[generation];
        }

        public int Generation { get; private set; }

        public void InitPopulation()
        {
            entities = new SimEntity[entityCount];
            stats = new List<GenerationInfo>();
            Generation = 0;
            for (int i =0; i< entityCount; i++)
            {
                this.entities[i] = handler.CreateEntity();
            }

            handler.SetupSimulation(this.entities);
        }

        public void Advance()
        {
            const float SIMULATE_SECONDS = 5;
            handler.Simulate(this.entities, SIMULATE_SECONDS);
            FinishStep();
            this.Generation++;
        }

        public SimEntity Simulate(int id, float seconds)
        {
            var e = this.entities[id];  //  TODO: fix this ... save all entities in dictionary and return by id
            const float SIMULATE_SECONDS = 5;
            this.handler.Simulate(new SimEntity[] { e }, SIMULATE_SECONDS);
            return e;
        }
        private void FinishStep()
        {
            UpdateFitness();

            //  calc stats
            var sumFit = this.entities.Sum(e => e.Fitness);
            var avgFit = sumFit / entityCount;
            var bf = entities.Max(e => e.Fitness);
            var stat = new GenerationInfo()
            {
                AvgFitness = avgFit,
                AvgAbove = entities.Where(e => e.Fitness >= avgFit).Count(),
                AvgBellow = entities.Where(e => e.Fitness < avgFit).Count(),
                BestFitness = bf,
                BestEntity = entities.First(e=> e.Fitness == bf)
            };
            this.stats.Add(stat);
            //this.stats[Generation++] = stat;

            //  kill lowest 50% (replace with mutations)
            var half = this.entityCount / 2;
            for(int i = 0; i< half; i++)
            {
                this.entities[i] = handler.MutateEntity(this.entities[i + half]);
                this.entities[i].Generation = this.Generation;
            }            
        }


        private void UpdateFitness()
        {
            for (int i = 0; i < entityCount; i++)
            {
                var fit = handler.EvaluateFitness(this.entities[i]);
                this.entities[i].Fitness = fit;
            }

            Array.Sort(this.entities, (e1, e2) => Math.Sign(e1.Fitness - e2.Fitness));
        }        
    }
}
