## 基本概念

https://www.albahari.com/threading/

**并行（parallel）**：同一时间，多个线程/进程同时执行。多线程的目的就是为了并行，充分利用cpu多个核心，提高程序性能

**线程（thread）**：线程是操作系统能够进行 **运算调度的最小单位**，是进程的实际运作单位。
一条线程指的是进程中一个单一顺序的控制流，一个进程中可以并行多个线程，每条线程并行执行不同的任务。

**进程（process）**：进程是操作系统进行**资源分配的基本单位**。多个进程并行的在计算机上执行，多个线程并行的在进程中执行，
进程之间是隔离的，线程之间共享堆，私有栈空间。

CLR 为每个线程分配各自独立的 **栈（stack）** 空间，因此局部变量是线程独立的。

```c#
static void Main()
{
  new Thread (Go).Start();      // 在新线程执行Go()
  Go();                         // 在主线程执行Go()
}

static void Go()
{
  // 定义和使用局部变量 - 'cycles'
  for (int cycles = 0; cycles < 5; cycles++) Console.Write ('?');
}
```
变量cycles的副本是分别在线程各自的栈中创建，因此会输出 10 个问号
```angular2html
??????????
```

线程可以通过对同一对象的引用来共享数据。例如：


```c#
static bool done;

static void Main()
{
  new Thread (tt.Go).Start();
  Go();
}

static void Go()
{
   if (!done) { 
      Console.WriteLine ("Done");
      done = true;
   }
}
```
这个例子引出了一个关键 **概念线程安全（thread safety）** ，由于并发，” Done “ 有可能会被打印两次

通过简单的加锁操作：在读写公共字段时，获得一个 **排它锁（互斥锁，exclusive lock ）** ，c#中使用lock即可生成 **临界区（critical section）** 
```c#
static readonly object locker = new object();
...
static void Go()
{
  lock (locker)
  {
    if (!done) { 
      Console.WriteLine ("Done");
      done = true;
    }
  }
}
```



**临界区（critical section）**：在同一时刻只有一个线程能进入，不允许并发。当有线程进入临界区段时，其他试图进入的线程或是进程必须 **等待或阻塞（blocking）**

**线程阻塞（blocking）**：指一个线程在执行过程中暂停，以等待某个条件的触发来解除暂停。阻塞状态的线程不会消耗CPU资源

可以通过调用Join方法等待线程执行结束，例如：
```c#
static void Main()
{
  Thread t = new Thread(Go);
  t.Start();
  t.Join();  // 等待线程 t 执行完毕
  Console.WriteLine ("Thread t has ended!");
}

static void Go()
{
  for (int i = 0; i < 1000; i++) Console.Write ("y");
}
```

也可以使用Sleep使当前线程阻塞一段时间：
```c#
Thread.Sleep (500);  // 阻塞 500 毫秒
```

> Thread.Sleep(0)会立即释放当前的时间片，将 CPU 资源出让给其它线程。Framework 4.0的Thread.Yield()方法与其大致相同，不同的是Yield()只会出让给运行在相同处理器核心上的其它线程。
>
> Sleep(0)和Yield在调整代码性能时偶尔有用，它也是一个很好的诊断工具，可以用于找出线程安全（thread safety）的问题。如果在你代码的任意位置插入Thread.Yield()会影响到程序，
> 基本可以确定存在 bug。


## 工作原理

### 硬件结构
https://xiaolincoding.com/os/1_hardware/how_cpu_run.html#%E5%9B%BE%E7%81%B5%E6%9C%BA%E7%9A%84%E5%B7%A5%E4%BD%9C%E6%96%B9%E5%BC%8F


### 运行时

&emsp;&emsp;线程在内部由一个 **线程调度器（thread scheduler）** 管理，一般 CLR 会把这个任务交给操作系统完成。线程调度器确保所有活动的线程能够分配到适当的执行时间，
并且保证那些处于等待或阻塞状态（例如，等待排它锁或者用户输入）的线程不消耗CPU时间。

&emsp;&emsp;在单核计算机上，线程调度器会进行 **时间切片（time-slicing）** ，快速的在活动线程中切换执行。在 Windows 操作系统上，一个时间片通常在十几毫秒（译者注：默认 15.625ms），远大于 CPU 在线程间进行上下文切换的开销（通常在几微秒区间）。

&emsp;&emsp;在多核计算机上，多线程的实现是混合了时间切片和 **真实的并发（genuine concurrency）** ，不同的线程同时运行在不同的 CPU 核心上。仍然会使用到时间切片，因为操作系统除了要调度其它的应用，还需要调度自身的线程。

&emsp;&emsp;线程的执行由于外部因素（比如时间切片）被中断称为 **被抢占（preempted）**。在大多数情况下，线程无法控制其在什么时间，什么代码块被抢占。


> &emsp;&emsp;多线程同样也会带来缺点，最大的问题在于它提高了程序的复杂度。使用多个线程本身并不复杂，复杂的是线程间的交互（共享数据）如何保证安全。无论线程间的交互是否有意为之，都会带来较长的开发周期，以及带来间歇的、难以重现的 bug。因此，最好保证线程间的交互尽可能少，并坚持简单和已被证明的多线程交互设计。
> 
> &emsp;&emsp;当频繁地调度和切换线程时（且活动线程数量大于 CPU 核心数），多线程会增加系统资源和 CPU 的开销，线程的创建和销毁也会增加开销。多线程并不总是能提升程序的运行速度，如果使用不当，反而可能降低速度。


## 基础用法







