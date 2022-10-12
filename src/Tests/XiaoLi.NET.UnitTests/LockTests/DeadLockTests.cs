namespace XiaoLi.NET.UnitTests;

public class DeadLockTests
{
    [Fact]
    void 测试死锁()
    {
        object locker1 = new object();
        object locker2 = new object();

        new Thread(() =>
        {
            lock (locker1)
            {
                Thread.Sleep(1000);
                lock (locker2)  // 死锁
                {
                    // do something..
                }
            }
        }).Start();
        
        lock (locker2)
        {
            Thread.Sleep(1000);
            lock (locker1)  // 死锁
            {
                // do something..
            }
        }
    }
}