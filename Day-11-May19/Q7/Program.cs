/*
    7) create a program to rotate the array to the left by one position.
    Input: {10, 20, 30, 40, 50}
    Output: {20, 30, 40, 50, 10}
*/

using System;

class Program
{
    static void reverse(int[] arr, int start, int end)
    {
        while (start < end)
        {
            int temp = arr[start];
            arr[start] = arr[end];
            arr[end] = temp;
            start++;
            end--;
        }
    }
    static void rotate(int[] arr, int k)
    {
        k = k % arr.Length;
        if (k < 0)
            k = k + arr.Length;
        reverse(arr, 0, k - 1);
        reverse(arr, k, arr.Length - 1);
        reverse(arr, 0, arr.Length - 1);
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
        int[] arr = GetArrayFromUser();
        Console.Write("Enter the number of times to rotate : ");
        if (!int.TryParse(Console.ReadLine(), out int k))
            Console.WriteLine("Please enter a valid rotation.");
        
        Console.WriteLine("Original Array : " + string.Join(" ", arr));
        rotate(arr, k);
        Console.WriteLine("Rotated Array : " + string.Join(" ", arr));
    }
}