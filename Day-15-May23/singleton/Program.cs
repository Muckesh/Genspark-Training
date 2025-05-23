// using System;

// class Program
// {
//     static void Main(string[] args)
//     {
//         FileManager fileManager = null;

//         try
//         {
//             fileManager = FileManager.Instance;
//             string choice;

//             do
//             {
//                 Console.WriteLine("\n--------------- File Manager --------------------");
//                 Console.WriteLine("1. Write to file");
//                 Console.WriteLine("2. Read from the file");
//                 Console.WriteLine("3. Exit");
//                 Console.Write("Enter your choice : ");
//                 choice = Console.ReadLine();

//                 switch (choice)
//                 {
//                     case "1":
//                         Console.Write("Enter a line of text to add to the file: ");
//                         string line = Console.ReadLine();
//                         if (!string.IsNullOrWhiteSpace(line))
//                         {
//                             fileManager.Write(line);
//                             Console.WriteLine("Text written successfully.");
//                         }
//                         else
//                         {
//                             Console.WriteLine("Input was empty. Nothing written.");
//                         }
//                         break;

//                     case "2":
//                         Console.WriteLine("\nReading from file:");
//                         fileManager.Read();
//                         break;

//                     case "3":
//                         Console.WriteLine("Exiting...");
//                         break;

//                     default:
//                         Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
//                         break;
//                 }

//             } while (choice != "3");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine("Unexpected error: " + ex.Message);
//         }
//         finally
//         {
//             fileManager?.Close();  // Ensure file is always closed
//         }
//     }
// }

using System;

class Program
{
    static void Main(string[] args)
    {
        FileManager fileManager = null;

        try
        {
            fileManager = FileManager.Instance;
            string choice;

            do
            {
                Console.WriteLine("\n--------------- File Manager --------------------");
                Console.WriteLine("1. Write to file");
                Console.WriteLine("2. Read from the file");
                Console.WriteLine("3. Exit");
                Console.Write("Enter your choice : ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter a line of text to add to the file: ");
                        string line = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            fileManager.Write(line);
                            Console.WriteLine("Text written successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Input was empty. Nothing written.");
                        }
                        break;

                    case "2":
                        Console.WriteLine("\nReading from file:");
                        fileManager.Read();
                        break;

                    case "3":
                        Console.WriteLine("Exiting...");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                        break;
                }

            } while (choice != "3");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error: " + ex.Message);
        }
        finally
        {
            fileManager?.Close();  // Ensure file is always closed
        }
    }
}
