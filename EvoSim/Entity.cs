using System;
using System.Collections.Generic;
using System.Text;

namespace EvoSim
{
    class Entity
    {
        public Entity()
        {
            Components = new List<Component>();
        }
        public string Name { get; set; }
        public int Generation { get; set; }
        public string Species { get; set; }
        public string Genome { get; set; }

        public float Fitness { get; set; }

        public List<Component> Components { get; private set; }

        public static unsafe Entity FromGenome(string genome)
        {
            var e = new Entity();

            int nodeCounter = 0;
            var components = genome.Split('|');
            foreach(var c in components)
            {
                var component = new Component();
                var data = c.Split(';');
                if(data[0] == "N")
                {
                    //  node
                    //  e.g. "N;1.35"
                    component.Attributes = new float[1];
                    component.Attributes[0] = float.Parse(data[1]); // friction
                    component.Name = $"Node{++nodeCounter}";
                }
                else
                {
                    //  connection
                    //  e.g. "C;2.0;0.5;1.0;1;2"
                    component.Attributes = new float[3];
                    component.Attributes[0] = float.Parse(data[1]); // length
                    component.Attributes[1] = float.Parse(data[2]); // contraction
                    component.Attributes[2] = float.Parse(data[3]); // period
                    component.Name = $"Connection{data[4]}-{data[5]}";
                }
                e.Components.Add(component);
            }
            
            return e;
        }
    }
}
