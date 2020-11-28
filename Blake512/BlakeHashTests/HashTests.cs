using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blake512;
using System.Collections;
using System.Linq;

namespace BlakeHashTests
{
    [TestClass]
    public class HashTests
    {
        private BitArray _hashNIST = new BitArray(new bool[] 
        {
            true, false, false, true, true, false,true, false, true, true
        });
        private BitArray _hashNISTBlock = new BitArray(new bool[]
        {
            false, true, true, false, false, true, true, false, true, false
        });
        private BitArray _hashNISTBinary = new BitArray(new bool[]
            {
                false, true, false, 
                true, true, false, 
                false,true, false, 
                false, true, false, 
                true, false, true, 
                false, true, true, 
                false, true
            }); 

        [TestMethod]
        public void FrequencyTest()
        {
            var blake512 = new Blake512Algorithm();
            var data = new byte[0];
            var hash = blake512.ComputeHash(data);
            var bits = new BitArray(hash);
            var pvFreq = new FrequencyAlg().getPValue(bits);

            Assert.IsTrue(pvFreq >= 0.01);
        }

        [TestMethod]
        public void RunsTest()
        {
            var blake512 = new Blake512Algorithm();
            var data = new byte[4] { 128, 128, 128, 128 };
            var hash = blake512.ComputeHash(data);
            var bits = new BitArray(hash);
            var pvFreq = new RunsAlg().getPValue(bits);

            Assert.IsTrue(pvFreq >= 0.01);
        }

        [TestMethod]
        public void BlocksTest()
        {
            var blake512 = new Blake512Algorithm();
            var data = new byte[4] { 128, 128, 128, 128 };
            var hash = blake512.ComputeHash(data);
            var bits = new BitArray(hash);
            var pvFreq = new FrequencyBlockAlg(3).getPValue(bits);

            Assert.IsTrue(pvFreq >= 0.01);
        }

        [TestMethod]
        public void RankTest()
        {
            var blake512 = new Blake512Algorithm();
            var data = new byte[4] { 128, 128, 128, 128 };
            var hash = blake512.ComputeHash(data);
            var bits = new BitArray(hash);
            var pvFreq = new BinaryMatrixRankAlg(3, 3).getPValue(bits);

            Assert.IsTrue(pvFreq >= 0.01);
        }

        [TestMethod]
        public void OverlappingTemplateTest()
        {
            var blake512 = new Blake512Algorithm();
            var data = new byte[4] { 128, 128, 128, 128 };
            var hash = blake512.ComputeHash(data);
            var bits = new BitArray(hash);
            var res = new BitArray(
                "10111011110010110100011100101110111110000101101001".Select(c => c == '1').ToArray());
            var pvFreq = new OverlappingTemplateMatchingAlg(
                new BitArray(new bool[] { true, true, true, true}), 32).getPValue(bits);

            Assert.IsTrue(pvFreq >= 0.01);
        }
    }
}
