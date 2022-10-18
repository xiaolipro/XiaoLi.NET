using System.Collections.Concurrent;
using Xunit.Abstractions;

namespace ThreadingTests.EventWaitHandles;

public class CountdownEventTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    CountdownEvent _countdown = new CountdownEvent (3);

    public CountdownEventTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    void 测试CountdownEvent等待()
    {
        Task.Run(工作);  // bg
        Task.Run(工作);
        Task.Run(工作);
        
        _countdown.Wait();
        _testOutputHelper.WriteLine("大家都干完了");
    }


    void 工作()
    {
        _testOutputHelper.WriteLine("干活");
        Thread.Sleep(1000);
        _countdown.Signal();//+1
    }




    #region 结构化并行构造
    [Fact]
    void 用Parallel实现等待()
    {
        Parallel.For(0, 3, x =>
        {
            工作();
        });
        
        _testOutputHelper.WriteLine("大家都干完了");
    }

    #endregion
}