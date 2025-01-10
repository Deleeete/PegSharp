using System.Text;
using PegSharp.Sexprs;

namespace PegSharp;

public interface ISexpr;
public interface ISexprAtom : ISexpr;
public interface ISexprList : IList<ISexpr>, ISexpr;

public static class SexprExtensions
{
    private const string NullLiteral = "<null>";

    public static string AsString(this ISexprAtom self)
    {
        if (self is StringSexpr strSexpr)
            return $"\"{strSexpr.Value}\"";
        else if (self is IntSexpr intSexpr)
            return intSexpr.Value.ToString();
        var ret = self.ToString();
        if (ret == null)
            return NullLiteral;
        return ret;
    }

    public static string AsString(this ISexprList self)
    {
        StringBuilder sb = new("(");
        foreach (var node in self)
        {
            sb.Append(node == null ? NullLiteral : node.AsString());
            sb.Append(", ");
        }
        if (sb.Length > 1)
            sb.Length -= 2;
        sb.Append(')');
        return sb.ToString();
    }

    public static string AsString(this ISexpr self)
    {
        if (self is ISexprAtom atom)
            return atom.AsString();
        else if (self is ISexprList list)
            return list.AsString();
        else
            throw new Exception($"Unexpected sexpr type {self.GetType()}");
    }
    /// <summary>
    /// 塌缩嵌套S表达式，去除单叶层级
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static ISexpr Collapse(this ISexpr self, bool recursive = false)
    {
        while (self is ISexprList sexprList)
        {
            if (recursive)
            {
                for (int i = 0; i < sexprList.Count; i++)
                {
                    var currentNode = sexprList[i];
                    if (currentNode == null)
                        continue;
                    sexprList[i] = currentNode.Collapse(true);
                }
            }
            if (sexprList.Count > 1)
                self = sexprList[0];
        }
        return self;
    }
}
