using Quantum_Mechanics.DE_Solver;
using Quantum_Mechanics.Quantum_Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Sandbox.Mathematical_Framework.Quantum_Mechanics_Tools
{
    public enum CoordinateSystem
    {
        CARTESIAN_1D,
        CARTESIAN_2D,
        POLAR
    }

    public enum MovementConstraints
    {
        FREE,
        POTENTIAL_BARRIER
    }

    public enum PotentialType
    {
        NONE,
        HARMONIC,
        ELECTRIC
    }
    
    public class QuantumSystem
    {
        public static string PotentialFunction(CoordinateSystem system, PotentialType type)
        {
            switch (system)
            {
                case CoordinateSystem.CARTESIAN_1D:
                    switch (type)
                    {
                        case PotentialType.NONE:
                            return "0";

                        case PotentialType.HARMONIC:
                            return 0.5 * QuantumConstants.Me + "*x^2";

                        case PotentialType.ELECTRIC:
                            return "(0-1)/(x+0,0001)";
                    }

                    break;

                case CoordinateSystem.CARTESIAN_2D:
                    switch (type)
                    {
                        case PotentialType.NONE:
                            return "0";

                        case PotentialType.HARMONIC:
                            return 0.5 * QuantumConstants.Me + "*(x^2+y^2)";

                        case PotentialType.ELECTRIC:
                            return "(0-1)/(sqrt(x*x+y*y)+0,0001)";
                    }

                    break;

                case CoordinateSystem.POLAR:
                    switch (type)
                    {
                        case PotentialType.NONE:
                            return "0";

                        case PotentialType.HARMONIC:
                            return 0.5 * QuantumConstants.Me + "*x^2";

                        case PotentialType.ELECTRIC:
                            return "(0-1)/(x+0,0001)";
                    }

                    break;
            }

            throw new ArgumentException();
        }

        public static CoordinateSystem ParseCoordinateSystem(string text)
        {
            switch (text)
            {
                case "Cartesian 1D":
                    return CoordinateSystem.CARTESIAN_1D;

                case "Cartesian 2D":
                    return CoordinateSystem.CARTESIAN_2D;

                case "POLAR":
                    return CoordinateSystem.POLAR;
            }

            throw new ArgumentException();
        }

        public static PotentialType ParsePotentialType(string text)
        {
            switch (text)
            {
                case "None":
                    return PotentialType.NONE;

                case "Harmonic":
                    return PotentialType.HARMONIC;

                case "Electric":
                    return PotentialType.ELECTRIC;
            }

            throw new ArgumentException();
        }

        public static MovementConstraints ParseMovementConstraints(string text)
        {
            switch (text)
            {
                case "Free Movement":
                    return MovementConstraints.FREE;

                case "Potential Barriers":
                    return MovementConstraints.POTENTIAL_BARRIER;
            }

            throw new ArgumentException();
        }
    }
}
