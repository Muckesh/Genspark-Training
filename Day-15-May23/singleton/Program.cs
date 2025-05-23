using System;

class Program
{
    static void Main()
    {
        var manager1 = FileManager.GetInstance();
        var manager2 = FileManager.GetInstance();

        if (manager1 == manager2)
        {
            Console.WriteLine("Both variables contain the same instance. Singleton works.");
        }else
        {
            Console.WriteLine("Variables contain the different instances. Singleton doesn't work.");
        }
        manager1.OpenFile("log.txt");
        

        manager1.WriteFile("First log entry");
        manager2.WriteFile("Second log entry");

        manager1.ReadFile();

        manager1.CloseFile();
    }
}

