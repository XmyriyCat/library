namespace Library.Application.Variables;

public static class SupportedImageExtensions
{
    public static readonly string[] AllowedExtensions =
    [
        ".jpg", ".jpeg", ".png"
    ];

    public static string GetMimeType(string filePath)
    {
        return Path.GetExtension(filePath).ToLowerInvariant() switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}