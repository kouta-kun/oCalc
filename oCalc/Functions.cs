using System;
using System.Linq;

namespace oCalc
{
    static class Functions
    {
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
