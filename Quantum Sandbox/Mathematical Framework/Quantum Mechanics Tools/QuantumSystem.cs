using Quantum_Mechanics.Quantum_Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;

namespace Quantum_Sandbox.Mathematical_Framework.Quantum_Mechanics_Tools
{
    public enum Scenario
    {
        INFINITE_RECTANGULAR_WELL,
        HARMONIC_OSCILLATOR,
        FINITE_RECTANGULAR_WELL,
        DELTA_POTENTIAL,
        HYDROGEN_ATOM
    }
    
    public class QuantumSystem
    {
        public static Scenario ParseScenario(string scenario)
        {
            switch (scenario)
            {
                case "Infinite rectangular well":
                    return Scenario.INFINITE_RECTANGULAR_WELL;

                case "Harmonic oscillator":
                    return Scenario.HARMONIC_OSCILLATOR;

                case "Finite rectangular well":
                    return Scenario.FINITE_RECTANGULAR_WELL;

                case "Delta potential":
                    return Scenario.DELTA_POTENTIAL;

                case "Hydrogen atom":
                    return Scenario.HYDROGEN_ATOM;
            }

            throw new ArgumentException();
        }
    }
}
