using System;
using System.Linq;

namespace EvoSim
{
    class Simulator
    {
        private IEntityHandler handler;
        private SimEntity[] entities;
        private GenerationInfo[] stats;

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
            stats = new GenerationInfo[entityCount];

            Generation = 0;
            for (int i =0; i< entityCount; i++)
            {
                this.entities[i] = handler.CreateEntity();
            }

            handler.SetupSimulation(this.entities);
        }

        public void Advance()
        {
            handler.Simulate();
            FinishStep();
        }

        private void Simulate()
        {
            
        }

        private void FinishStep()
        {
            UpdateFitness();

            //  calc stats
            var sumFit = this.entities.Sum(e => e.Fitness);
            var avgFit = sumFit / entityCount;
            var stat = new GenerationInfo()
            {
                AvgFitness = avgFit,
                AvgAbove = entities.Where(e => e.Fitness >= avgFit).Count(),
                AvgBellow = entities.Where(e => e.Fitness < avgFit).Count()
            };
            this.stats[Generation++] = stat;

            //  kill lowest 50% (replace with mutations)
            for(int i = 0; i< 500; i++)
            {
                this.entities[i] = handler.MutateEntity(this.entities[i + 500]);
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

        private void MutatePopulation()
        {
            for (int i = 0; i < entityCount; i++)
            {
                var me = handler.MutateEntity(this.entities[i]);
            }
        }
    }
}
