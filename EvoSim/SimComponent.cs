﻿namespace EvoSim
{
    public class SimComponent
    {
        public int Kind { get; set; }
        public float[] Attributes{ get; set; }
        public string Name { get; set; }

        static public SimComponent Clone(SimComponent c)
        {
            var clone = new SimComponent();
            clone.Kind = c.Kind;
            clone.Name = c.Name;
            clone.Attributes = (float[])c.Attributes.Clone(); //c.Attributes; // (float[])c.Attributes.Clone();
            return clone;
        }
    }
}
