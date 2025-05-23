// using System;
// using System.IO;

// public class FileManager
// {
//     private static readonly Lazy<FileManager> _instance = new Lazy<FileManager>(() => new FileManager());

//     private StreamWriter _writer;
//     private StreamReader _reader;
//     private FileStream _stream;
//     private readonly string _filePath = "data.txt";
//     private bool _isClosed = false;

//     private FileManager()
//     {
//         try
//         {
//             Console.WriteLine("Opening File");
//             _stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
//             _writer = new StreamWriter(_stream) { AutoFlush = true };
//             _reader = new StreamReader(_stream);
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine("Error initializing FileManager: " + ex.Message);
//             throw;
//         }
//     }

//     public static FileManager Instance => _instance.Value;

//     public void Write(string text)
//     {
//         try
//         {
//             if (_isClosed) throw new ObjectDisposedException("FileManager");
//             _writer.WriteLine(text);
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine("Error writing to file: " + ex.Message);
//         }
//     }

//     public void Read()
//     {
//         try
//         {
//             if (_isClosed) throw new ObjectDisposedException("FileManager");

//             _writer.Flush(); // Ensure all writes are saved
//             _stream.Seek(0, SeekOrigin.Begin); // Move to beginning of stream before reading

//             string line;
//             while ((line = _reader.ReadLine()) != null)
//             {
//                 Console.WriteLine(line);
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine("Error reading from file: " + ex.Message);
//         }
//     }

//     public void Close()
//     {
//         if (_isClosed) return;

//         try
//         {
//             Console.WriteLine("Closing file...");

//             _reader?.Dispose(); _reader = null;
//             _writer?.Dispose(); _writer = null;
//             _stream?.Dispose(); _stream = null;

//             _isClosed = true;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine("Error closing file: " + ex.Message);
//         }
//     }
// }

using System;
using System.IO;

public class FileManager
{
    private static readonly Lazy<FileManager> _instance = new Lazy<FileManager>(() => new FileManager());

    private StreamWriter _writer;
    private StreamReader _reader;
    private FileStream _stream;
    private readonly string _filePath = "data.txt";
    private bool _isClosed = false;

    private FileManager()
    {
        try
        {
            Console.WriteLine("Opening File");
            _stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            _writer = new StreamWriter(_stream) { AutoFlush = true };
            _reader = new StreamReader(_stream);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error initializing FileManager: " + ex.Message);
            throw;
        }
    }

    public static FileManager Instance => _instance.Value;

    public void Write(string text)
    {
        try
        {
            if (_isClosed) throw new ObjectDisposedException("FileManager");
            _writer.WriteLine(text);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error writing to file: " + ex.Message);
        }
    }

    public void Read()
    {
        try
        {
            if (_isClosed) throw new ObjectDisposedException("FileManager");

            _writer.Flush(); // Ensure all writes are saved
            _stream.Seek(0, SeekOrigin.Begin); // Move to beginning of stream before reading

            string line;
            while ((line = _reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading from file: " + ex.Message);
        }
    }

    public void Close()
    {
        if (_isClosed) return;

        try
        {
            Console.WriteLine("Closing file...");

            // _reader?.Dispose(); _reader = null;
            // _writer?.Dispose(); _writer = null;
            _stream?.Dispose(); _stream = null;

            _isClosed = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error closing file: " + ex.Message);
        }
    }
}
