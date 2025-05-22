public static class ExtensionFunctions
{
    public static bool StringValidationCheck(this string str)
    {
        if (str.Substring(0, 1).ToLower() == "s" && str.Length == 6)
        {
            return true;
        }
        return false;
    }
}