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
            "sqrt",
            "exp",
            "log",
            "sin",
            "cos",
            "tan",
            "asin",
            "acos",
            "atan",
        };

        private static System.Numerics.Complex ExecuteFunction(string function, System.Numerics.Complex argument)
        {
            switch (function)
            {
                case "sqrt":
                    return System.Numerics.Complex.Sqrt(argument);

                case "exp":
                    return System.Numerics.Complex.Exp(argument);

                case "log":
                    return System.Numerics.Complex.Log(argument);

                case "sin":
                    return System.Numerics.Complex.Sin(argument);

                case "cos":
                    return System.Numerics.Complex.Cos(argument);

                case "tan":
                    return System.Numerics.Complex.Tan(argument);

                case "asin":
                    return System.Numerics.Complex.Asin(argument);

                case "acos":
                    return System.Numerics.Complex.Acos(argument);

                case "atan":
                    return System.Numerics.Complex.Atan(argument);
            }

            throw new ArgumentException();
        }

        private static System.Numerics.Complex ExecuteOperation(string op, System.Numerics.Complex a, System.Numerics.Complex b)
        {
            switch (op)
            {
                case "^":
                    return System.Numerics.Complex.Pow(b, a);

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
                {
                    if (input[i] == '-')
                    {
                        if (i - 1 < 0)
                        {
                            result += input[i];
                            continue;
                        }    

                        if ((input[i - 1] >= '0' && input[i - 1] <= '9') || input[i - 1] == ')')
                        {
                            result += " " + input[i] + " ";
                            continue;
                        }

                        result += input[i];
                    }

                    result += " " + input[i] + " ";
                }
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
            var o = "";

            for (int i = 0; i < symbols.Length; ++i)
            {
                symbol = symbols[i];

                switch (symbol)
                {
                    case string s when Complex32.TryParse(s, out x) || s == "t" || s == "x" || s == "y" || s == "z" || s == "pi" || s == "e" || s == "h":
                        result.Enqueue(s);
                        break;

                    case string s when Functions.Contains(s):
                        stack.Push(s);
                        break;

                    case string s when Operators.ContainsKey(s):
                        while (stack.TryPeek(out o) && Operators.ContainsKey(o) && Operators[s] <= Operators[o])
                            result.Enqueue(stack.Pop());

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

            while (stack.TryPeek(out symbol))
                result.Enqueue(stack.Pop());

            return result;
        }

        public static System.Numerics.Complex Calculate(string input, System.Numerics.Complex t)
        {
            var queue = Parse(input);
            var stack = new Stack<System.Numerics.Complex>();
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

                    case string s when s == "h":
                        stack.Push(1.054571817f);
                        break;

                    case string s when Complex32.TryParse(s, out x):
                        stack.Push(new System.Numerics.Complex(x.Real, x.Imaginary));
                        break;

                    case string s when Operators.ContainsKey(s):
                        var a = stack.Pop();
                        System.Numerics.Complex b;

                        if (stack.TryPeek(out b))
                            b = stack.Pop();
                        else
                            b = System.Numerics.Complex.Zero;

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

        public static System.Numerics.Complex Calculate(string input, System.Numerics.Complex x, System.Numerics.Complex y)
        {
            var queue = Parse(input);
            var stack = new Stack<System.Numerics.Complex>();
            var t = new Complex32(0, 0);
            var r = "";

            while (queue.TryPeek(out r))
            {
                switch (queue.Dequeue())
                {
                    case string s when s == "x":
                        stack.Push(x);
                        break;

                    case string s when s == "y":
                        stack.Push(y);
                        break;

                    case string s when s == "pi":
                        stack.Push(MathF.PI);
                        break;

                    case string s when s == "e":
                        stack.Push(MathF.E);
                        break;

                    case string s when s == "h":
                        stack.Push(1.054571817f);
                        break;

                    case string s when Complex32.TryParse(s, out t):
                        stack.Push(new System.Numerics.Complex(t.Real, t.Imaginary));
                        break;

                    case string s when Operators.ContainsKey(s):
                        var a = stack.Pop();
                        System.Numerics.Complex b;

                        if (stack.TryPeek(out b))
                            b = stack.Pop();
                        else
                            b = System.Numerics.Complex.Zero;

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

        public static System.Numerics.Complex Calculate(string input)
        {
            var queue = Parse(input);
            var stack = new Stack<System.Numerics.Complex>();
            var x = new Complex32(0, 0);

            while (queue.Count > 0)
            {
                switch (queue.Dequeue())
                {
                    case string s when Complex32.TryParse(s, out x):
                        stack.Push(new System.Numerics.Complex(x.Real, x.Imaginary));
                        break;

                    case string s when s == "pi":
                        stack.Push(MathF.PI);
                        break;

                    case string s when s == "e":
                        stack.Push(MathF.E);
                        break;

                    case string s when s == "h":
                        stack.Push(1.054571817f * MathF.Pow(10, -3f));
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
