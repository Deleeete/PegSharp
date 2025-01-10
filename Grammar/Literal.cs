using PegSharp.Sexprs;

namespace PegSharp.Grammar;

public class Literal(string value, string? name = null) : GrammarBase
{
    public override string? Name => name;
    public override string Type => "literal";
    public string Value => value;

    public override string? ContentDesc()
        => $"literal \"{Value}\"";

    public override ISexpr? Parser(ILexer lexer)
    {
        var token = lexer.Read();
        if (token == Token.EOF)
            return null;
        if (token.Value != Value)
                return null;
        return (StringSexpr)token.Value;
    }

    public static implicit operator string(Literal literalGrammar) => literalGrammar.Value;
    public static implicit operator Literal(string s) => new(s);
}
