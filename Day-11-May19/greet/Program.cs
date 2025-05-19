/*
1) create a program that will take name from user and greet the user
*/

using System;

class Program {

    static void greet(string? name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            Console.WriteLine($"Hello, {name}. Good Morning!");
        }
        else
        {
            Console.WriteLine("Enter a valid name");
        }
    }
    static void Main(string[] args)
    {
        Console.Write("Enter your name : ");
        string? name = Console.ReadLine();
        greet(name);
    }
}