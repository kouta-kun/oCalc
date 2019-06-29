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
            Dictionary<char, IExpression<double>> variables = new Dictionary<char, IExpression<double>>();
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
                }
                else if (input.Contains("="))
                {
                    variables[input.Split('=')[0][0]] = ParseExpression.Parse(input.Split('=')[1], variables);
                }
                else
                {
                    var Expr = ParseExpression.Parse(input, variables);
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
