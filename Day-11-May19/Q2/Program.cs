// 2) Take 2 numbers from user and print the largest

using System;

class Program
{

    static int Max(int num1, int num2)
    {
        return num1 > num2 ? num1 : num2;
    }
    static void Main(string[] args)
    {
        try
        {
            Console.Write("Enter number 1 : ");
            int num1 = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter number 2 : ");
            int num2 = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine($"The maximum of {num1} and {num2} is {Max(num1, num2)}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred : " + ex.Message);
        }
    }
}