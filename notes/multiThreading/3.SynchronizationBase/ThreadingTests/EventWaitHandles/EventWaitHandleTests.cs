using System.Collections.Concurrent;
using Xunit.Abstractions;

namespace ThreadingTests.EventWaitHandles;

public class EventWaitHandleTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public EventWaitHandleTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    #region ManualResetEvent

    [Fact]
    void 测试ManualResetEvent()
    {
        //var waitHandle = new ManualResetEvent(false);
        var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        Task.Run(() =>
        {
            _testOutputHelper.WriteLine(Thread.CurrentThread.ManagedThreadId + " 尝试进门...");
            waitHandle.WaitOne();
            _testOutputHelper.WriteLine(Thread.CurrentThread.ManagedThreadId + " 进去了");
            业务逻辑();
            _testOutputHelper.WriteLine("当前门的状态是开启的吗？"+waitHandle.WaitOne(0));
        });
        Thread.Sleep(1000);
        _testOutputHelper.WriteLine(Thread.CurrentThread.ManagedThreadId + " say：我来开门");
        waitHandle.Set();
    }

    #endregion

    #region AutoResetEvent

    [Fact]
    void 测试AutoResetEvent()
    {
        EventWaitHandle waitHandle = new AutoResetEvent(false);
        // var waitHandle2 = new EventWaitHandle(false, EventResetMode.AutoReset);
        Task.Factory.StartNew(() =>
        {
            _testOutputHelper.WriteLine(Thread.CurrentThread.ManagedThreadId + " 尝试进门...");
            waitHandle.WaitOne();
            _testOutputHelper.WriteLine(Thread.CurrentThread.ManagedThreadId + " 进去了");
            业务逻辑();
            // _testOutputHelper.WriteLine("当前门的状态是开启的吗？"+waitHandle.WaitOne(0));
            // waitHandle.Set();
            // Task.Run(() =>
            // {
            //     waitHandle.WaitOne();
            //     _testOutputHelper.WriteLine(Thread.CurrentThread.ManagedThreadId + " 进去了");
            //     业务逻辑();
            // });
        });
        Thread.Sleep(1000);
        _testOutputHelper.WriteLine(Thread.CurrentThread.ManagedThreadId + " say：我来开门");
        waitHandle.Set();
        
        
        Thread.Sleep(1000);
    }


    void 业务逻辑()
    {
        _testOutputHelper.WriteLine(Thread.CurrentThread.ManagedThreadId + " 开始干活！");
    }


    [Fact]
    void 多线程并发通知()
    {
        EventWaitHandle waitHandle = new AutoResetEvent(false);
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
        EventWaitHandle waitHandle = new AutoResetEvent(true);
        await Task.Run(() =>
        {
            _testOutputHelper.WriteLine(Thread.CurrentThread.ManagedThreadId + " 尝试进门...");
            waitHandle.WaitOne();
            _testOutputHelper.WriteLine(Thread.CurrentThread.ManagedThreadId + " 进去了");
            业务逻辑();
            _testOutputHelper.WriteLine("当前门的状态是开启的吗？"+waitHandle.WaitOne(0));
        });
    }

    [Fact]
    async Task 拉闸测试()
    {
        EventWaitHandle waitHandle = new AutoResetEvent(true);
        await Task.Run(() =>
        {
            _testOutputHelper.WriteLine("等待通知");
            waitHandle.WaitOne(); // 等待通知
            _testOutputHelper.WriteLine("接到通知");
            业务逻辑();
        });
        waitHandle.Reset();waitHandle.Reset();waitHandle.Reset();waitHandle.Reset();waitHandle.Reset();
    }

    [Fact]
    async Task 先发通知()
    {
        EventWaitHandle waitHandle = new AutoResetEvent(false);

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
        lock (_locker) _message = "exit"; // 发信号通知工作线程退出
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
                _testOutputHelper.WriteLine("执行本次工作的线程id是：" + _message);
            }
        }
    }

    #endregion

}