using System.Numerics;
using PegSharp.Grammar;

namespace PegSharp;

public interface IGrammar
{
    string? Name { get; }
    string Type { get; }
    bool Suppressed { get; }
    bool Collapsed { get; }
    string? ContentDesc();
    ISexpr? Parser(ILexer lexer);
}

// 由于隐式类型转换和操作符实现不支持接口，很遗憾这个基类目前（C#11）是必需的
public abstract class GrammarBase :
    IGrammar,
    IDivisionOperators<GrammarBase, GrammarBase, Switch>,
    ISubtractionOperators<GrammarBase, GrammarBase, Join>,
    IUnaryPlusOperators<GrammarBase, Stack>
{
    public abstract string? Name { get; }
    public abstract string Type { get; }
    public bool Suppressed { get; }
    public bool Collapsed { get; }

    public abstract string? ContentDesc();
    public abstract ISexpr? Parser(ILexer lexer);

    // 醋
    public static implicit operator GrammarBase(string s) => new Literal(s);
    public static Switch operator /(GrammarBase left, GrammarBase right)
        => new(null, left, right);

    public static Join operator -(GrammarBase left, GrammarBase right)
        => new(null, left, right);

    public static Stack operator +(GrammarBase value)
        => new(value, null);
}

public static class GrammarExtensions
{
    public static ISexpr? Parse(this IGrammar self, ILexer lexer)
    {
        var parsed = self.Parser(lexer);
        if (self.Suppressed)
            return null;
        if (parsed != null && self.Collapsed)
            parsed = parsed.Collapse();
        return parsed;
    }
}
