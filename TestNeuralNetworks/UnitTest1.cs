using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeuralNetworks;
using System;
using System.Collections.Generic;

namespace TestNeuralNetworks
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FeedForwardTest()
        {
            var topology = new Topology(4, 1, 2);
            var neuralNetworks = new NeuralNetworks1(topology);
            neuralNetworks.Layers[1].Neurons[0].SetWeights(0.5, -0.1, 0.3, -0.1);
            neuralNetworks.Layers[1].Neurons[1].SetWeights(0.1, -0.3, 0.7, -0.3);
            neuralNetworks.Layers[2].Neurons[0].SetWeights(1.2, 0.8);
            var result = neuralNetworks.FeedForward(new List<double> { 1, 0, 0, 0 });
        }
    }
}
