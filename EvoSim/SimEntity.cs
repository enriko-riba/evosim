using ParticlePhysics;
using System.Collections.Generic;

namespace EvoSim
{
    public class SimEntity
    {
        public SimEntity()
        {
            Components = new List<SimComponent>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Generation { get; set; }
        public string Species { get; set; }
        public float Fitness { get; set; }

        public List<SimComponent> Components { get; private set; }

        public string Genome { get; set; }

        public int[] CustomData { get; set; }

        public Simulation Simulation { get; set; }
        public override string ToString() => $"{Generation}-{Name}[{Id}]";
    }
}
