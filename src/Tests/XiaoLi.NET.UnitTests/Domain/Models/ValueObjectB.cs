using XiaoLi.NET.Domain;

namespace XiaoLi.NET.UnitTests.Domain;

public class ValueObjectB: ValueObject
{
    public ValueObjectB(int a, string b, params int[] c)
    {
        A = a;
        B = b;
        C = c.ToList();
    }

    public int A { get; }
    public string B { get; }

    public List<int> C { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return A;
        yield return B;

        foreach (var c in C)
        {
            yield return c;
        }
    }
}