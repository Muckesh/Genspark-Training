using System;
using System.IO;


public sealed class FileManager
{
    private static FileManager? _instance;
    private FileStream? _fileStream;
    private StreamReader? _reader;
    private StreamWriter? _writer;

    private FileManager() { }

    public static FileManager GetInstance()
    {
        if (_instance == null)
            _instance = new FileManager();
        return _instance;
    }

    public void OpenFile(string path)
    {
        _fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _reader = new StreamReader(_fileStream);
        _writer = new StreamWriter(_fileStream);
    }

    public void WriteFile(string line)
    {
        if (_writer == null)
        {
            Console.WriteLine("Error Opening file for writing.");
            return;
        }

        _writer.BaseStream.Seek(0, SeekOrigin.End); // Move to end
        _writer.WriteLine(line);
        _writer.Flush(); // Always flush!
        Console.WriteLine("Contents written successfully.");
    }

    public void ReadFile()
    {
        if (_reader == null)
        {
            Console.WriteLine("Error opening file for reading.");
            return;
        }

        _reader.BaseStream.Seek(0, SeekOrigin.Begin); // Go to start
        string? line;
        Console.WriteLine("Reading from file:");
        while ((line = _reader.ReadLine()) != null)
        {
            Console.WriteLine(line);
        }
    }

    public void CloseFile()
    {
        _writer?.Dispose();
        _reader?.Dispose();
        _fileStream?.Dispose();

        _writer = null;
        _reader = null;
        _fileStream = null;
    }
}
