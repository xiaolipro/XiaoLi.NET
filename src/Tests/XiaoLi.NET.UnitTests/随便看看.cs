using System.Reflection;
using System.Security.Cryptography;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using HermaFx.Text;
using iTextSharp.text.pdf;
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
    void 去水印()
    {
        string sourcePdfPath = "D:\\Users\\23577\\Downloads\\212300708517111_212300708517111(1)(1).pdf"; // 源PDF文件路径
        string outputPdfPath = "D:\\Users\\23577\\Downloads\\去除水印.pdf"; // 输出无水印PDF文件路径

        using (PdfReader pdfReader = new PdfReader(sourcePdfPath))
        {
            using (PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(outputPdfPath, FileMode.Create)))
            {
                for (int pageIndex = 1; pageIndex <= pdfReader.NumberOfPages; pageIndex++)
                {
                    PdfContentByte pdfPageContents = pdfStamper.GetUnderContent(pageIndex);
                    pdfPageContents.SetLiteral("\b"); // 尝试清除页面内容
                }
            }
        }

        Console.WriteLine("Watermark removed.");
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
        var text1 = File.ReadAllText(@"D:\Users\23577\Downloads\tt.txt");
        var text2 = File.ReadAllText(@"D:\Users\23577\Downloads\tt2.txt");

        var diff = CompareTexts(text1, text2);

        _testOutputHelper.WriteLine(diff);
    }
    public string CompareTexts(string text1, string text2)
    {
        var differ = new Differ();
        var inlineBuilder = new InlineDiffBuilder(differ);
        var diffResult = inlineBuilder.BuildDiffModel(text1, text2);

        var diffOutput = new List<string>();

        foreach (var line in diffResult.Lines)
        {
            switch (line.Type)
            {
                case ChangeType.Inserted:
                    diffOutput.Add("+ " + line.Text);
                    break;
                case ChangeType.Deleted:
                    diffOutput.Add("- " + line.Text);
                    break;
                case ChangeType.Modified:
                    diffOutput.Add("* " + line.Text);
                    break;
                case ChangeType.Unchanged:
                    diffOutput.Add("  " + line.Text);
                    break;
                case ChangeType.Imaginary:
                    diffOutput.Add("? " + line.Text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return string.Join(Environment.NewLine, diffOutput);
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