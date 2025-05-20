using System;
using System.Collections.Generic;

class EmployeePromotion
{
    private List<string> promotionList = new List<string>();

    public void CollectPromotionList()
    {
        Console.WriteLine("\nPlease enter the employee names in the order of their eligibility for promotion. (Enter blank to stop):");
        while (true)
        {
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                break;
            }
            promotionList.Add(name);
        }
    }

    public void DisplayPromotionList()
    {
        Console.WriteLine("\n--------------- Promotion List ------------------");
        if (promotionList.Count == 0)
        {
            Console.WriteLine("No employee names in the list.");
            return;
        }
        foreach (string name in promotionList)
        {
            Console.WriteLine(name);
        }
    }

    public void FindPromotionPosition()
    {
        if (promotionList.Count == 0)
        {
            Console.WriteLine("Promotion list is empty. Please collect names first.");
            return;
        }

        Console.Write("\nEnter the name of the employee to check promotion position: ");
        string name = Console.ReadLine();
        int index = promotionList.IndexOf(name);

        if (index != -1)
        {
            Console.WriteLine($"\n\"{name}\" is at position {index + 1} for promotion.");
        }
        else
        {
            Console.WriteLine($"\n\"{name}\" is not in the promotion list.");
        }
    }

    public void TrimExcessListSpace()
    {
        Console.WriteLine($"The current size of the collection is {promotionList.Capacity}.");
        promotionList.TrimExcess();
        Console.WriteLine($"The size after removing the extra space is {promotionList.Capacity}.");
    }

    public void DisplaySortedPromotedList()
    {
        if (promotionList.Count == 0)
        {
            Console.WriteLine("Promotion list is empty. Nothing to sort.");
            return;
        }

        promotionList.Sort();
        Console.WriteLine("\nPromoted employee list (Sorted): ");
        foreach (string name in promotionList)
        {
            Console.WriteLine(name);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        EmployeePromotion promotion = new EmployeePromotion();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\n=============== Employee Promotion Menu ===============");
            Console.WriteLine("1. Collect Promotion List");
            Console.WriteLine("2. Display Promotion List");
            Console.WriteLine("3. Find Employee Promotion Position");
            Console.WriteLine("4. Trim Excess List Space");
            Console.WriteLine("5. Display Sorted Promotion List");
            Console.WriteLine("0. Exit");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    promotion.CollectPromotionList();
                    break;
                case "2":
                    promotion.DisplayPromotionList();
                    break;
                case "3":
                    promotion.FindPromotionPosition();
                    break;
                case "4":
                    promotion.TrimExcessListSpace();
                    break;
                case "5":
                    promotion.DisplaySortedPromotedList();
                    break;
                case "0":
                    exit = true;
                    Console.WriteLine("Exiting the application...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please enter a number from 0 to 5.");
                    break;
            }
        }
    }
}
