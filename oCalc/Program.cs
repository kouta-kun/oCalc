using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace oCalc
{


    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<char, IExpression<double>> bindings = new Dictionary<char, IExpression<double>>();
            Dictionary<char, IExpression<double>> variables = new Dictionary<char, IExpression<double>>();
            Dictionary<char, JITFunction> functions = new Dictionary<char, JITFunction>();
            Console.WriteLine("ExpInt 0.1.\nEscriba '?' para ayuda.");
            while (true)
            {
                Console.Write(">> ");
                var input = Console.ReadLine();
                if (input == "?")
                {
                    Console.WriteLine("Interprete de expresiones");
                    Console.WriteLine("Soporta: multiplicación (*), sumas (+), restas (-), divisiones (/), paréntesis, variables, potencias (^), y raíces(_).");
                    Console.WriteLine("VARIABLES: [a-z]=valor numerico -- Asignar valor a variable de una letra");
                    Console.WriteLine("VARIABLES: $[a-z] -- Se remplaza por valor de la variable en expresiones");
                    Console.WriteLine("CONSTANTES: %e -- Numero e");
                    Console.WriteLine("CONSTANTES: %p -- Pi");
                    Console.WriteLine("NEGACION: <x -- valor negativo de x. No puede usarse la notación '-x', en su lugar debe usar '<x' por implementación");
                    Console.WriteLine("¿(EXPR): Imprimir el árbol de expresiones al que se evalúa EXPR");
                }
                else if (input.Contains("="))
                {
                    variables[input[0]] = ParseExpression.Parse(input.Split('=')[1], bindings);
                    if (!bindings.ContainsKey(input[0]))
                    {
                        bindings.Add(input[0], new Variable(input[0], variables));
                    }
                }
                else if (input.StartsWith("¿"))
                {
                    var expr = ParseExpression.Parse(input.Substring(1), bindings);
                    Console.WriteLine(expr.AsTree());
                }
                else if (input.Contains("->"))
                {
                    int pos = input.IndexOf("->");
                    var functionHead = input.Substring(0, pos).Trim();
                    var functionTail = input.Substring(pos + 2).Trim();
                    var _nargs = functionHead.Substring(2, functionHead.Length - 3).Split(',');
                    var nargs = new char[_nargs.Length];
                    Dictionary<char, IExpression<double>> vars = new Dictionary<char, IExpression<double>>();
                    Dictionary<char, IExpression<double>> fvars = new Dictionary<char, IExpression<double>>();
                    for (int i = 0; i < nargs.Length; i++)
                    {
                        nargs[i] = _nargs[i].Trim()[0];
                        fvars.Add(nargs[i], new Constant(0));
                        vars[nargs[i]] = new Variable(nargs[i], fvars);
                    }
                    var expr = ParseExpression.Parse(functionTail, vars);
                    functions[functionHead[0]] = new JITFunction(expr, new List<char>(nargs), functionHead[0]);
                }
                else if (input.EndsWith("!"))
                {
                    var iargs = input.Substring(2, input.Length - 4).Split(',');
                    var vargs = new IExpression<double>[iargs.Length];
                    for(int i = 0; i < iargs.Length; i++)
                    {
                        vargs[i] = ParseExpression.Parse(iargs[i].Trim(), variables);
                    }
                    Console.WriteLine(input[0] + "(): " + functions[input[0]].Evaluate(vargs));
                }
                else
                {
                    var Expr = ParseExpression.Parse(input, bindings);
                    if (Expr == null)
                    {
                        Console.WriteLine("Expresion inválida");
                    }
                    else
                    {
                        Console.WriteLine("Resultado: " + Expr.Evaluate());
                    }
                }
            }
        }
    }
}
