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
        /// <summary>
        /// Обучение нейронки
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="epoch"></param>
        /// <returns></returns>
        public double Learn(double[] expected, double[,] inputs, int epoch)
        {
            var error = 0.0;
            for (int i = 0; i < epoch; i++)
            {
                for (int j = 0; j < expected.Length; j++)
                {
                    var output = expected[j];
                    var input = GetRow(inputs, j);
                    error += BackPropagation(output, input);
                }
            }
            var result = error / epoch;
            return result;
        }
        public static double[] GetRow(double[,] matrix, int row)
        {
            var colums = matrix.GetLength(1);
            var array = new double[colums];
            for (int i = 0; i < colums; i++)
            {
                array[i] = matrix[row, i];
            }
            return array;
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
        private double[,] Scalling(double[,] inputs)
        {
            var result = new double[inputs.GetLength(0), inputs.GetLength(1)];
            for (int column = 0; column < inputs.GetLength(1); column++)
            {
                var min = inputs[0, column];
                var max = inputs[0, column];
                for (int row = 1; row < inputs.GetLength(0); row++)
                {
                    var item = inputs[row, column];
                    if (item > max)
                    {
                        max = item;
                    }
                    if (item < min)
                    {
                        min = item;
                    }
                }
                var divider = max - min;
                for (int row = 0; row < inputs.GetLength(0); row++)
                {
                    result[row, column] = (inputs[row, column] - min) / divider;
                }
            }
            return result;
        }
        private double[,] Normalization(double[,] inputs)
        {
            var result = new double[inputs.GetLength(0), inputs.GetLength(1)];
            for (int column = 0; column < inputs.GetLength(1); column++)
            {
                var sum = 0.0;
                for (int row = 0; row < inputs.GetLength(0); row++)
                {
                    sum += inputs[row, column];
                }
                var average = sum / inputs.GetLength(0);
                var error = 0.0;
                for (int row = 0; row < inputs.GetLength(0); row++)
                {
                    error += Math.Pow((inputs[row, column] - average), 2);
                }
                var standartError = Math.Sqrt(error / inputs.GetLength(0));
                for (int row = 0; row < inputs.GetLength(0); row++)
                {
                    result[row, column] = (inputs[row, column] - average) / standartError;
                }
            }
            return result;
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
