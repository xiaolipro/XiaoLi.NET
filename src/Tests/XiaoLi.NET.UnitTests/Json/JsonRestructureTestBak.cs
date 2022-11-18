using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests;

public class JsonRestructureTestBak
{
    private readonly ITestOutputHelper _testOutputHelper;

    public JsonRestructureTestBak(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    void Core()
    {
        var str =
            "{\"A\":1,\"B\":{\"a\":2,\"b\":[{\"a\":101,\"b\":\"b1\",\"c\":[1,2,3]},{\"a\":102,\"b\":\"b2\"},{\"a\":103,\"b\":\"b3\"}]},\"C\":[\"4\",\"3\",\"2\",\"1\"],\"D\":[{\"a\":1,\"b\":1},{\"a\":2,\"b\":2}],\"E\":[{\"x\":\"x1\",\"y\":\"y1\"},{\"x\":\"x2\",\"y\":\"y2\"},{\"x\":\"x3\",\"y\":\"y4\"}]}";
        var hash = Jobject2Hash(JObject.Parse(str));
        foreach (var item in hash)
        {
            _testOutputHelper.WriteLine(item.Key + "=" + item.Value);
        }

        _testOutputHelper.WriteLine("==============");

        List<Parameter> list = new()
        {
            new()
            {
                Id = 1,
                PId = 0,
                Name = "_A",
                Type = (int)ParameterType.String,
                Alias = "_A",
                MapAlias = "A"
            },
            new()
            {
                Id = 2,
                PId = 0,
                Name = "_B",
                Alias = "_B",
                Type = (int)ParameterType.Array,
            },
            new()
            {
                Id = 3,
                PId = 2,
                Name = "_C",
                Alias = "_B[*]",
                Type = (int)ParameterType.Object,
            },
            new()
            {
                Id = 4,
                PId = 3,
                Name = "_D",
                Alias = "_B[*]._D",
                Type = (int)ParameterType.Array,
            },
            new()
            {
                Id = 5,
                PId = 4,
                Name = "_E",
                Alias = "_B[*]._D[*]",
                Type = (int)ParameterType.String,
                MapAlias = "C[*]" // 叶子
            },
            new()
            {
                Id = 6,
                PId = 3,
                Name = "_F",
                Alias = "_B[*]._F",
                Type = (int)ParameterType.String,
                MapAlias = "B.b[*].b"
            },
            new()
            {
                Id = 7,
                PId = 0,
                Name = "_G",
                Alias = "_K",
                Type = (int)ParameterType.Array,
            },
            new()
            {
                Id = 8,
                PId = 7,
                Name = "_gg",
                Alias = "_K[*]",
                Type = (int)ParameterType.Object,
            },
            new()
            {
                Id = 9,
                PId = 8,
                Name = "_X",
                Alias = "_K[*]._X",
                Type = (int)ParameterType.Number,
                MapAlias = "D[*].a"
            },
            new()
            {
                Id = 10,
                PId = 8,
                Name = "_Y",
                Alias = "_K[*]._Y",
                Type = (int)ParameterType.String,
                MapAlias = "E[*].x"
            },
        };

        List<ParameterNode> treeList = BuildTreeList(list, 0);

        var res = JsonConvert.SerializeObject(dfs_object(treeList, hash, 0));
        _testOutputHelper.WriteLine(res);
    }

    private List<ParameterNode> BuildTreeList(List<Parameter> list,
        int pid)
    {
        var res = new List<ParameterNode>();
        foreach (var child in list.Where(x => x.PId == pid))
        {
            var node = new ParameterNode();
            node.Name = child.Name;
            node.Type = child.Type;
            node.Alias = child.Alias;
            if (node.Type is (int)ParameterType.Object or (int)ParameterType.Array)
            {
                node.Children = BuildTreeList(list, child.Id);
            }
            else
            {
                node.MapAlias = child.MapAlias;
            }

            res.Add(node);
        }

        return res;
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
                    throw new InvalidOperationException("不支持");
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


    JObject dfs_object(List<ParameterNode> list, Dictionary<string, JToken> hash, int idx)
    {
        var res = new JObject();
        foreach (var item in list)
        {
            string name = item.Name;

            if (item.Type == (int)ParameterType.Object)
            {
                var jobject = dfs_object(item.Children, hash, 0);
                res.Add(name, JObject.FromObject(jobject));
            }
            else if (item.Type == (int)ParameterType.Array)
            {
                var jarray = dfs_array(item, hash);
                res.Add(item.Name, JArray.FromObject(jarray));
            }
            else
            {
                if (item.Alias.Split("[*]").Length != item.MapAlias.Split("[*]").Length) continue;
                var key = item.MapAlias.Replace("[*]", $"[{idx}]");
                if (!hash.ContainsKey(key)) continue;
                var val = hash[key];
                //ValidationValue(item.Type, val);
                res.Add(name, val);
            }
        }

        return res;
    }

    JArray dfs_array(ParameterNode arr, Dictionary<string, JToken> hash)
    {
        var res = new JArray();
        foreach (var item in arr.Children)
        {
            int len = get_len(item, hash);
            for (int idx = 0; idx < len; idx++)
                if (item.Type == (int)ParameterType.Object)
                {
                    var jobject = dfs_object(item.Children, hash, idx);
                    res.Add(jobject);
                }
                else if (item.Type == (int)ParameterType.Array)
                {
                    var jarray = dfs_array(item, hash);
                    res.Add(jarray);
                }
                else
                {
                    var key2 = item.MapAlias.Replace("[*]", $"[{idx}]");
                    var val = hash[key2];
                    ValidationValue(item.Type, val);
                    res.Add(val);
                }
        }

        return res;
    }

    private int get_len(ParameterNode item, Dictionary<string, JToken> hash)
    {
        int res = 1;
        if (item.Type is not (int)ParameterType.Object and not (int)ParameterType.Array)
        {
            int idx = 0;
            while (hash.ContainsKey(item.MapAlias.Replace("[*]", $"[{idx}]"))) idx++;
            res = Math.Max(res, idx);

            return res;
        }

        foreach (var sub in item.Children)
        {
            if (sub.Type == (int)ParameterType.Object || sub.Type == (int)ParameterType.Array) continue;
            int idx = 0;
            while (hash.ContainsKey(sub.MapAlias.Replace("[*]", $"[{idx}]"))) idx++;
            res = Math.Max(res, idx);
        }

        return res;
    }


    private void ValidationValue(int itemType, JToken val)
    {
    }
}