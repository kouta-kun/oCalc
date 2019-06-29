using System;

namespace oCalc
{
    class Constant : IExpression<double>
    {
        public double Value;
        public double Evaluate()
        {
            return Value;
        }
        public Constant(double Value)
        {
            this.Value = Value;
        }

        public static Constant Parse(string str)
        {
            try
            {
                return new Constant(Double.Parse(str));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
