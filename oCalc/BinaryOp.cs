using System;

namespace oCalc
{
    class BinaryOp : IExpression<double>
    {
        public Op Operation;
        public IExpression<double> lhs;
        public IExpression<double> rhs;

        public BinaryOp(Op operation, IExpression<double> lhs, IExpression<double> rhs)
        {
            Operation = operation;
            this.lhs = lhs;
            this.rhs = rhs;
        }

        public double Evaluate()
        {
            switch (Operation)
            {
                case Op.Add:
                case Op.Substract:
                    if (Operation == Op.Add) // Si es una suma, la convertimos a substracción usando su negativa
                    {
                        rhs = rhs.Negative();
                        Operation = Op.Substract;
                    }
                    return lhs.Evaluate() - rhs.Evaluate();
                case Op.Multiply:
                case Op.Divide:
                    if (Operation == Op.Multiply) // si es una multiplicación, la convertimos a división usando su inversa
                    {
                        rhs = rhs.Inverse();
                        Operation = Op.Divide;
                    }
                    return lhs.Evaluate() / rhs.Evaluate();
                case Op.Power:
                    return Math.Pow(lhs.Evaluate(), rhs.Evaluate());
                case Op.Root:
                    return Math.Pow(lhs.Evaluate(), 1 / rhs.Evaluate()); // a^(1/b) == raiz b-sima de a
                default:
                    throw new InvalidOperationException("Unknown operation");
            }
        }
    }
}
