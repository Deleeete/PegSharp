using PegSharp.Sexprs;

namespace PegSharp.Grammar;

public class Join : GrammarBase
{
    public override string? Name { get; }
    public override string Type => "join";
    public GrammarBase[] Sequence { get; }

    public Join(string? name = null, params GrammarBase[] elements)
    {
        Name = name;
        // 深度优化：展平后续的Join节点
        List<GrammarBase> flatSequence = [];
        foreach (var element in elements)
        {
            if (element is Join joinElement)
                flatSequence.AddRange(joinElement.Sequence);
            else
                flatSequence.Add(element);
        }
        Sequence = [.. flatSequence];
    }

    public override string? ContentDesc()
    {
        string elements = string.Join(", ", Sequence.Select(s => s.ContentDesc()));
        return $"[{elements}]";
    }

    public override ISexpr? Parser(ILexer lexer)
    {
        List<ISexpr> parsedGrammars = new(Sequence.Length);
        foreach (var grammar in Sequence)
        {
            var parsed = grammar.Parse(lexer);
            if (parsed != null)
                parsedGrammars.Add(parsed);
        }
        return (SexprList)parsedGrammars;
    }
}
