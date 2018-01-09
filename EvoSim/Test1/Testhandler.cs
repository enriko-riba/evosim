using System;

namespace EvoSim.Test1
{
    class Testhandler : IEntityHandler
    {
        const int NODE_KIND = 0;
        const int CONNECTION_KIND = 1;
        
        private Random r = new Random();

        public SimEntity CreateEntity(string genome)
        {
            var e = new SimEntity();

            int nodeCounter = 0;
            var components = genome.Split('|');
            foreach (var c in components)
            {
                var component = new SimComponent();
                var data = c.Split(';');
                if (data[0] == "N")
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

        public SimEntity CreateEntity()
        {
            var entity = new SimEntity();
            

            //  generate 2-4 nodes
            //  generate connections, max connections = (n*(n-1))/2
            var n = r.Next(2, 5);
            var c = (n * (n - 1)) / 2;

            entity.Name = $"N{n}C{c}";
            for (int x = 0; x < n; x++)
            {
                var comp = new SimComponent()
                {
                    Kind = NODE_KIND,
                    Name = $"Node{x + 1}",
                    Attributes = new float[] { RandomFriction() } //  range 0.5 - 1.5
                };
                entity.Components.Add(comp);
            }

            for (int x = 0; x < c; x++)
            {
                var comp = new SimComponent()
                {
                    Kind = CONNECTION_KIND,
                    Name = $"Connection{x + 1}",
                    Attributes = new float[3]
                    {
                            RandomLength(),    
                            RandomContraction(),
                            RandomPeriod(),    
                    }
                };
                entity.Components.Add(comp);
            }

            return entity;
        }

        public float EvaluateFitness(SimEntity e)
        {
            return (float)r.NextDouble();
        }

        public SimEntity CloneEntity(SimEntity e)
        {
            var entity = new SimEntity();
            entity.Name = e.Name;
            entity.Generation = e.Generation;
            entity.Species = e.Species;
            entity.Species = e.Species;
            entity.CustomDataArr = e.CustomDataArr;

            foreach (var c in e.Components)
            {
                var clone = SimComponent.Clone(c);
                entity.Components.Add(clone);
            }
            return entity;
        }

        public SimEntity MutateEntity(SimEntity e)
        {
            var entity = CloneEntity(e);

            const double pNodeCnt = 0.1;
            const double pConnCnt = 0.2;
            const double pAtr = 1.0;

            //  Change node count
            var d = r.NextDouble();
            if (d < pNodeCnt)
            {
                //  TODO
                return entity;
            }

            //  Change connection count
            d = r.NextDouble();
            if(d < pConnCnt)
            {
                //  TODO
                return entity;
            }


            //  Change component
            var i = r.Next(0, entity.Components.Count);
            var component = entity.Components[i];
            
            //  mutation range depends on kind
            if(component.Kind == NODE_KIND)
            {
                component.Attributes[0] = RandomFriction();
            }
            else
            {
                i = r.Next(0, component.Attributes.Length);
                if (i == 0)
                    component.Attributes[0] = RandomLength();
                else if(i == 1)
                    component.Attributes[1] = RandomContraction();
                else
                    component.Attributes[2] = RandomPeriod();
            }
            return entity;
        }

        private float RandomFriction()
        {
            var f = r.Next(500, 1500) / 1000.0f; //  range 0.5 - 1.5
            return f;
        }
        private float RandomLength()
        {
            var f = r.Next(100, 500) / 100f;    //  range 1 - 5
            return f;
        }
        private float RandomContraction()
        {
            var f = r.Next(300, 900) / 1000.0f;   //  range 0.3 - 0.9
            return f;
        }
        private float RandomPeriod()
        {
            var f = r.Next(10, 50)/10f;    //  range 1 - 5
            return f;
        }
    }
}
