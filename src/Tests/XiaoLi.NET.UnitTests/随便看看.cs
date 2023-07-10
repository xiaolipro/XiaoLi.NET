using System.Reflection;
using System.Security.Cryptography;
using HermaFx.Text;
using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests;

public class 随便看看
{
    private readonly ITestOutputHelper _testOutputHelper;

    public 随便看看(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    void sadf()
    {
        string timestamp = "1649823785567"; // 以字符串形式表示的时间戳
        string secretKey = "uYMGr8eU"; // 密钥
        string data = "{\"mailNoList\":[\"47234208672823\"]}"; // 需要加密的数据

        // 构造待加密字符串
        string toBeHashed = timestamp + secretKey + data;
        // 计算 MD5 值
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(toBeHashed);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // 将结果转换为十六进制字符串
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }

            _testOutputHelper.WriteLine(sb.ToString()); // 输出加密后的值
        }
    }

    [Fact]
    void f()
    {
        _testOutputHelper.WriteLine(typeof(JsonTest).FullName);
    }
    [Fact]
    void ddf()
    {
        byte[] randomBytes = new byte[11];

        RandomNumberGenerator.Create().GetBytes(randomBytes);
        randomBytes[0] = 0;
        randomBytes[1] = 0;
        _testOutputHelper.WriteLine(new Base32Encoder().Encode(randomBytes));
    }
}

public class A
{
    public A()
    {
        Go();
    }
    public virtual void Go()
    {
        Console.WriteLine(123);
    }
}

public class B : A
{
    public override void Go()
    {
        Console.WriteLine(321);
    }
}