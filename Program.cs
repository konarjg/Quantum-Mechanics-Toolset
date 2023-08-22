const double E = 66.2;
const double Q = 6.62;
const double k = 0.662;
const double w = 0.0662;

double a(uint n)
{
    if (n == 1)
        return 10;
    else if (n == 2)
        return 5;

    return 1 / (k * n * n) * (E * a(n - 2) + Q * a(n - 1) + w);
}

double[] f(double r)
{
    double y = a(1) + r * a(2);

    for (uint n = 3; n <= 7; ++n)
        y += a(n) * Math.Pow(r, n);

    return new double[2] { y, y };
}

double[] g(double fi)
{
    var y = new double[2];

    y[0] = Math.Sqrt(7 / (2 * Math.PI)) * Math.Cos(w * fi);
    y[1] = Math.Sqrt(7 / (2 * Math.PI)) * Math.Sin(w * fi);

    return y;
}

double[] psi(double r, double fi)
{
    var R = f(r);
    var FI = g(fi);

    var y = new double[2];
    y[0] = R[0] * FI[0];
    y[1] = R[1] * FI[1];

    return y;
}

