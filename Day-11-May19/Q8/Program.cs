/*
    8) Given two integer arrays, merge them into a single array.
    Input: {1, 3, 5} and {2, 4, 6}
    Output: {1, 3, 5, 2, 4, 6}
*/

using System;

class Program
{
    static int[] Merge(int[] arr1, int[] arr2)
    {
        int len1 = arr1.Length;
        int len2 = arr2.Length;

        int[] merged = new int[len1 + len2];

        for (int i = 0; i < len1; i++)
        {
            merged[i] = arr1[i];
        }

        for (int i = 0; i < len2; i++)
        {
            merged[i + len1] = arr2[i];
        }

        return merged;
    }
    
    static int[] GetArrayFromUser()
    {
        Console.Write("Enter the size of the array : ");
        if (!int.TryParse(Console.ReadLine(), out int size))
            Console.WriteLine("Please enter a valid size for the array.");

        int[] arr = new int[size];

        for (int i = 0; i < size; i++)
        {
            Console.Write($"Enter number {i + 1} : ");
            if (!int.TryParse(Console.ReadLine(), out arr[i]))
            {
                Console.WriteLine("Please enter a valid number.");
                i--;
            }
        }
        return arr;
    }
    static void Main(string[] args)
    {
        int[] arr1 = GetArrayFromUser();
        int[] arr2 = GetArrayFromUser();
        int[] mergedArray = Merge(arr1, arr2);

        Console.WriteLine("Array 1 : " + string.Join(' ', arr1));
        Console.WriteLine("Array 2 : " + string.Join(' ', arr2));
        Console.WriteLine("Merged Array : " + string.Join(' ', mergedArray));
    }
}