using System.Reflection;
using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests;

public class 随便看看
{
    private readonly ITestOutputHelper _testOutputHelper;

    public 随便看看(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    void sadf()
    {
        var tail = "mutiarr".TrimStart("mutiarr".ToCharArray());
        _testOutputHelper.WriteLine(tail);
    }
    [Fact]
    void ddf()
    {
        new A();
    }
}

public class A
{
    public A()
    {
        Go();
    }
    public virtual void Go()
    {
        Console.WriteLine(123);
    }
}

public class B : A
{
    public override void Go()
    {
        Console.WriteLine(321);
    }
}