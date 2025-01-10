namespace PegSharp.Sexprs;

public readonly struct StringSexpr(string value) : ISexprAtom
{
    public string Value { get; } = value;
    public static implicit operator string(StringSexpr strSexpr) => strSexpr.Value;
    public static implicit operator StringSexpr(string s) => new(s);
    //public static explicit operator string(StringSexpr strSexpr) => strSexpr.Value;
    //public static explicit operator StringSexpr(string s) => new(s);
}

public readonly struct IntSexpr(long value) : ISexprAtom
{
    public long Value { get; } = value;
    public static implicit operator long(IntSexpr intSexpr) => intSexpr.Value;
    public static implicit operator IntSexpr(int val) => new(val);
}
