using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests;

public class SemaphoreTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(3, 3);

    public SemaphoreTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Theory]
    [InlineData(10)]
    void 循环测试SemaphoreSlim(int threadCnt)
    {
        for (int i = 0; i < threadCnt; i++)
        {
            进来玩(i);
        }
    }

    [Theory]
    [InlineData(10)]
    void 并行测试SemaphoreSlim(int threadCnt)
    {
        Parallel.For(0, threadCnt, 进来玩);
    }

    private async void 进来玩(int x)
    {
        _testOutputHelper.WriteLine(x + "想进来");
        await SemaphoreSlim.WaitAsync();
        _testOutputHelper.WriteLine(x + "进来了");
        await Task.Delay(1000 * x);
        _testOutputHelper.WriteLine(x + "溜了");
        SemaphoreSlim.Release();
    }
}