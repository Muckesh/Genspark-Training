// 5) Take 10 numbers from user and print the number of numbers that are divisible by 7	

using System;

class Program
{
    static void Main(string[] args)
    {
        int divisibleBy7 = 0;
        for (int i = 1; i <= 10; i++)
        {
            Console.Write($"Enter number {i} : ");
            if (int.TryParse(Console.ReadLine(), out int num))
            {
                if (num % 7 == 0)
                    divisibleBy7++;
            }
            else
            {
                Console.WriteLine("Enter a valid number.");
                i--;
            }
        }
        Console.WriteLine($"{divisibleBy7} numbers are divisible by 7.");
    }
}