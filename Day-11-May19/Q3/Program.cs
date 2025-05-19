//3) Take 2 numbers from user, check the operation user wants to perform (+,-,*,/). Do the operation and print the result

using System;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.Write("Enter number 1 : ");
            int num1 = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter number 2 : ");
            int num2 = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter the operation to perform (+,-,*/) : ");
            char operation = Console.ReadLine()[0];

            switch (operation)
            {
                case '+':
                    Console.WriteLine($"{num1} + {num2} = {num1 + num2}");
                    break;
                case '-':
                    Console.WriteLine($"{num1} - {num2} = {num1 - num2}");
                    break;
                case '*':
                    Console.WriteLine($"{num1} * {num2} = {num1 * num2}");
                    break;
                case '/':
                    Console.WriteLine($"{num1} / {num2} = {num1 / num2}");
                    break;

                default:
                    Console.WriteLine("Invalid operation. Please enter one of +,-,*,/.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred : " + ex.Message);
        }
    }
}