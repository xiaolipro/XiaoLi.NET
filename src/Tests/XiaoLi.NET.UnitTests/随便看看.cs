using System.Reflection;

namespace XiaoLi.NET.UnitTests;

public class 随便看看
{
    [Fact]
    void hh()
    {
        var flow = Assembly.GetAssembly(typeof(IA))!.GetTypes().Where(x => x.IsPublic).Where(x =>
        {
            if (!x.IsClass) return false;
            if (x.IsAbstract) return false;
            return typeof(IA).IsAssignableFrom(x);
        }).FirstOrDefault(x => x.Name.Equals("B"));
        if (flow == default) new A().Go();
        else (Activator.CreateInstance(flow) as IA)!.Go();
    }
}

public interface IA
{
    void Go();
}

public class A : IA
{
    public void Go()
    {
        Console.WriteLine(123);
    }
}

public class B : IA
{
    public void Go()
    {
        Console.WriteLine(321);
    }
}