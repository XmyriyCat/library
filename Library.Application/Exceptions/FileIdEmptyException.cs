namespace Library.Application.Exceptions;

public class FileIdEmptyException : Exception
{
    public FileIdEmptyException(string message) : base(message)
    {
    }
}