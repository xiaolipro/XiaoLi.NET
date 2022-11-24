using System.Dynamic;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests.Dynamics;

public class Normal
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Normal(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// 遍历ExpandoObject
    /// </summary>
    [Fact]
    public void GoThroughExpandoObject()
    {
        dynamic dynEO = new ExpandoObject();
        dynEO.number = 10;
        dynEO.Increment = new Action(() => { dynEO.number++; });

        Console.WriteLine(dynEO.number);
        dynEO.Increment();
        Console.WriteLine(dynEO.number);

        //dynEO.number 中number是动态添加属性。
        //dynEO.Increment 中Increment 是动态添加的Action 委托。

        //枚举ExpandoObject的所有成员：

        foreach (var property in (IDictionary<String, Object>)dynEO)
        {
            _testOutputHelper.WriteLine(property.Key + ": " + property.Value);
        }
    }

    /// <summary>
    /// 动态添加字段
    /// </summary>
    [Fact]
    public void DynamicObject()
    {
        var res = new List<Dictionary<string, object>>();
        var fields = new string[] { "appKey", "appToken" };

        var data = new List<object>()
        {
            new
            {
                MainBody = "123",
                FieldValues = "wqfdaqfwgrw$interface$lsddddddddddddddddddddrr",
                UpdateTime = DateTime.Now
            },
            new
            {
                MainBody = "321",
                FieldValues = "ffff$interface$lsddd",
                UpdateTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(213213))
            }
        };

        foreach (var item in data)
        {
            var properties = item.GetType().GetProperties();
            var dic = new Dictionary<string, object>();
            foreach (var propertyInfo in properties)
            {
                var val = propertyInfo.GetValue(item);
                if (propertyInfo.Name == "FieldValues")
                {
                    var values = val.ToString().Split("$interface$");
                    for (var i = 0; i < fields.Length; i++)
                    {
                        dic[fields[i]] = values[i];
                    }
                }
                else
                {
                    dic[propertyInfo.Name] = val;
                }
            }

            res.Add(dic);
        }

        _testOutputHelper.WriteLine(JsonConvert.SerializeObject(res));
    }
}