using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.Quantum_Mechanics
{
    public static class QuantumConstants
    {
        //Reduced Planck's Constant H = h / 2pi
        public static double H = 1.054571817;
        //Mass of a single electron
        public static double Me = 0.0091093837;
        //Kinetic energy of a single electron
        public static double Te = -H * H / (2 * Me);
    }
}
