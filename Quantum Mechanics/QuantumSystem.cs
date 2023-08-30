using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.Quantum_Mechanics
{
    public class QuantumSystem
    {
        public double Mass;
        public int EnergyLevel;
        public string PotentialFunction;
        public double[,] PositionDomain;

        public QuantumSystem(double mass, int energyLevel, string potentialFunction, double[,] positionDomain) 
        {
            Mass = mass;
            EnergyLevel = energyLevel;
            PotentialFunction = potentialFunction;
            PositionDomain = positionDomain;
        }
    }
}
