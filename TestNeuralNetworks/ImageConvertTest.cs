using Microsoft.VisualStudio.TestTools.UnitTesting;
using MNeuralNetworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestNeuralNetworks
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void ConvertTest()
        {
            var converter = new PictureConverter();

            var inputs = converter.Convert(@"C:\\Users\\RZA\\source\\repos\\OopNeuralNetworks\\TestNeuralNetworks\\Images\\Parasitized.png");
            converter.Save("c:\\image.png", inputs);
        }
    }
}
