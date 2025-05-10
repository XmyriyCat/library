namespace Library.Data.Exceptions;

public class CopyFailingException : Exception
{
    public CopyFailingException(string message, Exception ex) : base(message, ex)
    {
    }
}