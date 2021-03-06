﻿namespace EvoSim
{
    public interface IEntityHandler
    {
        SimEntity CreateEntity();

        SimEntity CreateEntity(string genome);

        SimEntity CloneEntity(SimEntity e);

        SimEntity MutateEntity(SimEntity e);

        float EvaluateFitness(SimEntity e);

        void SetupSimulation(SimEntity[] entities);
        void Simulate(SimEntity[] entities, float seconds);
    }
}
