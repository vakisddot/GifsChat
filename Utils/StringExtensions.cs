namespace GifsChat.Utils;

public static class StringExtensions
{
    public static bool IsUrl(this string str)
        => str.StartsWith("http://") || str.StartsWith("https://");

    public static bool IsValidGifUrl(this string str)
        => str.EndsWith(".gif") || str.EndsWith(".webp");
}