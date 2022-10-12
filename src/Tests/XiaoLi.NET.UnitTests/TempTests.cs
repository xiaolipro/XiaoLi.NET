using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests;

public class TempTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    public volatile static int num = 0;

    public TempTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    void volatile_test()
    {
        _testOutputHelper.WriteLine("current num is: " + num);
        
        Task.Run(() =>
        {
            while (num == 0)
            {
            }

            _testOutputHelper.WriteLine("ok");
        });

        Task.Run(async () =>
        {
            _testOutputHelper.WriteLine("change num..");
            await Task.Delay(2000);
            num++;
        });
        
        
        Thread.Sleep(5000);
    }
}