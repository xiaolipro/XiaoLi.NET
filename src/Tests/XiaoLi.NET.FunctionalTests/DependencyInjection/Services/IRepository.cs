using XiaoLi.NET.DependencyInjection.LifecycleInterfaces;

namespace XiaoLi.NET.FunctionalTests.DependencyInjection.Services;

public interface IRepository<T>
{
    int Count();
}

public class Repository<T> : IRepository<T>
{
    public int Count()
    {
        return typeof(T).Name.Length;
    }
}