namespace EvoSim
{
    public class SimComponent
    {
        public int Kind { get; set; }
        public float[] Attributes{ get; set; }
        public string Name { get; set; }

        public int[] CustomData { get; set; }

        public override string ToString() => Name;

        static public SimComponent Clone(SimComponent c)
        {
            var clone = new SimComponent();
            clone.Kind = c.Kind;
            clone.Name = c.Name;
            clone.Attributes = (float[])c.Attributes.Clone(); 
            return clone;
        }
    }
}
