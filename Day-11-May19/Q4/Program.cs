/*
    4) Take username and password from user. Check if user name is "Admin" and password is "pass" if yes then print success message.
    Give 3 attempts to user. In the end of eh 3rd attempt if user still is unable to provide valid creds then exit the application after print the message 
    "Invalid attempts for 3 times. Exiting...."
*/
using System;

class Program {
    static void Main(string[] args)
    {
        int attempts = 0;
        int allowedAttempts = 3;
        string validUsername = "Admin";
        string validPassword = "pass";
        bool isAuthenticated = false;

        while (attempts < allowedAttempts)
        {
            Console.Write("Enter username : ");
            string username = Console.ReadLine();

            Console.Write("Enter password : ");
            string password = Console.ReadLine();

            if (username == validUsername && password == validPassword)
            {
                Console.WriteLine($"Login Successful. Welcome {validUsername}.");
                isAuthenticated = true;
                break;
            }
            else
            {
                attempts++;
                Console.WriteLine($"Invalid credentials. Attempt {attempts} of {allowedAttempts}");
            }
        }

        if (!isAuthenticated)
        {
            Console.WriteLine($"Invalid attempts of {attempts} times. Exiting.....");
        }
    }
}