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
            for (int i = 0; i < outputs.Length; i++)
            {
                var row = NeuralNetwork.GetRow(inputs, i);
                results.Add(neuralNetworks.Predict(row).Output);
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
                    var value = row.Split(',').Select(v => Convert.ToDouble(v.Replace(".", ","))).ToList();
                    var output = value.Last();
                    var input = value.Take(value.Count - 1).ToArray();
                    outputs.Add(output);
                    inputs.Add(input);
                }
                var inputSignal = new double[inputs.Count, inputs[0].Length];
                for (int i = 0; i < inputSignal.GetLength(0); i++)
                {
                    for (int j = 0; j < inputSignal.GetLength(1); j++)
                    {
                        inputSignal[i, j] = inputs[i][j];
                    }
                }
                var topology = new Topology(outputs.Count, 1, 0.1, outputs.Count / 2);
                var neuralNetworks = new NeuralNetwork(topology);
                var difference = neuralNetworks.Learn(outputs.ToArray(), inputSignal, 10);
                var results = new List<double>();

                for (int i = 0; i < outputs.Count; i++)
                {
                    results.Add(neuralNetworks.Predict(inputs[i]).Output);
                }
                for (int i = 0; i < results.Count; i++)
                {
                    var expected = Math.Round(outputs[i], 3);
                    var actual = Math.Round(results[i], 3);
                    Assert.AreEqual(expected, actual);
                }
            }
        }
        [TestMethod]
        public void RecognizeImage()
        {
            var size = 20;
            var parazitePath = (@"C:\\Image\Parazite");
            var unParazitePath = (@"C:\\Image\UnParazite");
            var converter = new PictureConverter();

            // var inputs = converter.Convert(@"OopNeuralNetworks\\TestNeuralNetworks\\Images\\Parasitized.png");
            var testParazitImage = converter.Convert(@"C:\\Users\\RZA\\source\\repos\\OopNeuralNetworks\\TestNeuralNetworks\\Images\\Parasitized.png");
            var testUnParazitImage = converter.Convert(@"C:\\Users\\RZA\\source\\repos\\OopNeuralNetworks\\TestNeuralNetworks\\Images\\UnParasitized.png");

            var topology = new Topology(testParazitImage.Count, 1, 0.1, testParazitImage.Count / 2);
            var neuralNetwork = new NeuralNetwork(topology);

            double[,] parazitInputs = GetData(parazitePath, converter, testParazitImage, size);
            neuralNetwork.Learn(new double[] { 1 }, parazitInputs, 3);

            double[,] unParazitInputs = GetData(unParazitePath, converter, testParazitImage, size);
            neuralNetwork.Learn(new double[] { 0 }, unParazitInputs, 3);

            var par = neuralNetwork.Predict(testParazitImage.Select(t => (double)t).ToArray());
            var unPar = neuralNetwork.Predict(testUnParazitImage.Select(t => (double)t).ToArray());

            Assert.AreEqual(1, Math.Round(par.Output, 2));
            Assert.AreEqual(0, Math.Round(unPar.Output, 2));
        }

        private static double[,] GetData(string parazitePath, PictureConverter converter, List<int> testImageInput, int size)
        {
            var images = Directory.GetFiles(parazitePath);
            var result = new double[size, testImageInput.Count];
            for (int i = 0; i < size; i++)
            {
                var image = converter.Convert(images[i]);
                for (int j = 0; j < image.Count; j++)
                {
                    result[i, j] = image[j];
                }
            }

            return result;
        }
    }
}
