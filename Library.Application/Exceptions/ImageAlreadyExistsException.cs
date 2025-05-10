namespace Library.Application.Exceptions;

public class ImageAlreadyExistsException : Exception
{
    public ImageAlreadyExistsException(string message) : base(message)
    {
    }
}