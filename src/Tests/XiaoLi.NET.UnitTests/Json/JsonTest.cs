using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests;

public class JsonTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    [Fact]
    void Seri()
    {
        //JObject jObject = 
    }
    
    
    [Fact]
    public void Deseri()
    {
        var str = "{\"a\":1,\"B\":{\"a\":2,\"b\":[1,2,3]},\"C\":\"4\"}";

        var dic = JsonConvert.DeserializeObject<JObject>(str);

        var res = dfs(dic.Properties(), 0);
        _testOutputHelper.WriteLine(JsonConvert.SerializeObject(res));
        Assert.Equal(5, nodes.Count);
        Assert.Equal(2,nodes[3].pid);
    }

    private List<(int id, int pid,string name)> nodes = new List<(int id, int pid,string name)>();
    private int idx = 1;

    public JsonTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public JObject dfs(IEnumerable<JProperty> properties, int pid)
    {
        JObject res = new JObject();
        foreach (var property in properties)
        {
            nodes.Add((idx ++, pid, property.Name));
            if (property.Value is JObject obj)
            {
                var jobject = dfs(obj.Properties(),idx - 1);
                res.Add(property.Name,JObject.FromObject(jobject));
            }
            else
            {
                res.Add(property.Name,property.Value);
            }
        }

        return res;
    }

    [Fact]
    public void bfs()
    {
        var str = "{\"A\":1,\"B\":{\"a\":2,\"b\":[1,2,3]},\"C\":\"4\"}";

        var dic = JsonConvert.DeserializeObject<JObject>(str);

        var q = new Queue<(JProperty property, int pid)>(dic.Properties().Select(x => (x, 0)));
        var nodes = new List<(int id, int pid,string name)>();
        int idx = 1;
        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            _testOutputHelper.WriteLine(cur.property.Path + " " + cur.property.Value);
            nodes.Add((idx ++, cur.pid, cur.property.Name));
            if (cur.property.Value is JObject obj)
            {
                foreach (var item in obj.Properties())
                {
                    q.Enqueue((item,idx - 1));
                }
            }
        }
        
        Assert.Equal(5, nodes.Count);
        Assert.Equal(2,nodes[3].pid);
        Assert.Equal(2,nodes[4].pid);
        Assert.Equal(0,nodes[2].pid);
    }
}