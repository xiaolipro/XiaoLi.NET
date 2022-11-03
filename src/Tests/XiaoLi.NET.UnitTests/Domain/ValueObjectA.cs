using XiaoLi.NET.Domain;

namespace XiaoLi.NET.UnitTests.Domain;

public class ValueObjectA: ValueObject
{
    public ValueObjectA(int a, string b, Guid c, ComplexObject d, string notAnEqualityComponent = null)
    {
        A = a;
        B = b;
        C = c;
        D = d;
        NotAnEqualityComponent = notAnEqualityComponent;
    }

    public int A { get; }
    public string B { get; }
    public Guid C { get; }
    public ComplexObject D { get; }
    public string NotAnEqualityComponent { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return A;
        yield return B;
        yield return C;
        yield return D;
    }
}