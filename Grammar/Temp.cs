namespace PegSharp.Grammar;

public class Temp(string? name = null) : GrammarBase
{
    public override string? Name => name;
    public override string Type => "temp";
    public GrammarBase? InstalledGrammar { get; private set; } = null;

    public override string? ContentDesc()
    {
        if (InstalledGrammar == null)
            return $"[temp:{Name}]";
        return InstalledGrammar.ContentDesc();
    }

    public void Install(GrammarBase grammar)
    {
        InstalledGrammar = grammar;
    }

    public override ISexpr? Parser(ILexer lexer)
    {
        if (InstalledGrammar == null)
            return null;
        return InstalledGrammar.Parser(lexer);
    }
}
