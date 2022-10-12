using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests;

public class ReentrantLockTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    static readonly object _locker = new object();

    public ReentrantLockTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    void 测试Monitor是可重入锁()
    {
        lock (_locker)
        {
            嵌套方法();
            // 这里依然拥有锁，因为锁是可重入的
            _testOutputHelper.WriteLine ("我也没有被阻塞");
        }
    }

    void 嵌套方法()
    {
        lock (_locker)
        {
            _testOutputHelper.WriteLine ("我没有被阻塞");
        }
    }
}