namespace Library.Application.Exceptions;

public class InvalidImageExtensionException : Exception
{
    public InvalidImageExtensionException(string message) : base(message)
    {
    }
}