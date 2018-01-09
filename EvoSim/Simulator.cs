using System;
using System.Collections.Generic;
using System.Text;

namespace EvoSim
{
    class Simulator
    {
        private List<Entity> entities = new List<Entity>();
        private Random r = new Random();

        private int generation = 0;
        private int entityCount = 1000;

        public void InitPopulation()
        {
            for(int i =0; i< entityCount; i++)
            {
                var entity = new Entity();
                this.entities.Add(entity);

                //  generate 2-4 nodes
                //  generate connections, max connections = (n*(n-1))/2
                var n = r.Next(2, 5);
                var c = (n * (n - 1)) / 2;

                entity.Name = $"N{n}C{c}";
                for (int x = 0; x < n; x++)
                {
                    var comp = new Component()
                    {
                        Kind = 1,
                        Name = $"Node{x + 1}",
                        Attributes = new float[] { r.Next(50, 150)/100.0f } //  range 0.5 - 1.5
                    };
                    entity.Components.Add(comp);
                }

                for (int x = 0; x < c; x++)
                {
                    var comp = new Component()
                    {
                        Kind = 1,
                        Name = $"Connection{x + 1}",
                        Attributes = new float[3] 
                        {
                            (float)r.Next(1, 5),    //  length 1-5
                            r.Next(3, 9)/10.0f,     //  contraction 0.3 - 0.9
                            (float)r.Next(1, 5),    //  period 1s - 5s
                        } 
                    };
                    entity.Components.Add(comp);                    
                }
            }
        }
    }
}
