using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests;

public class MonitorTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private static readonly object _locker = new object();
    private static int _val1 = 1, _val2 = 1, num = 0;

    public MonitorTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    private void 测试Go(int threadNum)
    {
        num = 0;
        Parallel.For(0, threadNum, s => 线程不安全的Go());

        _testOutputHelper.WriteLine(num.ToString());

        num = 0;
        Parallel.For(0, threadNum, s => Monitor实现线程安全的Go2());

        _testOutputHelper.WriteLine(num.ToString());

        num = 0;
        Parallel.For(0, threadNum, s => Lock语法糖实现线程安全的Go());

        _testOutputHelper.WriteLine(num.ToString());
    }


    void 线程不安全的Go()
    {
        for (int i = 0; i < 10000; i++)
        {
            num++;
        }
    }

    void Monitor实现线程安全的Go()
    {
        for (int i = 0; i < 10000; i++)
        {
            // C# 3.0
            Monitor.Enter(_locker); // 20ns
            try
            {
                num++;
            }
            finally
            {
                Monitor.Exit(_locker);
            }
        }
    }

    void Lock语法糖实现线程安全的Go()
    {
        for (int i = 0; i < 10000; i++)
        {
            // 确保同一时间只有一个线程可以访问资源或代码
            // 对所有参与同步的线程可见的任何对象（必须为引用类型）都可以被当作同步对象使用
            lock (_locker) num++;
        }
    }

    #region lockTaken

    void Monitor实现线程安全的Go2()
    {
        for (int i = 0; i < 10000; i++)
        {
            bool taken = false;
            try
            {
                // JIT应该内联此方法，以便在典型情况下优化lockTaken参数的检查。请注意，要使VM允许内联，方法必须是透明的。
                Monitor.Enter(_locker,ref taken);
                num++;
            }
            finally
            {
                // C# 4.0 解决锁泄露问题
                if (taken) Monitor.Exit(_locker);
            }
        }
    }

    #endregion
}