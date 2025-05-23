public class File : IFile
{
    public void Read()
    {
        Console.WriteLine("[Access Granted] Reading sensitive file content...");
    }

    public void ReadMetadata()
    {
        Console.WriteLine("[Access Granted] Reading file metadata only...");
    }
}