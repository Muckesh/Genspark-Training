/*
    9) Write a program that:

    Has a predefined secret word (e.g., "GAME").

    Accepts user input as a 4-letter word guess.

    Compares the guess to the secret word and outputs:

    X Bulls: number of letters in the correct position.

    Y Cows: number of correct letters in the wrong position.

    Continues until the user gets 4 Bulls (i.e., correct guess).

    Displays the number of attempts.

    Bull = Correct letter in correct position.

    Cow = Correct letter in wrong position.

    Secret Word	User Guess	Output	Explanation
    GAME	GAME	4 Bulls, 0 Cows	Exact match
    GAME	MAGE	1 Bull, 3 Cows	A in correct position, MGE misplaced
    GAME	GUYS	1 Bull, 0 Cows	G in correct place, rest wrong
    GAME	AMGE	2 Bulls, 2 Cows	A, E right; M, G misplaced
    NOTE	TONE	2 Bulls, 2 Cows	O, E right; T, N misplaced
*/

using System;
using System.Runtime.ConstrainedExecution;

class Program
{
    static void Main(string[] args)
    {
        string secretWord = "GAME";
        int attempts = 0;
        bool isGuessed = false;

        Console.WriteLine("Welcome to Bulls and Cows!");
        Console.WriteLine("Try to guess the 4-letter secret word.");
        Console.WriteLine("Bull = Correct letter in correct position.");
        Console.WriteLine("Cow = Correct letter in wrong position.\n");

        while (!isGuessed)
        {
            Console.Write("Enter your 4-letter guess word : ");
            string guess = Console.ReadLine().ToUpper();

            if (guess.Length != 4)
            {
                Console.WriteLine("Invalid input. Please enter a word with exactly 4-letters.\n");
                continue;
            }

            attempts++;
            int bulls = 0;
            int cows = 0;

            bool[] secretUsed = new bool[4];
            bool[] guessUsed = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                if (secretWord[i] == guess[i])
                {
                    bulls++;
                    secretUsed[i] = true;
                    guessUsed[i] = true;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (guessUsed[i])
                    continue;

                for (int j = 0; j < 4; j++)
                {
                    if (!secretUsed[j] && guess[i] == secretWord[j])
                    {
                        cows++;
                        secretUsed[j] = true;
                        break;
                    }
                }
            }

            Console.WriteLine($"Bulls : {bulls}, Cows : {cows}.");

            if (bulls == 4)
            {
                Console.WriteLine($"Congratulations! You guessed the word in {attempts} attempts.");
                isGuessed = true;
            }

        }
    }
}