using System.Collections;

namespace PegSharp.Sexprs;

public class SexprList : ISexprList
{
    public List<ISexpr> Elements { get; init; }

    public SexprList(params ISexpr?[] elements)
    {
        Elements = [..elements];
    }
    public SexprList(List<ISexpr> elements)
    {
        Elements = elements;
    }

    public static implicit operator List<ISexpr>(SexprList strSexpr) => strSexpr.Elements;
    public static implicit operator SexprList(List<ISexpr> elements) => new(elements);

    public ISexpr this[int index] 
    {
        get => Elements[index];
        set => Elements[index] = value;
    }

    public int Count => Elements.Count;
    public bool IsReadOnly { get; }

    public void Add(ISexpr item)
        => Elements.Add(item);

    public void Clear()
        => Elements.Clear();

    public bool Contains(ISexpr item)
        => Elements.Contains(item);

    public void CopyTo(ISexpr[] array, int arrayIndex)
        => Elements.CopyTo(array, arrayIndex);

    public IEnumerator<ISexpr> GetEnumerator()
        => Elements.GetEnumerator();

    public int IndexOf(ISexpr item)
        => Elements.IndexOf(item);

    public void Insert(int index, ISexpr item)
        => Elements.Insert(index, item);

    public bool Remove(ISexpr item)
        => Elements.Remove(item);

    public void RemoveAt(int index)
        => Elements.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator()
        => Elements.GetEnumerator();
}
