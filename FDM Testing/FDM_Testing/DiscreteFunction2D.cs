using MathNet.Numerics.Integration;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.DE_Solver
{
    public class DiscreteFunction2D
    {
        private Func<double, double, double> Function;

        public DiscreteFunction2D(Func<double, double, double> function)
        {
            Function = function;
        }

        public Func<double, double, double> GetHandle()
        {
            return Function;
        }

        private DiscreteFunction2D ToPolarCoordinates(DiscreteFunction2D f, bool includeJacobian = false)
        {
            var g = new Func<double, double, double>((r, fi) =>
            {
                var value = f.Evaluate(r * Math.Cos(fi), r * Math.Sin(fi));

                if (includeJacobian)
                    return r * value;

                return value;
            });

            return new DiscreteFunction2D(g);
        }

        private DiscreteFunction2D ToCartesianCoordinates(DiscreteFunction2D f)
        {
            var g = new Func<double, double, double>((x, y) =>
            {
                var r = Math.Sqrt(x * x + y * y);
                var fi = Math.Atan2(y, x);

                var value = f.Evaluate(r, fi);

                return value;
            });

            return new DiscreteFunction2D(g);
        }

        public double Evaluate(double x, double y)
        {
            return MathUtils.Round(Function.Invoke(x, y));
        }

        public double IntegratePolar(double a, double b, double c, double d)
        {
            return ToPolarCoordinates(this, true).Integrate(a, b, c, d);
        }

        public double Integrate(double a, double b, double c, double d, bool jacobian = false)
        {
            if (jacobian)
            {
                var f = new Func<double, double, double>((x, y) => x * Function(x, y));
                return MathUtils.Round(GaussLegendreRule.Integrate(f, a, b, c, d, 10));
            }

            return MathUtils.Round(GaussLegendreRule.Integrate(Function, a, b, c, d, 10));
        }
    }

}
