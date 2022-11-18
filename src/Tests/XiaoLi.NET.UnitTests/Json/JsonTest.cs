using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests;

public class JsonTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public JsonTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    void Seri()
    {
        var obj = new
        {
            a = 1,
            b = "str",
            c = true
        };

        var res = JObject.FromObject(obj);
        _testOutputHelper.WriteLine(JsonConvert.SerializeObject(res));
        _testOutputHelper.WriteLine(JsonConvert.SerializeObject(obj));
    }
    
    
    [Fact]
    public void Deseri()
    {
        var str = "{\"a\":1,\"B\":{\"a\":2,\"b\":[1,2,3]},\"C\":\"4\"}";

        var dic = JsonConvert.DeserializeObject<JObject>(str);
    }

    [Fact]
    public void Path()
    {
        var str = "{\"a\":1,\"B\":{\"a\":true,\"b\":[1,2,3]},\"C\":\"4\"}";

        var obj = JsonConvert.DeserializeObject<JObject>(str);
        
        foreach (var item in Jobject2Hash(obj))
        {
            _testOutputHelper.WriteLine(item.Key);
        }
        
        _testOutputHelper.WriteLine(obj.SelectToken("B.a").ToString());
        
        Assert.True(obj.SelectToken("B.a").ToString().Equals(Boolean.TrueString));
        Assert.True(obj.SelectToken("B.a").ToString().Equals("true", StringComparison.OrdinalIgnoreCase));
    }

    private Dictionary<string, JToken> Jobject2Hash(JObject jobject)
    {
        Dictionary<string, JToken> res = new();
        var q = new Queue<JProperty>(jobject.Properties());
        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            if (cur.Value is JObject obj)
            {
                foreach (var item in obj.Properties())
                {
                    q.Enqueue(item);
                }
            }
            else if (cur.Value is JArray arr)
            {
                Add(res, JArrayToHash(arr));
            }
            else
            {
                res.Add(cur.Path, cur.Value);
            }
        }

        return res;
    }


    Dictionary<string, JToken> JArrayToHash(JArray jarr)
    {
        Dictionary<string, JToken> res = new();
        //res.Add(jarr.Path, jarr.Count);
        for (var i = 0; i < jarr.Count; i++)
        {
            var jToken = jarr[i];
            if (jToken is JArray arr)
            {
                Add(res, JArrayToHash(arr));
            }
            else if (jToken is JObject obj)
            {
                Add(res, Jobject2Hash(obj));
            }
            else
            {
                if (jToken is JProperty prop)
                {
                    res.Add(jToken.Path /*.Replace("[0]","[*]")*/, prop.Value);
                }
                else if (jToken is JValue val)
                {
                    res.Add(jToken.Path /*.Replace("[0]","[*]")*/, val);
                }
                else
                {
                }
            }
        }

        return res;
    }
    
    void Add(Dictionary<string, JToken> a, Dictionary<string, JToken> b)
    {
        foreach (var item in b)
        {
            a.Add(item.Key, item.Value);
        }
    }
}