using System;
using System.Windows.Forms;

namespace UniversalCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Form1 f = new Form1();
            Application.Run(f);

            Frac n1 = new Frac(0, 0);
            Frac n2 = new Frac(1, 2);

            System.Console.WriteLine(n1.Add(n2));


        }
    }
}
