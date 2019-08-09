using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Compilers.Build
{
    public class AddTask : Task
    {
        private int number1;

        [Required]
        public int Number1
        {
            get { return number1; }
            set { number1 = value; }
        }

        private int number2;

        [Required]
        public int Number2
        {
            get { return number2; }
            set { number2 = value; }
        }

        private int sum;

        [Output]
        public int Sum
        {
            get { return sum; }
            set { sum = value; }
        }

        public override bool Execute()
        {
            try
            {
                sum = number1 + number2;
            }
            catch (ArithmeticException e)
            {
                Console.WriteLine("Error occured during addition: {0}", e.Message);
                return false;
            }
            return true;
        }
    }
}