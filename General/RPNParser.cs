using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.General
{
    public static class RPNParser
    {
        private static Dictionary<string, int> Operators = new Dictionary<string, int>()
        {
            { "^", 2 },
            { "*", 1 },
            { "/", 1 },
            { "+", 0 },
            { "-", 0 },
        };

        private static List<string> Functions = new List<string>()
        {
            "exp",
            "log",
            "sin",
            "cos",
            "tan",
            "asin",
            "acos",
            "atan",
        };

        private static Complex32 ExecuteFunction(string function, Complex32 argument)
        {
            switch (function)
            {
                case "exp":
                    return Complex32.Exp(argument);

                case "log":
                    return Complex32.Log(argument);

                case "sin":
                    return Complex32.Sin(argument);

                case "cos":
                    return Complex32.Cos(argument);

                case "tan":
                    return Complex32.Tan(argument);

                case "asin":
                    return Complex32.Asin(argument);

                case "acos":
                    return Complex32.Acos(argument);

                case "atan":
                    return Complex32.Atan(argument);
            }

            throw new ArgumentException();
        }

        private static Complex32 ExecuteOperation(string op, Complex32 a, Complex32 b)
        {
            switch (op)
            {
                case "^":
                    return Complex32.Pow(b, a);

                case "*":
                    return b * a;

                case "/":
                    return b / a;

                case "+":
                    return b + a;

                case "-":
                    return b - a;
            }

            throw new ArgumentException();
        }

        public static string AddSpaces(this string input)
        {
            var result = "";

            for (int i = 0; i < input.Length; ++i)
            {
                if (Operators.ContainsKey(input[i].ToString()) || input[i] == '(' || input[i] == ')')
                    result += " " + input[i] + " ";
                else
                    result += input[i];
            }

            input = result;
            return input;
        }

        public static Queue<string> Parse(string input)
        {
            var x = new Complex32(0, 0);
            var symbol = "";

            var result = new Queue<string>();
            var stack = new Stack<string>();

            var symbols = input.Replace(" ", "").AddSpaces().Split(' ');

            for (int i = 0; i < symbols.Length; ++i)
            {
                symbol = symbols[i];

                switch (symbol)
                {
                    case string s when Complex32.TryParse(s, out x) || s == "t" || s == "x" || s == "y" || s == "z" || s == "pi" || s == "e":
                        result.Enqueue(s);
                        break;

                    case string s when Functions.Contains(s):
                        stack.Push(s);
                        break;

                    case string s when Operators.ContainsKey(s):
                        if (stack.Count > 0)
                        {
                            while (Operators.ContainsKey(stack.Peek()))
                            {
                                if (Operators[s] <= Operators[stack.Peek()])
                                    result.Enqueue(stack.Pop());
                            }
                        }

                        stack.Push(s);
                        break;

                    case string s when s == "(":
                        stack.Push(s);
                        break;

                    case string s when s == ")":
                        while (stack.Count > 0)
                        {
                            if (stack.Peek() == "(")
                            {
                                stack.Pop();

                                if (stack.Count > 0)
                                {
                                    if (Functions.Contains(stack.Peek()))
                                        result.Enqueue(stack.Pop());
                                }

                                break;
                            }

                            result.Enqueue(stack.Pop());
                        }
                        break;
                }
            }

            while (stack.Count > 0)
                result.Enqueue(stack.Pop());

            return result;
        }

        public static Complex32 Calculate(string input, Complex32 t)
        {
            var queue = Parse(input);
            var stack = new Stack<Complex32>();
            var x = new Complex32(0, 0);

            while (queue.Count > 0)
            {
                switch (queue.Dequeue())
                {
                    case string s when s == "t" || s == "x" || s == "y" || s == "z":
                        stack.Push(t);
                        break;

                    case string s when s == "pi":
                        stack.Push(MathF.PI);
                        break;

                    case string s when s == "e":
                        stack.Push(MathF.E);
                        break;

                    case string s when Complex32.TryParse(s, out x):
                        stack.Push(x);
                        break;

                    case string s when Operators.ContainsKey(s):
                        var a = stack.Pop();
                        var b = stack.Pop();

                        stack.Push(ExecuteOperation(s, a, b));
                        break;

                    case string s when Functions.Contains(s):
                        var c = stack.Pop();

                        stack.Push(ExecuteFunction(s, c));
                        break;
                }
            }

            return stack.Pop();
        }

        public static Complex32 Calculate(string input)
        {
            var queue = Parse(input);
            var stack = new Stack<Complex32>();
            var x = new Complex32(0, 0);

            while (queue.Count > 0)
            {
                switch (queue.Dequeue())
                {
                    case string s when Complex32.TryParse(s, out x):
                        stack.Push(x);
                        break;

                    case string s when s == "pi":
                        stack.Push(MathF.PI);
                        break;

                    case string s when s == "e":
                        stack.Push(MathF.E);
                        break;

                    case string s when Operators.ContainsKey(s):
                        var a = stack.Pop();
                        var b = stack.Pop();

                        stack.Push(ExecuteOperation(s, a, b));
                        break;

                    case string s when Functions.Contains(s):
                        var c = stack.Pop();

                        stack.Push(ExecuteFunction(s, c));
                        break;
                }
            }

            return stack.Pop();
        }
    }
}
