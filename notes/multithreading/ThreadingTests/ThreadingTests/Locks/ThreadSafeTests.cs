using System.Collections.Concurrent;
using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests;

public class ThreadSafeTests
{
    static List<string> _list = new List <string>();
    private readonly ITestOutputHelper _testOutputHelper;

    public ThreadSafeTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    void 测试线程安全()
    {
        new Thread (AddItem).Start();
        new Thread (AddItem).Start();
    }
    
    void AddItem()
    {
        lock (_list) _list.Add ("Item " + _list.Count);

        // TODO: 这里能不能优化？
        lock (_list)
        {
            foreach (string item in _list)
            {
                _testOutputHelper.WriteLine (item);
            }
        }
        
    }
}