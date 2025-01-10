namespace PegSharp;

public record struct Token(string Value, int Line)
{
    public static readonly Token EOF = new("$EOF", -1);
}
