namespace Library.Application.Exceptions;

public class DirectoryNotFoundException : Exception
{
    public DirectoryNotFoundException(string message) : base(message)
    {
    }
}