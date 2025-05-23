// // Creator (abstract/base class)
// //   └── FactoryMethod() → Product

// // ConcreteCreator (subclass)
// //   └── Overrides FactoryMethod() → returns a ConcreteProduct

// // Product (interface/base class)
// //   └── Common operations

// // ConcreteProductA / B (implementation classes)

// // Product interface

// public interface IFileManager
// {
//     void Write(string text);
//     void Read();
// }

// // concrete products

// public class TextFileManager : IFileManager
// {
//     private string _filePath = "txtfile.txt";

//     public void Write(string text)
//     {
//         File.AppendAllText(_filePath, text + Environment.NewLine);
//         Console.WriteLine("Written to text file.");
//     }

//     public void Read()
//     {
//         if (File.Exists(_filePath))
//         {
//             Console.WriteLine(File.ReadAllText(_filePath));
//         }
//         else
//         {
//             Console.WriteLine("Text file is empty.");
//         }
//     }
// }

// public class JsonFileManager : IFileManager
// {
//     public void Read()
//     {
//         throw new NotImplementedException();
//     }

//     public void Write(string text)
//     {
//         throw new NotImplementedException();
//     }
// }

// // creator or base class

// public abstract class FileManagerFactory
// {
//     public abstract IFileManager CreateFileManager();
// }

// // implement concrete factories

// public class TextFileManagerFactory : FileManagerFactory
// {
//     public override IFileManager CreateFileManager()
//     {
//         return new TextFileManager();
//     }
// }

// public class JsonFileManagerFactory : FileManagerFactory
// {
//     public override IFileManager CreateFileManager()
//     {
//         return new JsonFileManager();
//     }
// }

// // using factory in client code

// class Program
// {
//     static void Main(string[] args)
//     {
//         FileManagerFactory factory = new TextFileManagerFactory();
//     }
// }