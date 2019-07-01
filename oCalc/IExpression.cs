namespace oCalc
{
    public interface IExpression<T>
    { // expresión es algo que puede ser evaluado a un valor
        T Evaluate();          // ejemplo: "2", "5+8", "(3*2)+9"
    };
}
