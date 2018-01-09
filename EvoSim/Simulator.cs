using System;
using System.Collections.Generic;
using System.Linq;

namespace EvoSim
{
    class Simulator
    {
        private IEntityHandler handler;
        private List<SimEntity> entities = new List<SimEntity>();

        public Simulator(IEntityHandler handler, int entityCount)
        {
            this.handler = handler;
            this.entityCount = entityCount;
        }


        private int generation;
        private int entityCount;

        public void InitPopulation()
        {
            generation = 0;
            for (int i =0; i< entityCount; i++)
            {
                this.entities.Add(handler.CreateEntity());
            }
        }

        public void Advance()
        {
            Simulate();
            FinishStep();
        }

        private void Simulate()
        {

        }

        private void FinishStep()
        {
            UpdateFitness();
            this.entities.Sort((e1, e2) => Math.Sign(e1.Fitness - e2.Fitness));
            var sumFit = this.entities.Sum(e => e.Fitness);
            var avgFit = sumFit / entityCount;

            Console.WriteLine("Generation {0} avg fitness: {1}", generation, avgFit);
            Console.WriteLine("< avg: {0}, >= avg: {1}", 
                        entities.Where(e=> e.Fitness < avgFit).Count(), 
                        entities.Where(e=> e.Fitness >= avgFit).Count());

            generation++;
        }

        private void UpdateFitness()
        {
            for (int i = 0; i < entityCount; i++)
            {
                var fit = handler.EvaluateFitness(this.entities[i]);
                this.entities[i].Fitness = fit;
            }
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
