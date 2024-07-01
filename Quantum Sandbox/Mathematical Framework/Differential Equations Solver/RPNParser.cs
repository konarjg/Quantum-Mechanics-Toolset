using ScottPlot.MinMaxSearchStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM
{
    namespace Symbolics
    {
        public class Expression
        {
            private Queue<string> Tokens;

            public Expression(string expression)
            {
                Tokens = RPNParser.Parse(expression);
            }

            public Expression(Queue<string> tokens)
            {
                Tokens = tokens;
            }

            public Queue<string> GetTokens()
            {
                return Tokens;
            }

            public override string ToString()
            {
                var output = "";

                for (int i = 0; i < Tokens.Count; ++i)
                    output += Tokens.ElementAt(i) + " ";

                return output.TrimEnd();
            }

            public double Calculate(params double[] variables)
            {
                return RPNParser.Calculate(this, variables);
            }
        }

        public static class RPNParser
        {
            private static Dictionary<char, (int, Func<double, double, double>)> Operators = new()
            {
                { '^', (2, (a, b) => Math.Pow(b, a)) },
                { '*', (1, (a, b) => b * a) },
                { '/', (1, (a, b) => b / a) },
                { '+', (0, (a, b) => b + a) },
                { '-', (0, (a, b) => b - a) }
            };

            private static Dictionary<string, Func<double, double>> Functions = new()
            {
                { "exp", Math.Exp },
                { "ln", Math.Log },
                { "sin", Math.Sin },
                { "cos", Math.Cos },
                { "tan", Math.Tan },
                { "cot", x => 1 / Math.Tan(x) },
                { "sqrt", Math.Sqrt }
            };

            public static double Operator(char o, double a, double b)
            {
                return Operators[o].Item2(a, b);
            }

            public static double Function(string f, double x)
            {
                return Functions[f](x);
            }

            public static string AddSpaces(this string input)
            {
                var output = "";
                var x = 0d;

                for (int i = 0; i < input.Length; ++i)
                {
                    if (Operators.ContainsKey(input[i]) || input[i] == '(' || input[i] == ')')
                    {
                        if (input[i] == '-')
                        {
                            if (i - 1 < 0)
                                output += "0 " + input[i] + " ";
                            else if (double.TryParse(input[i - 1].ToString(), out x) || input[i] == 'x' || input[i] == 'y' || input[i] == 'z')
                                output += " " + input[i] + " ";
                            else
                                output += "0 " + input[i] + " ";
                        }
                        else
                            output += " " + input[i] + " ";
                    }
                    else
                        output += input[i];
                }

                input = output;
                return input;
            }

            public static Queue<string> Parse(this string input)
            {
                var tokens = input.Replace(" ", "").AddSpaces().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var x = 0d;

                var output = new Queue<string>();
                var stack = new Stack<string>();
                var o2 = "";

                for (int i = 0; i < tokens.Length; ++i)
                {
                    var token = tokens[i];

                    switch (token)
                    {
                        case string s when double.TryParse(token, out x) || token == "x" || token == "y" || token == "z":
                            output.Enqueue(s);
                            break;

                        case string s when Functions.ContainsKey(s):
                            stack.Push(s);
                            break;

                        case string s when Operators.ContainsKey(s[0]):
                            while (stack.TryPeek(out o2) && Operators.ContainsKey(o2[0]) && Operators[o2[0]].Item1 >= Operators[s[0]].Item1)
                                output.Enqueue(stack.Pop());

                            stack.Push(s);
                            break;

                        case string s when s == "(":
                            stack.Push(s);
                            break;

                        case string s when s == ")":
                            while (stack.TryPeek(out o2))
                            {
                                if (o2 == "(")
                                {
                                    stack.Pop();

                                    if (stack.TryPeek(out o2))
                                    {
                                        if (Functions.ContainsKey(o2))
                                            output.Enqueue(stack.Pop());
                                    }

                                    break;
                                }

                                output.Enqueue(stack.Pop());
                            }

                            break;
                    }
                }

                while (stack.TryPeek(out o2))
                    output.Enqueue(stack.Pop());

                return output;      
            }

            public static double Calculate(Expression expression, params double[] variables)
            {
                var stack = new Stack<double>();
                var x = 0d;
                var a = 0d;
                var b = 0d;

                var tokens = expression.GetTokens();

                for (int i = 0; i < tokens.Count; ++i)
                {
                    var token = tokens.ElementAt(i);

                    switch (token)
                    {
                        case string s when double.TryParse(s, out x):
                            stack.Push(x);
                            break;

                        case string s when s == "x":
                            stack.Push(variables[0]);
                            break;

                        case string s when s == "y":
                            stack.Push(variables[1]);
                            break;

                        case string s when s == "z":
                            stack.Push(variables[2]);
                            break;

                        case string s when Operators.ContainsKey(s[0]):
                            a = stack.Pop();
                            b = stack.Pop();
                            var t = Operator(s[0], a, b);

                            stack.Push(t);
                            break;

                        case string s when Functions.ContainsKey(s):
                            x = stack.Pop();
                            stack.Push(Function(s, x));
                            break;
                    }
                }

                return stack.Pop();
            }
        }
    }
}
