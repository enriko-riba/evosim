namespace EvoSim
{
    class GenerationInfo
    {
        public int Generation { get; set; }
        public float AvgFitness { get; set; }
        public int AvgBellow { get; set; }
        public int AvgAbove { get; set; }
        public float BestFitness { get; set; }
        public SimEntity BestEntity { get; set; }
    }
}
