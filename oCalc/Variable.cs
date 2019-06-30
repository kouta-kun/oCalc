using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oCalc
{
    class Variable : IExpression<double>
    {
        public readonly char Binding;
        public readonly IExpression<double> BoundExpression;

        public Variable(char binding, IExpression<double> boundExpression)
        {
            Binding = binding;
            BoundExpression = boundExpression;
        }

        public double Evaluate()
        {
            return BoundExpression.Evaluate();
        }
    }
}
