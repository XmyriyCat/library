namespace Library.Data.Exceptions;

public class DbPageException : Exception
{
    public DbPageException(string message) : base(message)
    {
    }
}