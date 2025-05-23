// // The Singleton should always be a 'sealed' class to prevent class
// // inheritance through external classes and also through nested classes.
// public sealed class Singleton
// {
//     // Singleton constructor should always be private
//     // to prevent direct construction calls with 'new' operator

//     private Singleton()
//     {
//         Console.WriteLine("Singleton instance created.");
//     }

//     // Singleton's instance is stored in a static field

//     private static Singleton _instance;

//     // The static method that controls the access to singleton instance.
//     //  On the first run, it creates a singleton object and places
//     // it into the static field. On subsequent runs, it returns the client
//     // existing object stored in the static field.

//     public static Singleton GetInstance()
//     {
//         if (_instance == null)
//         {
//             _instance = new Singleton();
//         }
//         return _instance;
//     }

//     public void Greet()
//     {
//         Console.WriteLine("Hi. Singleton Method executed.");
//     }

// }

// class NaiveSingleton
// {
//     static void Main(string[] args)
//     {
//         Singleton s1 = Singleton.GetInstance();
//         Singleton s2 = Singleton.GetInstance();

//         s1.Greet();
//         s2.Greet();

//         if (s1 == s2)
//         {
//             Console.WriteLine("Singleton works, both variables contain the same instance.");
//         }else
//         {
//             Console.WriteLine("Singleton failed, variables contain different instances.");
//         }
        
//     }
// }