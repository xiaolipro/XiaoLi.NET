using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests;

public class JsonTurnTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public JsonTurnTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    void Json_to_jobject_to_json_test()
    {
        //var str = "{\"size\":\"10\", \"weight\":\"10kg\"}";
        var str =
            "{\"A\":1,\"B\":{\"a\":2,\"b\":[{\"a\":1},{\"b\":2,\"b2\":[1,2,3,{\"c\":\"ok\",\"d\":2132143324532432}]},{\"c\":3,\"d\":4}]},\"C\":\"4\"}";
        //var str = "{\"name\":\"数组\",\"abbreviation\":\"ss\",\"requestAddress\":\"dsfd\",\"description\":null,\"requestProtocol\":13,\"requestMethod\":16,\"category\":3,\"isBatch\":false,\"contentType\":19,\"isReturnMessage\":false,\"optionType\":3,\"inParameterList\":[{\"id\":79,\"pId\":0,\"name\":\"aa\",\"cnName\":null,\"type\":12,\"description\":null,\"isInPara\":true,\"children\":[{\"id\":80,\"pId\":79,\"name\":\"aa_vv\",\"cnName\":null,\"type\":9,\"description\":null,\"isInPara\":true,\"children\":[],\"_X_ROW_CHILD\":[],\"Id\":\"row_33\"}],\"_X_ROW_CHILD\":[{\"id\":80,\"pId\":79,\"name\":\"aa_vv\",\"cnName\":null,\"type\":9,\"description\":null,\"isInPara\":true,\"children\":[],\"_X_ROW_CHILD\":[],\"Id\":\"row_33\"}],\"Id\":\"row_32\"},{\"id\":80,\"pId\":79,\"name\":\"aa_vv\",\"cnName\":null,\"type\":9,\"description\":null,\"isInPara\":true,\"children\":[],\"_X_ROW_CHILD\":[],\"Id\":\"row_33\"},{\"optionType\":2,\"id\":81,\"pId\":80,\"name\":\"aa_vv_asd\",\"cnName\":null,\"type\":8,\"description\":null,\"isInPara\":true,\"children\":[],\"_X_ROW_CHILD\":[],\"Id\":\"row_34\",\"parameterId\":60},{\"optionType\":1,\"id\":1701921002,\"pId\":0,\"type\":7,\"name\":\"bb\",\"cnName\":null,\"description\":null,\"action\":null,\"Id\":\"row_35\",\"_X_ROW_CHILD\":[],\"parameterId\":60,\"children\":[]}],\"inParameterList2\":[{\"id\":79,\"pId\":0,\"name\":\"aa\",\"cnName\":null,\"type\":12,\"description\":null,\"isInPara\":true,\"children\":[{\"id\":80,\"pId\":79,\"name\":\"aa_vv\",\"cnName\":null,\"type\":9,\"description\":null,\"isInPara\":true,\"children\":[],\"_X_ROW_CHILD\":[],\"Id\":\"row_33\"}],\"_X_ROW_CHILD\":[{\"id\":80,\"pId\":79,\"name\":\"aa_vv\",\"cnName\":null,\"type\":9,\"description\":null,\"isInPara\":true,\"children\":[],\"_X_ROW_CHILD\":[],\"Id\":\"row_33\"}],\"Id\":\"row_32\"},{\"id\":80,\"pId\":79,\"name\":\"aa_vv\",\"cnName\":null,\"type\":9,\"description\":null,\"isInPara\":true,\"children\":[],\"_X_ROW_CHILD\":[],\"Id\":\"row_33\"},{\"optionType\":2,\"id\":81,\"pId\":80,\"name\":\"aa_vv_asd\",\"cnName\":null,\"type\":8,\"description\":null,\"isInPara\":true,\"children\":[],\"_X_ROW_CHILD\":[],\"Id\":\"row_34\",\"parameterId\":60},{\"optionType\":1,\"id\":1701921002,\"pId\":0,\"type\":7,\"name\":\"bb\",\"cnName\":null,\"description\":null,\"action\":null,\"Id\":\"row_35\",\"_X_ROW_CHILD\":[],\"parameterId\":60,\"children\":[]}],\"inParameterList3\":[{\"id\":79,\"pId\":0,\"name\":\"aa\",\"cnName\":null,\"type\":12,\"description\":null,\"isInPara\":true,\"children\":[{\"id\":80,\"pId\":79,\"name\":\"aa_vv\",\"cnName\":null,\"type\":9,\"description\":null,\"isInPara\":true,\"children\":[],\"_X_ROW_CHILD\":[],\"Id\":\"row_33\"}],\"_X_ROW_CHILD\":[{\"id\":80,\"pId\":79,\"name\":\"aa_vv\",\"cnName\":null,\"type\":9,\"description\":null,\"isInPara\":true,\"children\":[],\"_X_ROW_CHILD\":[],\"Id\":\"row_33\"}],\"Id\":\"row_32\"},{\"id\":80,\"pId\":79,\"name\":\"aa_vv\",\"cnName\":null,\"type\":9,\"description\":null,\"isInPara\":true,\"children\":[],\"_X_ROW_CHILD\":[],\"Id\":\"row_33\"},{\"optionType\":2,\"id\":81,\"pId\":80,\"name\":\"aa_vv_asd\",\"cnName\":null,\"type\":8,\"description\":null,\"isInPara\":true,\"children\":[],\"_X_ROW_CHILD\":[],\"Id\":\"row_34\",\"parameterId\":60},{\"optionType\":1,\"id\":1701921002,\"pId\":0,\"type\":7,\"name\":\"bb\",\"cnName\":null,\"description\":null,\"action\":null,\"Id\":\"row_35\",\"_X_ROW_CHILD\":[],\"parameterId\":60,\"children\":[]}],\"outParameterList\":[{\"optionType\":1,\"id\":1415909283,\"pId\":0,\"type\":7,\"name\":\"bb\",\"cnName\":null,\"description\":null,\"action\":null,\"Id\":\"row_36\",\"_X_ROW_CHILD\":[],\"parameterId\":60,\"children\":[]}],\"parameterId\":60}";
        //str = "{\"A\":\"a\",\"B\":null,\"C\":123,\"D\":123.666,\"E\":true,\"F\":false,\"G\":[\"a\",\"b\",\"c\",\"d\"],\"H\":[123,666,123.666,666.123],\"I\":[true,false,false],\"J\":[{\"A\":\"a\",\"B\":null,\"C\":123,\"D\":123.666,\"E\":true,\"F\":false},{\"A\":\"a\",\"B\":null,\"C\":123,\"D\":123.666,\"E\":true,\"F\":false}],\"K\":[[],[],[]],\"L\":[],\"M\":[null,null,null]}";
        //str = "[{\"_A\":\"a\",\"_B\":null,\"_C\":123,\"_D\":123.666,\"_E\":true,\"_F\":false},{\"_A\":\"a\",\"_B\":null,\"_C\":123,\"_D\":123.666,\"_E\":true,\"_F\":false}]";

        str = File.ReadAllText("D:\\BSI\\Downloads\\100.json");
        var res = Turn(str);
        _testOutputHelper.WriteLine(res);
    }

    private string Turn(string str)
    {
        try
        {
            var obj = JObject.Parse(str);
            var res = dfs(obj);
            return JsonConvert.SerializeObject(res);
        }
        catch (JsonReaderException e)
        {
            var arr = JArray.Parse(str);
            var res = dfs(arr);
            return JsonConvert.SerializeObject(res);
        }
    }

    [Fact]
    void Json_to_jobject_error_test()
    {
        var str = "{\"A\":1,\"b\":{\"a\":1s2,\"b\":[1,2,{\"c\":\"ok\",\"d\":2132143324532432}]},\"C\":\"4\"}";

        Assert.Throws<JsonReaderException>(() =>
        {
            var obj = JObject.Parse(str);
        });
    }

    JObject dfs(JObject @object)
    {
        var properties = @object.Properties();
        JObject res = new JObject();
        foreach (var property in properties)
        {
            string name = "_" + property.Path;
            var jvalue = property.Value;

            if (property.Value is JArray array)
            {
                jvalue = dfs(array);
            }
            else if (property.Value is JObject obj)
            {
                jvalue = dfs(obj);
            }

            res.Add(name, jvalue);
        }

        return res;
    }

    JArray dfs(JArray array)
    {
        var res = new JArray();
        foreach (var jToken in array)
        {
            var jvalue = jToken;

            if (jToken is JArray arr)
            {
                jvalue = dfs(arr);
            }
            else if (jToken is JObject obj)
            {
                jvalue = dfs(obj);
            }

            res.Add(jvalue);
        }

        return res;
    }

    [Fact]
    void Create_jobject_test()
    {
        var jobject = new JObject();
        jobject.Add("a", "123");

        var jobject2 = new JObject();
        jobject2.Add("b", JObject.FromObject(new { a = 1 }));
        jobject.Add("B", JObject.FromObject(jobject2));
        jobject.Add("C", JArray.FromObject(new[] { 1, 2, 3 }));
        _testOutputHelper.WriteLine(jobject.ToString());
    }


    [Fact]
    void Jobject2Hash_test()
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
                Type = ParameterType.String,
                Alias = "_A",
                MapAlias = "A"
            },
            new()
            {
                Id = 2,
                PId = 0,
                Name = "_B",
                Alias = "_B",
                Type = ParameterType.Array,
            },
            new()
            {
                Id = 3,
                PId = 2,
                Name = "_C",
                Alias = "_B[*]",
                Type = ParameterType.Object,
            },
            new()
            {
                Id = 4,
                PId = 3,
                Name = "_D",
                Alias = "_B[*]._D",
                Type = ParameterType.Array,
            },
            new()
            {
                Id = 5,
                PId = 4,
                Name = "_E",
                Alias = "_B[*]._D[*]",
                Type = ParameterType.String,
                MapAlias = "C[*]" // 叶子
            },
            new()
            {
                Id = 6,
                PId = 3,
                Name = "_F",
                Alias = "_B[*]._F",
                Type = ParameterType.String,
                MapAlias = "B.b[*].b"
            },
            new()
            {
                Id = 7,
                PId = 0,
                Name = "_G",
                Alias = "_K",
                Type = ParameterType.Array,
            },
            new()
            {
                Id = 8,
                PId = 7,
                Name = "_gg",
                Alias = "_K[*]",
                Type = ParameterType.Object,
            },
            new()
            {
                Id = 9,
                PId = 8,
                Name = "_X",
                Alias = "_K[*]._X",
                Type = ParameterType.Number,
                MapAlias = "D[*].a"
            },
            new()
            {
                Id = 10,
                PId = 8,
                Name = "_Y",
                Alias = "_K[*]._Y",
                Type = ParameterType.String,
                MapAlias = "E[*].x"
            },
        };

        List<ParameterNode> treeList = BuildTreeList(list, 0);

        var res = JsonConvert.SerializeObject(dfs(treeList, hash, 0));
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
            if (node.Type is ParameterType.Object or ParameterType.Array)
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


    JObject dfs(List<ParameterNode> list, Dictionary<string, JToken> hash, int idx)
    {
        var res = new JObject();
        foreach (var item in list)
        {
            string name = item.Name;

            if (item.Type == ParameterType.Object)
            {
                var jobject = dfs(item.Children, hash, 0);
                res.Add(name, JObject.FromObject(jobject));
            }
            else if (item.Type == ParameterType.Array)
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
                if (item.Type == ParameterType.Object)
                {
                    var jobject = dfs(item.Children, hash, idx);
                    res.Add(jobject);
                }
                else if (item.Type == ParameterType.Array)
                {
                    var jarray = dfs_array(item, hash);
                    res.Add(jarray);
                }
                else
                {
                    var key2 = item.MapAlias.Replace("[*]", $"[{idx}]");
                    var val = hash[key2];
                    ValidationValue(item, val);
                    res.Add(val);
                }
        }

        return res;
    }

    private int get_len(ParameterNode item, Dictionary<string, JToken> hash)
    {
        int res = 1;
        if (item.Type is not ParameterType.Object and not ParameterType.Array)
        {
            int idx = 0;
            while (hash.ContainsKey(item.MapAlias.Replace("[*]", $"[{idx}]"))) idx++;
            res = Math.Max(res, idx);

            return res;
        }

        foreach (var sub in item.Children)
        {
            if (sub.Type == ParameterType.Object || sub.Type == ParameterType.Array) continue;
            int idx = 0;
            while (hash.ContainsKey(sub.MapAlias.Replace("[*]", $"[{idx}]"))) idx++;
            res = Math.Max(res, idx);
        }

        return res;
    }


    private void ValidationValue(ParameterNode node, JToken val)
    {
    }
}