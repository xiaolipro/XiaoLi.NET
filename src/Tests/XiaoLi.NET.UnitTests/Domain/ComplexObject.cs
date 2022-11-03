namespace XiaoLi.NET.UnitTests.Domain;

public class ComplexObject: IEquatable<ComplexObject>
{
    public ComplexObject(int a, string b)
    {
        A = a;
        B = b;
    }

    public int A { get; set; }

    public string B { get; set; }

    public override bool Equals(object obj)
    {
        return Equals(obj as ComplexObject);
    }

    public bool Equals(ComplexObject other)
    {
        return other != null &&
               A == other.A &&
               B == other.B;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(A, B);
    }
}