using System;
using System.Linq;

namespace EvoSim.Test1
{
    class Testhandler : IEntityHandler
    {
        const int NODE_KIND = 0;
        const int CONNECTION_KIND = 1;
        const int MIN_NODES = 2;
        const int MAX_NODES = 4;
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

            //  generate random node count
            var nodeCount = r.Next(MIN_NODES, MAX_NODES + 1);

            //  max connections = n * (n-1) / 2
            var maxConnections = (nodeCount * (nodeCount - 1)) / 2;
            var connectionCount = r.Next(nodeCount - 1, maxConnections + 1);

            entity.Name = $"N{nodeCount}C{connectionCount}";
            for (int n = 0; n < nodeCount; n++)
            {
                var comp = new SimComponent()
                {
                    Kind = NODE_KIND,
                    Name = $"Node{n}",
                    Attributes = new float[] { RandomFriction() } //  range 0.5 - 1.5
                };
                entity.Components.Add(comp);
            }

            int connectionCounter = 0;
            for (int n = 0; n < nodeCount - 1; n++)
            {
                for (int i = n + 1; i < nodeCount; i++)
                {
                    var comp = new SimComponent()
                    {
                        Kind = CONNECTION_KIND,
                        Name = $"{n}-{i}",
                        Attributes = new float[3]
                                        {
                            RandomLength(),
                            RandomContraction(),
                            RandomPeriod(),
                                        }
                    };
                    comp.CustomData = new int[2] { n, i };
                    entity.Components.Add(comp);

                    if (++connectionCounter >= connectionCount)
                    {
                        //  force both loops to exit
                        i = nodeCount;
                        n = nodeCount;
                    }
                }
            }

            return entity;
        }

        public float EvaluateFitness(SimEntity e)
        {
            //return (float)r.NextDouble();
            return e.Components.SelectMany(c => c.Attributes).Sum();
        }

        public SimEntity CloneEntity(SimEntity e)
        {
            var entity = new SimEntity();
            entity.Name = e.Name;
            entity.Generation = e.Generation;
            entity.Species = e.Species;
            entity.Species = e.Species;
            entity.CustomData = e.CustomData;

            foreach (var c in e.Components)
            {
                var clone = SimComponent.Clone(c);
                entity.Components.Add(clone);
            }
            return entity;
        }

        public SimEntity MutateEntity(SimEntity e)
        {
            const float PROBABILITY_NODE_COUNT_CHANGE = 0.1f;
            const float PROBABILITY_CONN_COUNT_CHANGE = 0.2f;

            SimEntity mutation;
            if (IsEvent(PROBABILITY_NODE_COUNT_CHANGE) && MutateNodeCount(e, out mutation))
            {
                return mutation;
            }

            if (IsEvent(PROBABILITY_CONN_COUNT_CHANGE) && MutateConnectionCount(e, out mutation))
            {
                return mutation;
            }

            //  more common case: mutate a single attribute
            mutation = MutateAttribute(e);
            return mutation;
        }

        #region Mutation variations
        private SimEntity MutateAttribute(SimEntity e)
        {
            var mutation = CloneEntity(e);

            //  Change component
            var i = r.Next(0, mutation.Components.Count);
            var component = mutation.Components[i];
            var delta = ((float)r.NextDouble() - 0.5f) / 5f;//   value in range [-.1, 0.1]

            //  mutation range depends on kind
            if (component.Kind == NODE_KIND)
            {
                component.Attributes[0] += delta;
            }
            else
            {
                i = r.Next(0, component.Attributes.Length);
                if (i == 0)
                    component.Attributes[0] += delta;
                else if (i == 1)
                    component.Attributes[1] += delta;
                else
                    component.Attributes[2] += delta;
            }
            return mutation;
        }

        private bool MutateNodeCount(SimEntity e, out SimEntity mutation)
        {
            mutation = CloneEntity(e);
            //  TODO: change node count
            return false;
        }
        private bool MutateConnectionCount(SimEntity e, out SimEntity mutation)
        {
            mutation = CloneEntity(e);
            var nodes = mutation.Components.Where(m => m.Kind == NODE_KIND);
            var connections = mutation.Components.Where(m => m.Kind == CONNECTION_KIND);
            var nCount = nodes.Count();

            if (IsEvent(0.5f))
            {
                //  do not allow going bellow minimum
                var minConn = nCount - 1;
                if (connections.Count() > minConn)
                {
                    //  TODO: decrease count
                    return false;
                }
            }
            else
            {
                //  TODO: add a new connection
                return false;
            }
            return false;
        }
        #endregion

        public void SetupSimulation(SimEntity[] entities)
        {
            foreach (var e in entities)
            {
                var nodes = e.Components.Where(c => c.Kind == NODE_KIND).Select(c => c.Attributes[0]);
                var conns = e.Components
                                .Where(c => c.Kind == CONNECTION_KIND)
                                .Select(c => new
                                {
                                    N1 = c.CustomData[0],   // from node id
                                    N2 = c.CustomData[1],   // to node id
                                    L = c.Attributes[0],
                                    C = c.Attributes[1],
                                    P = c.Attributes[2],
                                });
            }
        }

        public void Simulate() { }

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
            var f = r.Next(100, 250) / 100f;    //  range 1 - 2.5
            return f;
        }

        private bool IsEvent(float percentage)
        {
            return r.NextDouble() < percentage;
        }
    }
}
