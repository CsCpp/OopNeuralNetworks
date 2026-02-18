using System;
using System.Collections.Generic;
using System.Linq;

namespace MNeuralNetworks
{
    public class NeuralNetwork
    {
        public Topology Topology { get; }
        public List<Layer> Layers { get; }
        public NeuralNetwork(Topology topology)
        {
            Topology = topology;
            Layers = new List<Layer>();
            CreateInputLayrs();
            CreateHiddenLayers();
            CreateOutputLayers();

        }

        private void CreateOutputLayers()
        {
            var outputNeuron = new List<Neuron>();
            var lastLayer = Layers.Last();
            for (int i = 0; i < Topology.OutputCount; i++)
            {
                var neuron = new Neuron(lastLayer.Count, NeuronType.Output);
                outputNeuron.Add(neuron);
            }
            var outputLayer = new Layer(outputNeuron, NeuronType.Output);
            Layers.Add(outputLayer);
        }

        private void CreateHiddenLayers()
        {
            for (int j = 0; j < Topology.HidenLayers.Count; j++)
            {
                var hiddenNeuron = new List<Neuron>();
                var lastLayer = Layers.Last();
                for (int i = 0; i < Topology.HidenLayers[j]; i++)
                {
                    var neuron = new Neuron(lastLayer.Count);
                    hiddenNeuron.Add(neuron);
                }
                var hiddenLayer = new Layer(hiddenNeuron);
                Layers.Add(hiddenLayer);
            }
        }
        public Neuron FeedForward(List<double> inputSignal)
        {
            SetSignalsToInputNeurons(inputSignal);
            FeedForwardAllLayersAfterInput(inputSignal);
            if (Topology.OutputCount == 1)
            {
                return Layers.Last().Neurons[0];
            }
            else
            {
                return Layers.Last().Neurons.OrderByDescending(n => n.Output).First();
            }
        }

        private void FeedForwardAllLayersAfterInput(List<double> inputSignal)
        {
            for (int i = 1; i < Layers.Count; i++)
            {
                var layer = Layers[i];
                var previousLaersSignals = Layers[i - 1].GetSignals();
                foreach (var neuron in layer.Neurons)
                {
                    neuron.FeedForward(previousLaersSignals);
                }
            }
        }


        private void SetSignalsToInputNeurons(List<double> inputSignal)
        {
            for (int i = 0; i < inputSignal.Count; i++)
            {
                var signal = new List<double>() { inputSignal[i] };
                var neuron = Layers[0].Neurons[i];
                neuron.FeedForward(signal);

            }
        }

        private void CreateInputLayrs()
        {
            var inputsNeuron = new List<Neuron>();
            for (int i = 0; i < Topology.InputCount; i++)
            {
                var neuron = new Neuron(1, NeuronType.Input);
                inputsNeuron.Add(neuron);
            }
            var inputLayer = new Layer(inputsNeuron, NeuronType.Input);
            Layers.Add(inputLayer);
        }
    }
}
