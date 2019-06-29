using System;
using System.Collections.Generic;

namespace oCalc
{
    static class ParseExpression
    {
        private static Dictionary<char, IExpression<double>> Constants = new Dictionary<char, IExpression<double>>
        {
            {'e', new Constant(Math.E) },
            {'p', new Constant(Math.PI) }
        };
        public static IExpression<double> Parse(string Expression, Dictionary<char, IExpression<double>> variables)
        {
            int operationIndex = Expression.RankedIndex("(+-*/^_".ToCharArray());
            if (operationIndex >= 0 && Expression[operationIndex] != '(')
            {
                Op operation = Functions.CharToOp(Expression[operationIndex]);
                IExpression<double> lhs = Parse(Expression.Substring(0, operationIndex), variables);
                IExpression<double> rhs = Parse(Expression.Substring(operationIndex + 1), variables);
                return new BinaryOp(operation, lhs, rhs);
            }
            else if (operationIndex < 0)
            {
                if (Char.IsDigit(Expression[0]))
                    return Constant.Parse(Expression);
                else if (Expression[0] == '$' && variables.ContainsKey(Expression[1]))
                {
                    return variables[Expression[1]];
                }
                else if (Expression[0] == '%' && Constants.ContainsKey(Expression[1]))
                {
                    return Constants[Expression[1]];
                } else if(Expression[0] == '<')
                {
                    return Constant.Parse(Expression.Substring(1)).Negative();
                }
                else
                    return null;
            }
            else  // Si es un paréntesis, buscamos el final del mismo y pasamos el string contenido dentro del mismo a esta misma función
            {
                int depth = 1;
                for (int end = 1; end < Expression.Length - operationIndex; end++)
                {
                    if (Expression[operationIndex + end] == '(') depth++;
                    else if (Expression[operationIndex + end] == ')') depth--;
                    if (depth == 0)
                    {
                        var innerExp = Parse(Expression.Substring(operationIndex + 1, end - 1), variables);
                        Expression = Expression.Substring(0, operationIndex) + innerExp.Evaluate().ToString("F8") + Expression.Substring(operationIndex + end + 1);
                        return Parse(Expression, variables);
                    }
                }
                return null;
            }
        }
    }
}
