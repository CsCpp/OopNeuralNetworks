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
                var neuron = new Neuron(lastLayer.NeuronCount, NeuronType.Output);
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
                    var neuron = new Neuron(lastLayer.NeuronCount);
                    hiddenNeuron.Add(neuron);
                }
                var hiddenLayer = new Layer(hiddenNeuron);
                Layers.Add(hiddenLayer);
            }
        }
        public Neuron FeedForward(params double[] inputSignal)
        {
            SetSignalsToInputNeurons(inputSignal);
            FeedForwardAllLayersAfterInput();
            if (Topology.OutputCount == 1)
            {
                return Layers.Last().Neurons[0];
            }
            else
            {
                return Layers.Last().Neurons.OrderByDescending(n => n.Output).First();
            }
        }
        public double Learn(List<Tuple<double, double[]>> dataset, int epoch)
        {
            var error = 0.0;
            for (int i = 0; i < epoch; i++)
            {
                foreach (var data in dataset)
                {
                    error += BackPropagation(data.Item1, data.Item2);
                }
            }
            var result = error / epoch;
            return result;
        }
        private double BackPropagation(double expected, params double[] inputs)
        {
            var actual = FeedForward(inputs).Output;
            var difference = actual - expected;
            foreach (var neuron in Layers.Last().Neurons)
            {
                neuron.Learn(difference, Topology.LearningRate);
            }
            for (int j = Layers.Count - 2; j >= 0; j--)
            {
                var layer = Layers[j];
                var previousLayer = Layers[j + 1];
                for (int i = 0; i < layer.NeuronCount; i++)
                {
                    var neuron = layer.Neurons[i];
                    for (int k = 0; k < previousLayer.NeuronCount; k++)
                    {
                        var previousNeuron = previousLayer.Neurons[k];
                        var error = previousNeuron.Weights[i] * previousNeuron.Delta;
                        neuron.Learn(error, Topology.LearningRate);
                    }
                }
            }
            var result = difference * difference;
            return result;
        }
        private void FeedForwardAllLayersAfterInput()
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


        private void SetSignalsToInputNeurons(params double[] inputSignal)
        {
            for (int i = 0; i < inputSignal.Length; i++)
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
