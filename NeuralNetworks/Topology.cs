using System.Collections.Generic;

namespace MNeuralNetworks
{
    public class Topology
    {
        public int InputCount { get; }
        public int OutputCount { get; }
        public double LearningRate { get; }
        public List<int> HidenLayers { get; }
        public Topology(int inputCount, int outputCount,double learningRate, params int[] layers)
        {
            InputCount = inputCount;
            OutputCount = outputCount;
            LearningRate = learningRate;
            HidenLayers = new List<int>();
            HidenLayers.AddRange(layers);
        }
    }
}
