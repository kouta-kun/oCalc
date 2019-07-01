using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oCalc
{
    public class Variable : IExpression<double>
    {
        public readonly char Binding;
        private readonly Dictionary<char, IExpression<double>> BoundExpression;

        public Variable(char binding, Dictionary<char, IExpression<double>> boundExpression)
        {
            System.Diagnostics.Debug.Assert(boundExpression.ContainsKey(binding));
            Binding = binding;
            BoundExpression = boundExpression;
        }

        public double Evaluate() => BoundExpression[Binding].Evaluate();

        public IExpression<double> GetExpression() => BoundExpression[Binding];
    }
}
