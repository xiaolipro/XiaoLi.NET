using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests;

public class JsonRestructureTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public JsonRestructureTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private JObject jobj;

    [Fact]
    void Core()
    {
        // [][] -> [][]
        // [属性,属性,属性] -》1：1
        // len  危险

        //  a[].b[].c  ->  x[].y.z[].d
        //  a.c        ->  x.y[].z[].d
        //  order_weight ->  boxList[].weight[].x.c   NULL
        //  a[].c      ->  x.y[].z[].d
        var str =
            "{\"x\":{\"order_weight\":1},\"C\":[{\"a\":1},{\"a\":2},{\"a\":3}],\"B\":{\"a\":2,\"b\":[{\"a\":101,\"b\":\"b1\",\"c\":[1,2,3]},{\"a\":102,\"b\":\"b2\"},{\"a\":103,\"b\":\"b3\"}]},\"D\":[{\"a\":1,\"b\":1},{\"a\":2,\"b\":2}],\"E\":[{\"x\":\"x1\",\"y\":\"y1\"},{\"x\":\"x2\",\"y\":\"y2\"},{\"y\":\"y4\"}]}";
        jobj = JObject.Parse(str);
        _testOutputHelper.WriteLine(jobj.SelectToken("x.order_weight").ToString());
        var hash = Jobject2Hash(jobj);
        foreach (var item in hash)
        {
            _testOutputHelper.WriteLine(item.Key + "=" + item.Value);
        }

        _testOutputHelper.WriteLine("==============");

        List<Parameter> list = new()
        {
            new()
            {
                Id = 100,
                PId = 0,
                Name = "boxList",
                Type = ParameterType.Array,
                Alias = "boxList",
                //MapAlias = "order_weight"
            },
            new()
            {
                Id = 101,
                PId = 100,
                Name = "val",
                Type = ParameterType.Object,
                Alias = "boxList[*]",
            },
            new()
            {
                Id = 102,
                PId = 101,
                Name = "boxList2",
                Type = ParameterType.Array,
                Alias = "boxList[*].boxList2",
                //MapAlias = "order_weight"
            },
            new()
            {
                Id = 103,
                PId = 102,
                Name = "v",
                Type = ParameterType.Number,
                Alias = "boxList[*].boxList2[*]",
                MapAlias = "x.order_weight",
                //IsRequired = true
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
                Type = ParameterType.Number,
                MapAlias = "B.b[*].c[*]" // 叶子
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

        var res = JsonConvert.SerializeObject(dfs_object(treeList, hash, new List<int>()));

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
            node.IsRequired = child.IsRequired;

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


    JObject dfs_object(List<ParameterNode> list, Dictionary<string, JToken> hash, List<int> idxs)
    {
        var res = new JObject();
        foreach (var item in list)
        {
            string name = item.Name;

            if (item.Type == ParameterType.Object)
            {
                var jobject = dfs_object(item.Children, hash, idxs);
                res.Add(name, JObject.FromObject(jobject));
            }
            else if (item.Type == ParameterType.Array)
            {
                idxs.Add(-1);
                var jarray = dfs_array(item, hash, idxs);
                foreach (var jtoken in jarray)
                {
                    if (jtoken.Type != JTokenType.Null)
                    {
                        res.Add(item.Name, jarray);
                        break;
                    }
                }
                
                idxs.RemoveAt(idxs.Count - 1);
            }
            else
            {
                if (string.IsNullOrEmpty(item.MapAlias))
                {
                    if (item.IsRequired) throw new Exception($"必填参数{item.Name}（{item.Alias}）必须建立映射");
                    res.Add(name, null);
                    continue;
                }

                //if (item.Alias.Split("[*]").Length != item.MapAlias.Split("[*]").Length) continue;
                JToken val = JValue.CreateNull();

                string path = "";
                for (int i = 0, j = 0; i < item.MapAlias.Length; i++)
                {
                    if (item.MapAlias[i] == '[')
                    {
                        i += 2;
                        path += $"[{idxs[j]}]";
                        j++;
                    }
                    else path += item.MapAlias[i];
                }

                if (hash.ContainsKey(path))
                {
                    val = hash[path];
                }

                ValidationValue(item, val);
                res.Add(name, val);
            }
        }

        return res;
    }

    JArray dfs_array(ParameterNode arr, Dictionary<string, JToken> hash, List<int> idxs)
    {
        var res = new JArray();

        var item = arr.Children[0];
        int len = get_len(item);
        for (int idx = 0; idx < len; idx++)
        {
            idxs[^1]++;
            if (item.Type == ParameterType.Object)
            {
                var jobject = dfs_object(item.Children, hash, idxs);
                res.Add(jobject);
            }
            else if (item.Type == ParameterType.Array)
            {
                idxs.Add(-1);
                var jarray = dfs_array(item, hash, idxs);
                res.Add(jarray);
            }
            else
            {
                if (string.IsNullOrEmpty(item.MapAlias))
                {
                    if (item.IsRequired) throw new Exception($"必填参数{item.Name}（{item.Alias}）必须建立映射");
                    continue;
                }

                //if (item.Alias.Split("[*]").Length != item.MapAlias.Split("[*]").Length) continue;
                //var key = item.MapAlias.Replace("[*]", $"[{idx}]");

                JToken val = JValue.CreateNull();

                string path = "";
                var strs = item.MapAlias.Split("[*]");
                if (strs.Length < 2) path = item.MapAlias;
                for (var i = 0; i < strs.Length - 1; i++)
                {
                    path += $"{strs[i]}[{idxs[i]}]";
                }

                if (hash.ContainsKey(path))
                {
                    val = hash[path];
                }

                ValidationValue(item, val);
                res.Add(val);
            }
        }

        return res;
    }

    private int get_len(ParameterNode item)
    {
        if (item.MapAlias == null) return get_sub_len(item.Children);
        // 数组层级不对等情况
        if (item.Alias.Split("[*]").Length != item.MapAlias.Split("[*]").Length) return 1;

        return jobj.SelectTokens(item.MapAlias).Count();


        // if (item.Type is not ParameterType.Object and not ParameterType.Array)
        // {
        //     // 跳过无映射
        //     if (string.IsNullOrEmpty(item.MapAlias) || !item.MapAlias.Contains("[*]")) return res;
        //     int idx = 0;
        //     while (hash.ContainsKey(item.MapAlias.Replace("[*]", $"[{idx}]"))) idx++;
        //     res = Math.Max(res, idx);
        //
        //     return res;
        // }
        //
        // foreach (var sub in item.Children)
        // {
        //     if (sub.Type == ParameterType.Object || sub.Type == ParameterType.Array) continue;
        //     if (string.IsNullOrEmpty(sub.MapAlias) || !sub.MapAlias.Contains("[*]")) continue;
        //     int idx = 0;
        //     while (hash.ContainsKey(sub.MapAlias.Replace("[*]", $"[{idx}]"))) idx++;
        //     res = Math.Max(res, idx);
        // }
        //
        // return res;
    }

    private int get_sub_len(List<ParameterNode> nodes)
    {
        int res = 0;
        foreach (var node in nodes)
        {
            var len = get_len(node);
            if (res != 0 && res != len) throw new Exception("组成元素数量不对等");
            res = len;
        }

        return res;
    }


    private void ValidationValue(ParameterNode node, JToken val)
    {
        if (node.IsRequired && val.ToString() == "")
            throw new Exception($"参数{node.Name}（{node.Alias}）是必填的");
        if (!node.IsRequired && val.Type is JTokenType.Null) return;


        var equal = false;
        if (node.Type == ParameterType.Boolean) equal = bool.TryParse(val.ToString(), out _);
        if (node.Type == ParameterType.String) equal = val.Type is JTokenType.String;
        if (node.Type == ParameterType.Number) equal = decimal.TryParse(val.ToString(), out _);

        if (!equal)
            throw new Exception($"参数{node.Name}（{node.Alias}）期望的类型是{node.Type}，当实际分配的是{val.Type}类型的{val.ToString()}");
    }
}