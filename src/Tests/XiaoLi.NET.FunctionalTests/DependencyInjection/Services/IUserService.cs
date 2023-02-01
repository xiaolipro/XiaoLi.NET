using XiaoLi.NET.DependencyInjection.Attributes;
using XiaoLi.NET.DependencyInjection.LifecycleInterfaces;

namespace XiaoLi.NET.FunctionalTests.DependencyInjection.Services;

public interface IUserService
{
    void Increment();
    int GetV();
    Task<User> GetUserById(int userId);
}

public class UserService : IUserService
{
    private int _num;
    public void Increment()
    {
        _num++;
    }

    public int GetV()
    {
        return _num;
    }

    public async Task<User> GetUserById(int userId)
    {
        await Task.Yield();
        return new User()
        {
            Id = userId,
            Name = "用户"+userId,
            Age = new Random(DateTime.Now.Millisecond).Next(18, 35)
        };
    }
}

public class CustomUserService : IUserService
{
    private int _num;
    public void Increment()
    {
        _num++;
    }

    public int GetV()
    {
        return _num;
    }

    public async Task<User> GetUserById(int userId)
    {
        await Task.Yield();
        return new User()
        {
            Id = userId,
            Name = "Custom用户"+userId,
            Age = new Random(DateTime.Now.Millisecond).Next(18, 35)
        };
    }
}

public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }
}