namespace Library.Api;

public class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Author
    {
        private const string Base = $"{ApiBase}/authors";

        public const string Create = Base;

        public const string Get = $"{Base}/{{id:guid}}";

        public const string GetAll = Base;
        
        public const string Update = $"{Base}/{{id:guid}}";
        
        public const string Delete = $"{Base}/{{id:guid}}";
    }
    
    public static class Book
    {
        private const string Base = $"{ApiBase}/book";

        public const string Create = Base;

        public const string Get = $"{Base}/{{idOrIsbn}}";

        public const string GetAll = Base;
        
        public const string Update = $"{Base}/{{id:guid}}";
        
        public const string Delete = $"{Base}/{{id:guid}}";
    }
}