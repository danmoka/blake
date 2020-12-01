using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blake512;
using System.Collections;
using System.Linq;
using System.Text;

namespace BlakeHashTests
{
    [TestClass]
    public class HashTests
    {
        private string _hashNIST = "1001101011";
        private string _hashNISTBlockTest = "0110011010";
        private string _hashNISTBinaryMatrixTest = "01011001001010101101";
        private string _hashNISTOverlappingTemplateTest = "10111011110010110100011100101110111110000101101001";
        private string _testString = "Это тестовая строка на русском языке!";
        private byte[] _testData;
        private byte[] _hash;

        public HashTests()
        {
            _testData = Encoding.ASCII.GetBytes(_testString);
            var blake512 = new Blake512Algorithm();
            _hash = blake512.ComputeHash(_testData);
        }

        [TestMethod]
        public void FrequencyTest()
        {
            var pvFreq = new FrequencyAlg().getPValue(new BitArray(_hash));

            Assert.IsTrue(pvFreq >= 0.01);
        }

        [TestMethod]
        public void RunsTest()
        {
            var pvFreq = new RunsAlg().getPValue(new BitArray(_hash));

            Assert.IsTrue(pvFreq >= 0.01);
        }

        [TestMethod]
        public void BlocksTest()
        {
            var pvFreq = new FrequencyBlockAlg(3).getPValue(new BitArray(_hash));

            Assert.IsTrue(pvFreq >= 0.01);
        }

        [TestMethod]
        public void RankTest()
        {
            var pvFreq = new BinaryMatrixRankAlg(3, 3).getPValue(new BitArray(_hash));

            Assert.IsTrue(pvFreq >= 0.01);
        }

        [TestMethod]
        public void OverlappingTemplateTest()
        {
            var pvFreq = new OverlappingTemplateMatchingAlg(ConvertStringToBitArray("1111"), 32)
                .getPValue(new BitArray(_hash));

            Assert.IsTrue(pvFreq >= 0.01);
        }

        private BitArray ConvertStringToBitArray(string str)
        {
            return new BitArray(str.Select(s => s == '1').ToArray());
        }
    }
}
