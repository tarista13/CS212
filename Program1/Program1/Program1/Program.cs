using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Program1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nEnter n: ");
            int n = int.Parse(Console.ReadLine());
            int result = log(n);
            Console.WriteLine("(lg lg({0})) = {1}.", n, result);
        }

        static int log(int n)
        {
            return (n > 1) ? 1 + log(n / 2) : 0;
        }
    }
}

