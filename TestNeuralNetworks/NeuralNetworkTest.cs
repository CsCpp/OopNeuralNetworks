using Microsoft.VisualStudio.TestTools.UnitTesting;
using MNeuralNetworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestNeuralNetworks
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FeedForwardTest()
        {
            var outputs = new double[] { 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1 };
            var inputs = new double[,]
            {
                // Результат - Пациент болен - 1
                //             Пациент Здоров - 0

                // Неправильная температура T
                // Хороший возраст A
                // Курит S
                // Правильно питается F
                //                                             T  A  S  F
                { 0, 0, 0, 0 },
                { 0, 0, 0, 1 },
                { 0, 0, 1, 0 },
                { 0, 0, 1, 1 },
                { 0, 1, 0, 0 },
                { 0, 1, 0, 1 },
                { 0, 1, 1, 0 },
                { 0, 1, 1, 1 },
                { 1, 0, 0, 0 },
                { 1, 0, 0, 1 },
                { 1, 0, 1, 0 },
                { 1, 0, 1, 1 },
                { 1, 1, 0, 0 },
                { 1, 1, 0, 1 },
                { 1, 1, 1, 0 },
                { 1, 1, 1, 1 }
            };

            var topology = new Topology(4, 1, 0.1, 2);
            var neuralNetworks = new NeuralNetwork(topology);
            var difference = neuralNetworks.Learn(outputs, inputs, 100000);
            var results = new List<double>();
           for(int i=0; i<outputs.Length; i++)
            {
                var row = NeuralNetwork.GetRow(inputs, i);
                results.Add(neuralNetworks.FeedForward(row).Output);
            }
            for (int i = 0; i < results.Count; i++)
            {
                var expected = Math.Round(outputs[i], 3);
                var actual = Math.Round(results[i], 3);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void DatasetTest()
        {
            using (var sr = new StreamReader("heart.csv"))
            {
                var outputs = new List<double>();
                var inputs = new List<double[]>();
                var header = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    var row = sr.ReadLine();
                    var value = row.Split(',').Select(v => Convert.ToDouble(v.Replace(".",","))).ToList();
                    var output = value.Last();
                    var input = value.Take(value.Count - 1).ToArray();
                    outputs.Add(output);
                    inputs.Add(input);
                }
                var inputSignal = new double[inputs.Count, inputs[0].Length];
                for(int i=0; i<inputSignal.GetLength(0);i++)
                {
                    for(int j=0; j<inputSignal.GetLength(1); j++)
                    {
                        inputSignal[i, j] = inputs[i][j];
                    }
                }
                var topology = new Topology(outputs.Count, 1, 0.1, outputs.Count/2);
                var neuralNetworks = new NeuralNetwork(topology);
                var difference = neuralNetworks.Learn(outputs.ToArray(), inputSignal, 10);
                var results = new List<double>();

                for (int i = 0; i < outputs.Count; i++)
                {
                    results.Add(neuralNetworks.FeedForward(inputs[i]).Output);
                }
                for (int i = 0; i < results.Count; i++)
                {
                    var expected = Math.Round(outputs[i], 3);
                    var actual = Math.Round(results[i], 3);
                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }
}
