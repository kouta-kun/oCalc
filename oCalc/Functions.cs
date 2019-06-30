using System;
using System.Linq;

namespace oCalc
{
    static class Functions
    {
        public static string AsTree(this IExpression<double> expr)
        {
            if (expr == null)
                return "Null expression";
            if (expr is Constant)
                return expr.Evaluate().ToString("F3");
            else if (expr is BinaryOp bExpr)
            {
                string ret = Enum.GetName(typeof(Op), bExpr.Operation);
                ret += "\n|-- ";
                string lhs = bExpr.lhs.AsTree();
                ret += LineSplit(lhs);
                ret += "|-- ";
                string rhs = bExpr.rhs.AsTree();
                ret += LineSplit(rhs);
                return ret;
            }
            else if (expr is Variable variable)
            {
                string ret = "Variable $" + variable.Binding;
                try
                {
                    string exp = variable.BoundExpression.AsTree();
                    ret += "\n|-- Expression: " + LineSplit(exp, 16);
                }
                catch (InvalidOperationException)
                {
                    ret += " not yet bound to an expression";
                }
                return ret;
            }
            else
            {
                Console.Error.WriteLine("Don't know how to parse IExpression of type " + expr.GetType().Name + ", defaulting to evaluated value");
                return expr.Evaluate().ToString("F3");
            }
        }

        private static string LineSplit(string lhs, int leftPad = 4)
        {
            if (!lhs.Contains("\n"))
            {
                return lhs + "\n";
            }
            else
            {
                string[] splitLhs = lhs.Split('\n');
                string ret = splitLhs[0] + "\n";
                for (int i = 1; i < splitLhs.Length; i++)
                {
                    ret += string.Concat(Enumerable.Repeat(" ", leftPad)) + splitLhs[i] + "\n";
                }
                return ret;
            }
        }

        public static IExpression<double> Inverse(this IExpression<double> expr) // Convertir las divisiones en multiplicaciones
        {                                                                        // a / b = a * (1/b)
            return new Constant(1 / expr.Evaluate());
        }

        public static IExpression<double> Negative(this IExpression<double> expr) // Convertir las sumas en restas
        {                                                                         // a - b = a + (-b)
            return new Constant(-expr.Evaluate());
        }

        public static int RankedIndex(this string str, char[] characters)         // La primera aparición en el texto del caracter de menor índice del arreglo
        {                                                                         // ejemplo: "3+2*5^2", {+*^} -> 1, posición del caracter '+' en el texto
                                                                                  //          "3+2*5^",  {*+^} -> 3, posición del caracter '*' en el texto
            var token = characters.Select(x => str.IndexOf(x)).Where(x => x >= 0);
            if (token.Count() == 0)
            {
                return -1;
            }
            else return token.First();
        }
        public static Op CharToOp(char character)
        {
            switch (character)
            {
                case '+':
                    return Op.Add;
                case '*':
                    return Op.Multiply;
                case '/':
                    return Op.Divide;
                case '-':
                    return Op.Substract;
                case '^':
                    return Op.Power;
                case '_':
                    return Op.Root;
                default:
                    throw new InvalidOperationException("Unknown operation");
            }
        }
    }
}
