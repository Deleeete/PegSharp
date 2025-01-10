using PegSharp.Sexprs;

namespace PegSharp.Grammar;

public class Stack(GrammarBase element, string? name = null) : GrammarBase
{
    public override string? Name => name;
    public override string Type => "stack";
    public GrammarBase Element => element;

    public override string? ContentDesc()
    {
        return $"[{Type}:{Element}]";
    }

    public override ISexpr? Parser(ILexer lexer)
    {
        List<ISexpr> parsedSexpr = [];
        while (true)
        {
            var parsed = Element.Parse(lexer);
            if (parsed == null)
                return (SexprList)parsedSexpr;
            parsedSexpr.Add(parsed);
        }
    }
}
