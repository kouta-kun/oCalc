using System;

namespace oCalc
{
    public class BinaryOp : IExpression<double>
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
                    if (Operation == Op.Substract) // Si es una resta, la convertimos a suma usando su negativa
                    {
                        rhs = rhs.Negative();
                        Operation = Op.Add;
                    }
                    return lhs.Evaluate() + rhs.Evaluate();
                case Op.Multiply:
                case Op.Divide:
                    if (Operation == Op.Divide) // si es una división, la convertimos a multiplicación usando su inversa
                    {
                        rhs = rhs.Inverse();
                        Operation = Op.Multiply;
                    }
                    return lhs.Evaluate() * rhs.Evaluate();
                case Op.Power:
                    return Math.Pow(lhs.Evaluate(), rhs.Evaluate());
                case Op.Root:
                    return Math.Pow(lhs.Evaluate(), 1 / rhs.Evaluate()); // a^(1/b) == raiz b-sima de a
                default:
                    throw new InvalidOperationException("Unknown operation");
            }
        }

        public bool IsConstant()
        {
            return (lhs is Constant || (lhs is BinaryOp bl && bl.IsConstant())) && (rhs is Constant || (rhs is BinaryOp br && br.IsConstant()));
        }
    }
}
