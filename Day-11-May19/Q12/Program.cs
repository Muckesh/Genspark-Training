/*
    12) Write a program that:

    Takes a message string as input (only lowercase letters, no spaces or symbols).

    Encrypts it by shifting each character forward by 3 places in the alphabet.

    Decrypts it back to the original message by shifting backward by 3.

    Handles wrap-around, e.g., 'z' becomes 'c'.

    Examples
    Input:     hello
    Encrypted: khoor
    Decrypted: hello
    -------------
    Input:     xyz
    Encrypted: abc
    Test cases
    | Input | Shift | Encrypted | Decrypted |
    | ----- | ----- | --------- | --------- |
    | hello | 3     | khoor     | hello     |
    | world | 3     | zruog     | world     |
    | xyz   | 3     | abc       | xyz       |
    | apple | 1     | bqqmf     | apple     |
*/

using System;
using System.Net.Security;

class Program
{
    static bool isValidInput(string message)
    {
        foreach (char c in message)
        {
            if (c < 'a' || c > 'z')
            {
                return false;
            }
        }
        return true;
    }
    static string Encrypt(string message, int shift)
    {
        char[] result = new char[message.Length];

        for (int i = 0; i < message.Length; i++)
        {
            char shifted = (char)(((message[i] - 'a' + shift) % 26) + 'a');
            result[i] = shifted;
        }
        return new string(result);
    }

    static string Decrypt(string message, int shift)
    {
        char[] result = new char[message.Length];

        for (int i = 0; i < message.Length; i++)
        {
            char shifted = (char)(((message[i] - 'a' - shift + 26) % 26) + 'a');
            result[i] = shifted;
        }
        return new string(result);
    }
    static void Main(string[] args)
    {
        Console.WriteLine("Enter a lowercase message (no space or symbols) : ");
        string message = Console.ReadLine();

        if (!isValidInput(message))
        {
            Console.WriteLine("Invalid input. Only lowercase letters are allowed.");
            return;
        }

        Console.WriteLine("Enter the shift value : ");
        int shift;

        while (!int.TryParse(Console.ReadLine(), out shift))
        {
            Console.WriteLine("Please enter a valid shift value.");
        }

        string encrypted = Encrypt(message, shift);
        string decrypted = Decrypt(encrypted, shift);

        Console.WriteLine($"\nEncrypted : {encrypted}");
        Console.WriteLine($"Decrypted : {decrypted}");
    }
}