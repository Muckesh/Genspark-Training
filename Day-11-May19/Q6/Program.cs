/*
    6) Count the Frequency of Each Element
    Given an array, count the frequency of each element and print the result.
    Input: {1, 2, 2, 3, 4, 4, 4}

    output
    1 occurs 1 times  
    2 occurs 2 times  
    3 occurs 1 times  
    4 occurs 3 times
*/

using System;
using System.Collections.Generic;
class Program
{
    static void PrintFrequency(Dictionary<int, int> frequency)
    {

        foreach (var item in frequency)
        {
            Console.WriteLine($"{item.Key} occurs {item.Value} times.");
        }
    }

    static int[] GetArrayFromUser()
    {
        Console.Write("Enter the size of the array : ");
        if (!int.TryParse(Console.ReadLine(),out int size))
        {
            Console.WriteLine("Please enter a valid size for the array");
        }
        int[] array = new int[size];

        for (int i = 0; i < size; i++)
        {
            Console.Write($"Enter number {i + 1} : ");
            //array[i] = int.Parse(Console.ReadLine());
            if (!int.TryParse(Console.ReadLine(), out array[i]))
            {
                Console.WriteLine("Enter a valid number");
                i--;
            }
        }

        return array;
    }

    static Dictionary<int, int> CountFrequency(int[] arr)
    {
        Dictionary<int, int> frequency = new Dictionary<int, int>();

        foreach (int num in arr)
        {
            if (frequency.ContainsKey(num))
            {
                frequency[num]++;
            }
            else
            {
                frequency[num] = 1;
            }
        }
        return frequency;
    }
    static void Main(string[] args)
    {
        //int[] arr = { 1, 2, 2, 3, 4, 4, 4 };
        int[] arr = GetArrayFromUser();
        Dictionary<int, int> frequency = CountFrequency(arr);

        Console.WriteLine("\nThe given array is : " + string.Join(' ', arr));
        PrintFrequency(frequency);
    }
}