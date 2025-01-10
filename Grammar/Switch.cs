namespace PegSharp.Grammar;

public class Switch : GrammarBase
{
    public override string? Name { get; }
    public override string Type => "switch";
    public GrammarBase[] Options { get; }

    public Switch(string? name = null, params GrammarBase[] options)
    {
        Name = name;
        // 分支优化：自动展平分支
        List<GrammarBase> flatOptions = [];
        foreach (var option in options)
        {
            if (option is Switch switchOption)
                flatOptions.AddRange(switchOption.Options);
            else
                flatOptions.Add(option);
        }
        Options = [.. flatOptions];
    }

    public override string? ContentDesc()
    {
        string elements = string.Join("/ ", Options.Select(s => s.ContentDesc()));
        return $"[{elements}]";
    }

    public override ISexpr? Parser(ILexer lexer)
    {
        foreach (var grammar in Options)
        {
            var parsed = grammar.Parse(lexer);
            if (parsed != null)
                return parsed;
        }
        return null;
    }
}
