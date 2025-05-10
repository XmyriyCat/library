namespace Library.Application.Exceptions;

public class AuthorIdNotFoundException : Exception
{
    public AuthorIdNotFoundException(string message) : base(message)
    {
    }
}