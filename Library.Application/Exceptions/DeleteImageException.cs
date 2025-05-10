namespace Library.Application.Exceptions;

public class DeleteImageException : Exception
{
    public DeleteImageException(string message) : base(message)
    {
    }
}