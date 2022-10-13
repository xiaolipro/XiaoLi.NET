using System.Collections.Concurrent;
using Xunit.Abstractions;

namespace ThreadingTests.EventWaitHandles;

public class AutoResetEventTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public AutoResetEventTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    #region Base snip

    [Fact]
    void 基础测试()
    {
        EventWaitHandle waitHandle = new AutoResetEvent (false);
        Task.Factory.StartNew(() =>
        {
            _testOutputHelper.WriteLine("等待通知");
            waitHandle.WaitOne(); // 等待通知
            _testOutputHelper.WriteLine("接到通知");
            业务逻辑();
        });
        Thread.Sleep(1000);
        _testOutputHelper.WriteLine("发送通知");
        waitHandle.Set();
    }
    


    void 业务逻辑()
    {
        _testOutputHelper.WriteLine("干活干活");
    }
    
    
    [Fact]
    void 多线程并发通知()
    {
        EventWaitHandle waitHandle = new AutoResetEvent (false);
        Task.Factory.StartNew(() =>
        {
            _testOutputHelper.WriteLine("等待通知");
            waitHandle.WaitOne(); // 等待通知
            _testOutputHelper.WriteLine("接到通知");
            业务逻辑();
        });
        Thread.Sleep(1000);
        Parallel.For(0, 10, x =>
        {
            _testOutputHelper.WriteLine("发送通知");
            waitHandle.Set();
        });
    }

    [Fact]
    private async Task 默认已通知状态()
    {
        EventWaitHandle waitHandle = new AutoResetEvent (true);
        await Task.Run(() =>
        {
            _testOutputHelper.WriteLine("等待通知");
            waitHandle.WaitOne(); // 等待通知
            _testOutputHelper.WriteLine("接到通知");
            业务逻辑();
        });
    }
    
    [Fact]
    async Task 拉闸测试()
    {
        EventWaitHandle waitHandle = new AutoResetEvent (true);
        await Task.Run(() =>
        {
            _testOutputHelper.WriteLine("等待通知");
            waitHandle.WaitOne(); // 等待通知
            _testOutputHelper.WriteLine("接到通知");
            业务逻辑();
        });
        waitHandle.Reset();
    }
    
    [Fact]
    async Task 先发通知()
    {
        EventWaitHandle waitHandle = new AutoResetEvent (false);
        
        Parallel.For(0, 1, x =>
        {
            _testOutputHelper.WriteLine("发送通知");
            waitHandle.Set();
        });
        await Task.Factory.StartNew(() =>
        {
            _testOutputHelper.WriteLine("等待通知");
            waitHandle.WaitOne(); // 等待通知
            _testOutputHelper.WriteLine("接到通知");
            业务逻辑();
        });
    }

    #endregion

    #region 双向信号

    readonly EventWaitHandle _ready = new AutoResetEvent(false);
    readonly EventWaitHandle _entry = new AutoResetEvent(false);
    private readonly object _locker = new object();
    private string _message;
    [Fact]
    void 双向信号()
    {
        
        Task.Run(工作); // 这是一个工作线程

        Parallel.For(0, 10, x =>
        {
            _ready.WaitOne(); // 等待工作线程就绪
            lock (_locker)
            {
                _message = Thread.CurrentThread.ManagedThreadId.ToString();
            }

            _entry.Set(); // 通知工作线程开始工作
        });
        
        _ready.WaitOne();
        lock (_locker) _message = "exit";    // 发信号通知工作线程退出
        _entry.Set();
    }

    void 工作()
    {
        while (true)
        {
            _ready.Set();
            _entry.WaitOne();
            lock (_locker)
            {
                if (_message == "exit") return;
                _testOutputHelper.WriteLine("执行本次工作的线程id是："+_message);
            }
        }
    }

    #endregion
}