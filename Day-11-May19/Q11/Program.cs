/*
    11) In the question ten extend it to validate a sudoku game. 
    Validate all 9 rows (use int[,] board = new int[9,9])
*/

using System;

class Program
{
    static int[,] GetSudokuBoardFromUser()
    {
        int[,] board = new int[9, 9];
        Console.WriteLine("Enter the sudoku board row by row (9 numbers per row, space separated):");

        for (int row = 0; row < 9; row++)
        {
            Console.Write($"Row {row + 1} : ");
            string[] input = Console.ReadLine().Trim().Split(' ');

            while (input.Length != 9)
            {
                Console.WriteLine("Please enter exactly 9 numbers separated by spaces.");
                Console.Write($"Row {row + 1}: ");
                input = Console.ReadLine().Trim().Split(' ');
            }

            for (int col = 0; col < 9; col++)
            {
                if (!int.TryParse(input[col], out int number) || number < 1 || number > 9)
                {
                    Console.WriteLine("Each number must be between 1 and 9. Exiting....");
                    Environment.Exit(0);
                }
                board[row, col] = number;
            }
        }

        return board;
    }

    static bool isValidSudoku(int[,] board)
    {
        for (int i = 0; i < 9; i++)
        {
            if (!isValidGroup(GetRow(board, i)) ||
                    !isValidGroup(GetColumn(board, i)) ||
                    !isValidGroup(GetBox(board, i))
             )
            {
                return false;
            }
        }
        return true;
    }

    static int[] GetRow(int[,] board, int row)
    {
        int[] result = new int[9];
        for (int i = 0; i < 9; i++)
        {
            result[i] = board[row, i];
        }
        return result;
    }

    static int[] GetColumn(int[,] board, int col)
    {
        int[] result = new int[9];
        for (int i = 0; i < 9; i++)
        {
            result[i] = board[i, col];
        }
        return result;
    }

    static int[] GetBox(int[,] board, int boxIndex)
    {
        int[] result = new int[9];
        int startRow = (boxIndex / 3) * 3;
        int startCol = (boxIndex % 3) * 3;

        int k = 0;
        for (int r = startRow; r < startRow + 3; r++)
        {
            for (int c = startCol; c < startCol + 3; c++)
            {
                result[k++] = board[r, c];
            }
        }
        return result;
    }

    static bool isValidGroup(int[] group)
    {
        HashSet<int> seen = new HashSet<int>();
        foreach (int num in group)
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
        int[,] board = GetSudokuBoardFromUser();

        if (isValidSudoku(board))
        {
            Console.WriteLine("\n The Sudoku board is valid.");
        }
        else
        {
            Console.WriteLine("The Sudoku board is invalid.");
        }
    }
}

/*
Row 1: 5 3 4 6 7 8 9 1 2
Row 2: 6 7 2 1 9 5 3 4 8
Row 3: 1 9 8 3 4 2 5 6 7
Row 4: 8 5 9 7 6 1 4 2 3
Row 5: 4 2 6 8 5 3 7 9 1
Row 6: 7 1 3 9 2 4 8 5 6
Row 7: 9 6 1 5 3 7 2 8 4
Row 8: 2 8 7 4 1 9 6 3 5
Row 9: 3 4 5 2 8 6 1 7 9

*/
