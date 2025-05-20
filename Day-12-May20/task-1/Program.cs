/*
    1) Design a C# console app that uses a jagged array to store data for Instagram posts by multiple users. Each user can have a different number of posts, 
    and each post stores a caption and number of likes.

    You have N users, and each user can have M posts (varies per user).

    Each post has:

    A caption (string)

    A number of likes (int)

    Store this in a jagged array, where each index represents one user's list of posts.

    Display all posts grouped by user.

    No file/database needed — console input/output only.

    Example output
    Enter number of users: 2

    User 1: How many posts? 2
    Enter caption for post 1: Sunset at beach
    Enter likes: 150
    Enter caption for post 2: Coffee time
    Enter likes: 89

    User 2: How many posts? 1
    Enter caption for post 1: Hiking adventure
    Enter likes: 230

    --- Displaying Instagram Posts ---
    User 1:
    Post 1 - Caption: Sunset at beach | Likes: 150
    Post 2 - Caption: Coffee time | Likes: 89

    User 2:
    Post 1 - Caption: Hiking adventure | Likes: 230


    Test case
    | User | Number of Posts | Post Captions        | Likes      |
    | ---- | --------------- | -------------------- | ---------- |
    | 1    | 2               | "Lunch", "Road Trip" | 40, 120    |
    | 2    | 1               | "Workout"            | 75         |
    | 3    | 3               | "Book", "Tea", "Cat" | 30, 15, 60 |

*/

using System;

// [
//     [[caption,likes],[caption,likes]]  -- user1
//     [caption, likes] -- user1
// ]

class Post
{
    public string Caption { get; set; }
    public int Likes { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Instagram Posts.\n");
        Console.Write("Enter number of users : ");
        int users;
        while (!int.TryParse(Console.ReadLine(), out users))
        {
            Console.WriteLine("Enter a valid number of users.\n");
            Console.Write("Enter number of users : ");

        }
        Post[][] postsByUser = new Post[users][];

        for (int user = 0; user < users; user++)
        {
            Console.Write($"User {user + 1} : How many posts? : ");
            int userPostsCount;
            while (!int.TryParse(Console.ReadLine(), out userPostsCount))
            {
                Console.WriteLine($"Enter a valid number of posts for the User {user + 1}.\n");
                Console.Write($"User {user + 1} : How many posts? : ");

            }
            postsByUser[user] = new Post[userPostsCount];
            for (int i = 0; i < userPostsCount; i++)
            {
                Console.Write($"Enter caption for post {i + 1} : ");
                string caption = Console.ReadLine();
                Console.Write($"Enter Likes : ");
                int likes;
                while (!int.TryParse(Console.ReadLine(), out likes))
                {
                    Console.WriteLine("Enter a valid number of likes for the post.\n");
                    Console.Write($"Enter Likes : ");

                }
                postsByUser[user][i] = new Post { Caption = caption, Likes = likes };
            }

        }

        Console.WriteLine("\nDisplaying Posts :\n");

        for (int user = 0; user < users; user++)
        {
            Console.WriteLine($"User {user + 1} : ");
            for (int post = 0; post < postsByUser[user].Length; post++)
            {
                Console.WriteLine($"Post {post + 1} - Caption : {postsByUser[user][post].Caption} | Likes : {postsByUser[user][post].Likes}");
            }
            Console.WriteLine();
        }
    }
}