
using System.Collections.Generic;

namespace NeuralNetworks
{
    public class Topology
    {
        public int InputCount { get; }
        public int OutputCount { get; }
        public List<int> HidenLayers { get; }
        public Topology(int inputCount, int outputCount, params int[] layers)
        {
            InputCount = inputCount;
            OutputCount = outputCount;
            HidenLayers = new List<int>();
            HidenLayers.AddRange(layers);
        }
    }
}
