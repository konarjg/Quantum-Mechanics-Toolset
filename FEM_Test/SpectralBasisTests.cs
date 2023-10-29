using FEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM_Test
{
    [TestClass]
    public class SpectralBasisTests
    {
        [TestMethod]
        public void TNLessThanZeroShouldThrowException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => SpectralBasis.T(-1, 0));
        }

        [TestMethod]
        public void UNLessThanZeroShouldThrowException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => SpectralBasis.U(-1, 0));
        }

        [TestMethod]
        public void TPrimeNLessThanZeroShouldThrowException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => SpectralBasis.TPrime(-1, 0));
        }

        [TestMethod]
        public void TBisLessThanZeroShouldThrowException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => SpectralBasis.TBis(-1, 0));
        }

        [TestMethod]
        public void T0ShouldReturnOne()
        {
            var result = SpectralBasis.T(0, 0);

            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void T1ShouldReturnX()
        {
            var result = SpectralBasis.T(1, 5);

            Assert.AreEqual(result, 5);
        }

        [TestMethod]
        public void T2ShouldReturnTwoXSqrMinusOne()
        {
            var result = SpectralBasis.T(2, 5);

            Assert.AreEqual(result, 49);
        }

        [TestMethod]
        public void U0ShouldReturnOne()
        {
            var result = SpectralBasis.U(0, 0);

            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void U1ShouldReturnTwoX()
        {
            var result = SpectralBasis.U(1, 5);

            Assert.AreEqual(result, 10);
        }

        [TestMethod]
        public void U2ShouldReturnFourXSqrMinusOne()
        {
            var result = SpectralBasis.U(2, 5);

            Assert.AreEqual(result, 99);
        }


        [TestMethod]
        public void T0PrimeShouldReturnZero()
        {
            var result = SpectralBasis.TPrime(0, 5);

            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void T1PrimeShouldReturnOne()
        {
            var result = SpectralBasis.TPrime(1, 5);

            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void T2PrimeShouldReturnFourX()
        {
            var result = SpectralBasis.TPrime(2, 5);

            Assert.AreEqual(result, 20);
        }

        [TestMethod]
        public void T0BisShouldReturnZero()
        {
            var result = SpectralBasis.TBis(0, 5);

            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void T1BisShouldReturnZero()
        {
            var result = SpectralBasis.TBis(1, 5);

            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void T2PrimeShouldReturnFour()
        {
            var result = SpectralBasis.TBis(2, 5);

            Assert.AreEqual(result, 4);
        }
    }
}
