using FEM;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Complex = System.Numerics.Complex;

namespace FEM_Test
{
    [TestClass]
    public class MathUtilsTests
    {
        [TestMethod]
        public void IntegrateBoundsOutsideDomainShouldThrowException()
        {
            var n = 1000;
            var domain = new double[] { 0, 5 };
            var dx = (domain[1] - domain[0]) / (n - 1);
            var function = CreateVector.Random<Complex>(n);

            var a = -1;
            var b = 6;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => MathUtils.Integrate(function, domain, a, b, dx));
        }

        [TestMethod]
        public void IntegrateSineShouldEqualZeroOverDomain()
        {
            var n = 1000;
            var domain = new double[] { 0, 2 * Math.PI };
            var dx = (domain[1] - domain[0]) / (n - 1);
            var function = CreateVector.Random<Complex>(n);

            for (int i = 0; i < n; ++i)
                function[i] = Complex.Sin(domain[0] + i * dx);

            var a = 0;
            var b = 2 * Math.PI;

            var result = MathUtils.Integrate(function, domain, a, b, dx);

            Assert.IsTrue(result.AlmostEqual(0, 1e-6));
        }

        [TestMethod]
        public void Integrate2XBetween1And2ShouldEqual3()
        {
            var n = 1000;
            var domain = new double[] { -10, 10 };
            var dx = (domain[1] - domain[0]) / (n - 1);
            var function = CreateVector.Random<Complex>(n);

            for (int i = 0; i < n; ++i)
                function[i] = 2 * (domain[0] + i * dx);

            var a = 1;
            var b = 2;

            var result = MathUtils.Integrate(function, domain, a, b, dx);

            Assert.IsTrue(result.AlmostEqual(3, 1e-6));
        }

        [TestMethod]
        public void IntegrateComplexExponentialShouldEqual2IOver0AndPI()
        {
            var n = 1000;
            var domain = new double[] { 0, 2 * Math.PI };
            var dx = (domain[1] - domain[0]) / (n - 1);
            var function = CreateVector.Random<Complex>(n);

            for (int i = 0; i < n; ++i)
                function[i] = Complex.Exp(Complex.ImaginaryOne * (domain[0] + i * dx));

            var a = 0;
            var b = Math.PI;

            var result = MathUtils.Integrate(function, domain, a, b, dx);

            Assert.IsTrue(result.AlmostEqual(2 * Complex.ImaginaryOne, 1e-6));
        }

        [TestMethod]
        public void DoubleIntegrateBoundsOutsideDomainShouldThrowException()
        {
            var n = 1000;
            var domain = new double[,] { { 0, 5 }, { 0, 5 } };
            var dx = (domain[0, 1] - domain[0, 0]) / (n - 1);
            var dy = (domain[1, 1] - domain[1, 0]) / (n - 1);
            var function = CreateMatrix.Sparse<Complex>(n, n);

            var a = -1;
            var b = 8;
            var c = -1;
            var d = 7;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => MathUtils.DoubleIntegrate(function, domain, a, b, c, d, dx, dy));
        }

        [TestMethod]
        public void DoubleIntegrateSineXPlusSineYOverDomainShouldEqualZero()
        {
            var n = 1000;
            var domain = new double[,] { { 0, 2 * Math.PI }, { 0, 2 * Math.PI } };
            var dx = (domain[0, 1] - domain[0, 0]) / (n - 1);
            var dy = (domain[1, 1] - domain[1, 0]) / (n - 1);
            var function = CreateMatrix.Sparse<Complex>(n, n);

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    var x = domain[0, 0] + i * dx;
                    var y = domain[1, 0] + j * dy;

                    function[i, j] = Complex.Sin(x) + Complex.Sin(y);
                }
            }

            var a = 0;
            var b = 2 * Math.PI;
            var c = 0;
            var d = 2 * Math.PI;

            var result = MathUtils.DoubleIntegrate(function, domain, a, b, c, d, dx, dy);
            Assert.IsTrue(result.AlmostEqual(0, 1e-6));
        }

        [TestMethod]
        public void DoubleIntegrateSineXPlusYOver0PIForXYShouldEqualZero()
        {
            var n = 500;
            var domain = new double[,] { { 0, 2 * Math.PI }, { 0, 2 * Math.PI } };
            var dx = (domain[0, 1] - domain[0, 0]) / (n - 1);
            var dy = (domain[1, 1] - domain[1, 0]) / (n - 1);
            var function = CreateMatrix.Sparse<Complex>(n, n);

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    var x = domain[0, 0] + i * dx;
                    var y = domain[1, 0] + j * dy;

                    function[i, j] = Complex.Sin(x + y);
                }
            }

            var a = 0;
            var b = Math.PI;
            var c = 0;
            var d = Math.PI;

            var result = MathUtils.DoubleIntegrate(function, domain, a, b, c, d, dx, dy);
            Assert.IsTrue(result.AlmostEqual(0, 1e-6));
        }

        [TestMethod]
        public void DoubleIntegrateExpXPlusYOver02ForXAnd01ForYShouldEqualEMinusOneTimesESqrMinusOne()
        {
            var n = 500;
            var domain = new double[,] { { 0, 2 * Math.PI }, { 0, 2 * Math.PI } };
            var dx = (domain[0, 1] - domain[0, 0]) / (n - 1);
            var dy = (domain[1, 1] - domain[1, 0]) / (n - 1);
            var function = CreateMatrix.Sparse<Complex>(n, n);

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    var x = domain[0, 0] + i * dx;
                    var y = domain[1, 0] + j * dy;

                    function[i, j] = Complex.Exp(x + y);
                }
            }

            var a = 0;
            var b = 2;
            var c = 0;
            var d = 1;

            var result = MathUtils.DoubleIntegrate(function, domain, a, b, c, d, dx, dy);
            Assert.IsTrue(result.AlmostEqual((Math.E - 1) * (Math.E * Math.E - 1), 1e-6));
        }

        [TestMethod]
        public void DoubleIntegrateRSqrOverUnitCircleInPolarShouldEqualHalfPI()
        {
            var n = 500;
            var domain = new double[,] { { 0, 2 * Math.PI }, { 0, 2 * Math.PI } };
            var dx = (domain[0, 1] - domain[0, 0]) / (n - 1);
            var dy = (domain[1, 1] - domain[1, 0]) / (n - 1);
            var function = CreateMatrix.Sparse<Complex>(n, n);

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    var x = domain[0, 0] + i * dx;
                    var y = domain[1, 0] + j * dy;

                    function[i, j] = x * x;
                }
            }

            var a = 0;
            var b = 1;
            var c = 0;
            var d = 2 * Math.PI;

            var result = MathUtils.DoubleIntegrate(function, domain, a, b, c, d, dx, dy, true);
            Assert.IsTrue(result.AlmostEqual(Math.PI / 2, 1e-6));
        }
    }
}