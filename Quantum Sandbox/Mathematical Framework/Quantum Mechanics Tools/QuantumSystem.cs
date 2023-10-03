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
        ELECTRIC,
        MAGNETIC,
        ELECTROMAGNETIC
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
                            return 0.5 * QuantumConstants.Me + "*100*x^2";

                        case PotentialType.ELECTRIC:
                            return "(0-100)/(x+0,0001)";

                        case PotentialType.MAGNETIC:
                            return "((0-100)*x^2)/(16*" + QuantumConstants.Me + "^2)";

                        case PotentialType.ELECTROMAGNETIC:
                            return PotentialFunction(system, PotentialType.ELECTRIC) + "+ (" + PotentialFunction(system, PotentialType.MAGNETIC) + ")";
                    }

                    break;

                case CoordinateSystem.CARTESIAN_2D:
                    switch (type)
                    {
                        case PotentialType.NONE:
                            return "0";

                        case PotentialType.HARMONIC:
                            return 0.5 * QuantumConstants.Me + "*100*(x^2+y^2)";

                        case PotentialType.ELECTRIC:
                            return "(0-100)/(sqrt(x^2 + y^2) + 0,0001)";

                        case PotentialType.MAGNETIC:
                            return "((0-100)*(x^2 + y^2))/(16*" + QuantumConstants.Me + "^2)";

                        case PotentialType.ELECTROMAGNETIC:
                            return PotentialFunction(system, PotentialType.ELECTRIC) + "+ (" + PotentialFunction(system, PotentialType.MAGNETIC) + ")";
                    }

                    break;

                case CoordinateSystem.POLAR:
                    switch (type)
                    {
                        case PotentialType.NONE:
                            return "0";

                        case PotentialType.HARMONIC:
                            return 0.5 * QuantumConstants.Me + "*100*x^2";

                        case PotentialType.ELECTRIC:
                            return "(0-100)/(x+0,0001)";

                        case PotentialType.MAGNETIC:
                            return "((0-100)*x^2)/(16*" + QuantumConstants.Me + "^2)";

                        case PotentialType.ELECTROMAGNETIC:
                            return PotentialFunction(system, PotentialType.ELECTRIC) + "+ (" + PotentialFunction(system, PotentialType.MAGNETIC) + ")";
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

                case "Polar":
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

                case "Magnetic":
                    return PotentialType.MAGNETIC;

                case "Electromagnetic":
                    return PotentialType.ELECTROMAGNETIC;
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
