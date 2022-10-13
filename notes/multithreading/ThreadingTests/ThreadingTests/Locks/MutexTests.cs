using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests.Locks;
public class MutexTests:ContextBoundObject
{
    private readonly ITestOutputHelper _testOutputHelper;
    public MutexTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    void 测试Mutex互斥体()
    {
        // 命名的 Mutex 是机器范围的，它的名称需要是唯一的
        // 比如使用公司名+程序名，或者也可以用 URL
        using var mutex = new Mutex (false, "LOL");
        // 可能其它程序实例正在关闭，所以可以等待几秒来让其它实例完成关闭

        if (!mutex.WaitOne (TimeSpan.FromSeconds (3), false))
        {
            _testOutputHelper.WriteLine ("客户端正在运行");
            return;
        }
        RunProgram();
    }
    
    void RunProgram()
    {
        _testOutputHelper.WriteLine ("客户端启动中");
    }
}