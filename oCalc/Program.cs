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
                    Console.WriteLine("¿(EXPR): Imprimir el árbol de expresiones al que se evalúa EXPR");
                }
                else if (input.Contains("="))
                {
                    var variableExpression = ParseExpression.Parse(input.Split('=')[1], variables);
                    variables[input[0]] = new Variable(input[0], variableExpression);
                }
                else if (input.StartsWith("¿"))
                {
                    var expr = ParseExpression.Parse(input.Substring(1), variables);
                    Console.WriteLine(expr.AsTree());
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
