using PegSharp.Sexprs;
using System.Text.RegularExpressions;
using BclRegex = System.Text.RegularExpressions.Regex;

namespace PegSharp.Grammar;

public class Regex(BclRegex value, string? name = null) : GrammarBase
{
    public override string? Name => name;
    public override string Type => "regex";
    public BclRegex Value => value;

    public override string? ContentDesc()
        => $"literal \"{Value}\"";

    public override ISexpr? Parser(ILexer lexer)
    {
        var token = lexer.Read();
        if (!Value.IsMatch(token.Value))
            return null;
        return (StringSexpr)token.Value;
    }

    public static implicit operator string(Regex regexGrammar) => regexGrammar.Value.ToString();
    public static implicit operator Regex(string s) => new(new BclRegex(s, RegexOptions.Compiled));
}
