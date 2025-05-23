class ProxyFile : IFile
{
    private readonly User _user;

    private readonly File _realFile;

    public ProxyFile(User user)
    {
        _user = user;
        _realFile = new File();
    }

    public void Read()
    {
        switch (_user.UserRole)
        {
            case Role.Admin:
                _realFile.Read();
                break;
            case Role.User:
                _realFile.ReadMetadata();
                break;
            case Role.Guest:
                Console.WriteLine("[Access Denied] You do not have permission to read this file.");
                break;
            
        }
    }
}