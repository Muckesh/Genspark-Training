/*
    10) write a program that accepts a 9-element array representing a Sudoku row.

    Validates if the row:

    Has all numbers from 1 to 9.

    Has no duplicates.

    Displays if the row is valid or invalid.
*/

using System;

class Program
{
    static int[] GetSudokuRowFromUser()
    {
        int[] row = new int[9];

        Console.WriteLine("Enter 9 numbers (1 to 9) separated by spaces.");
        string[] input = Console.ReadLine().Split(' ');

        while (input.Length != 9)
        {
            Console.WriteLine("Please enter exactly 9 numbers : ");
            input = Console.ReadLine().Split(' ');
        }
        for (int i = 0; i < 9; i++)
        {
            if (!int.TryParse(input[i], out row[i]) || row[i] < 1 || row[i] > 9)
            {
                Console.WriteLine("Invalid input. All numbers must be between 1 and 9.");
                Environment.Exit(0);
            }
        }

        return row;
    }

    static bool isValidSudokuRow(int[] row)
    {
        HashSet<int> seen = new HashSet<int>();

        foreach (int num in row)
        {
            if (seen.Contains(num))
            {
                return false;
            }
            seen.Add(num);
        }
        return seen.Count == 9;
    }
    static void Main(string[] args)
    {
        int[] sudokuRow = GetSudokuRowFromUser();

        if (isValidSudokuRow(sudokuRow))
        {
            Console.WriteLine("The Sudoku row is valid.");
        }
        else
        {
            Console.WriteLine("The Sudoku row is invalid.");
        }
    }

}

