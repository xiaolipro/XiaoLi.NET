using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XiaoLi.NET.Mvc.Exceptions;
using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests.Json;

public class JsonRestructureTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public JsonRestructureTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    void Core()
    {
        _testOutputHelper.WriteLine(Thread.CurrentThread.CurrentCulture.ToString());
        _testOutputHelper.WriteLine(DateTime.Now.ToString("yyyy-M-d dddd"));
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        _testOutputHelper.WriteLine(Thread.CurrentThread.CurrentCulture.ToString());
        _testOutputHelper.WriteLine(DateTime.Now.ToString("yyyy-M-d dddd"));
        // [][] -> [][]
        // [属性,属性,属性] -》1：1
        // len  危险

        //  a[].b[].c  ->  x[].y.z[].d
        //  a.c        ->  x.y[].z[].d
        //  order_weight ->  boxList[].weight[].x.c   NULL
        //  a[].c      ->  x.y[].z[].d
        var str = //"[{\"orderNo\":1},{\"orderNo\":2},{\"orderNo\":3}]";
        """
{
	"x": [{
		"order_weight": 1
	}, {
		"order_weight": 2
	}, {
		"order_weight": 3
	}],
	"C": [{
		"a": 1
	}, {
		"a": 2
	}, {
		"a": 3
	}],
	"B": {
		"a": 2,
		"b": [{
			"a": 101,
			"b": "b1",
			"c": [123]
		}, {
			"a": 102,
			"b": "b2",
			"c": [2, 3, 7, 9]
		}, {
			"a": 103,
			"b": "b3",
			"c": [2]
		}]
	},

}
""";
        var jtoken = JToken.Parse(str);
        //_testOutputHelper.WriteLine(jtoken.SelectToken("[0].orderNo").ToString());

        _testOutputHelper.WriteLine("==============");

        List<Parameter> list = GetMaps();

        List<ParameterNode> treeList = BuildTreeList(list, 0);

        var res = JsonConvert.SerializeObject(dfs_json(jtoken, treeList));

        _testOutputHelper.WriteLine(res);
    }
    
    
    [Fact]
    void Jarray()
    {
        // [{}] -> []
        // [{}] -> {[]}
        // [{}] -> [{}]
        var str = "{\"orderNo\":1}";
        var jtoken = JToken.Parse(str);
        //_testOutputHelper.WriteLine(jtoken.SelectToken("[0].orderNo").ToString());
        var hash = Json2Hash(jtoken);
        foreach (var item in hash)
        {
            _testOutputHelper.WriteLine(item.Key + "=" + item.Value);
        }

        _testOutputHelper.WriteLine("==============");

        List<Parameter> list = GetMaps2();
        if (jtoken is JArray)
        {
            list.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.MapAlias))
                {
                    x.MapAlias = "[*]." + x.MapAlias;
                }
            });
        }

        List<ParameterNode> treeList = BuildTreeList(list, 0);

        var res = JsonConvert.SerializeObject(dfs_json(jtoken, treeList, MappedJsonType.Array));

        _testOutputHelper.WriteLine(res);
    }
    
    [Fact]
    void Value()
    {
        // {} -> v
        var str = "1";
        var jtoken = JToken.Parse(str);
        //_testOutputHelper.WriteLine(jtoken.SelectToken("[0].orderNo").ToString());
        var hash = Json2Hash(jtoken);
        foreach (var item in hash)
        {
            _testOutputHelper.WriteLine(item.Key + "=" + item.Value);
        }

        _testOutputHelper.WriteLine("==============");

        List<Parameter> list = GetMaps3();

        List<ParameterNode> treeList = BuildTreeList(list, 0);

        var res = JsonConvert.SerializeObject(dfs_json(jtoken, treeList, MappedJsonType.Value));

        _testOutputHelper.WriteLine(res);
    }

    [Fact]
    void annotation()
    {
        string json = @"
            {
                ""SubVersion"": ""1707"", //子版本，1801代表着18年1月
                ""RequestOption"": ""nonvalidate"", //请求操作，validate=城市/州/邮政编码/组合验证，nonvalidate=邮政编码/州组合验证/y
                ""TransactionReference"": {
                    ""CustomerContext"": """" //客户上下文信息
                }
            }";

        JsonLoadSettings settings = new JsonLoadSettings
        {
            CommentHandling = CommentHandling.Load
        };

        JToken token = JToken.Parse(json);

        // 遍历所有的注释
        foreach (var comment in token.Annotations<string>())
        {
            _testOutputHelper.WriteLine(comment); // 输出注释文本
        }

    }

    enum MappedJsonType
    {
        Object,
        Array,
        Value
    }

    private JToken dfs_json(JToken jtoken, List<ParameterNode> treeList, MappedJsonType mappedJsonType = MappedJsonType.Object)
    {
        if (treeList.Count == 0) return jtoken;
        var idxList = new List<int>();
        var hash = Json2Hash(jtoken);
        foreach (var item in hash)
        {
            _testOutputHelper.WriteLine(item.Key + "=" + item.Value);
        }
        
        switch (mappedJsonType)
        {
            case MappedJsonType.Object:
                return dfs_object(jtoken, treeList, hash, idxList);
            case MappedJsonType.Array:
                idxList.Add(-1);
                return dfs_array(jtoken, treeList[0], hash, idxList);
            case MappedJsonType.Value:
                var value = hash[treeList[0].MapAlias!];
                return value;
            default:
                throw new ArgumentOutOfRangeException(nameof(mappedJsonType), mappedJsonType, null);
        }
    }

    public static Dictionary<string, JToken> Json2Hash(JToken jtoken)
    {
        if (jtoken is JObject obj)
        {
            return Jobject2Hash(obj);
        }

        if (jtoken is JArray arr)
        {
            return JArrayToHash(arr);
        }

        // jvalue
        return new Dictionary<string, JToken>()
        {
            { jtoken.Path, jtoken }
        };
    }

    public static List<Parameter> GetMaps()
    {
        return new List<Parameter>()
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
                Id = 102,
                PId = 101,
                Name = "name",
                Type = ParameterType.String,
                Alias = "boxList[*].name",
                MapAlias = "x[*].order_weight"
            },
            new()
            {
                Id = 103,
                PId = 102,
                Name = "v",
                Type = ParameterType.Integer,
                Alias = "boxList[*].boxList2[*]",
                MapAlias = "B.b[*].c[*]",
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
                Name = "val",
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
                Type = ParameterType.Integer,
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
                Name = "_K",
                Alias = "_K",
                Type = ParameterType.Array,
            },
            new()
            {
                Id = 8,
                PId = 7,
                Name = "val",
                Alias = "_K[*]",
                Type = ParameterType.Object,
            },
            new()
            {
                Id = 9,
                PId = 8,
                Name = "_X",
                Alias = "_K[*]._X",
                Type = ParameterType.Integer,
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
    }

    public static List<Parameter> GetMaps2()
    {
        // return new List<Parameter>()
        // {
        //     new()
        //     {
        //         Id = 100,
        //         PId = 0,
        //         Name = "codes",
        //         Type = ParameterType.Array,
        //         Alias = "codes",
        //     },
        //     new()
        //     {
        //         Id = 101,
        //         PId = 100,
        //         Name = "val",
        //         Type = ParameterType.Object,
        //         Alias = "codes[*]",
        //     },
        //     new()
        //     {
        //         Id = 102,
        //         PId = 0,
        //         Name = "no",
        //         Type = ParameterType.String,
        //         Alias = "no",
        //         MapAlias = "orderNo"
        //     },
        //     new()
        //     {
        //         Id = 110,
        //         PId = 101,
        //         Name = "no",
        //         Type = ParameterType.String,
        //         Alias = "codes[*].val",
        //         MapAlias = "orderNo"
        //     },
        // };
        return new List<Parameter>()
        {
            new()
            {
                Id = 100,
                PId = 0,
                Name = "codes",
                Type = ParameterType.Array,
                Alias = "codes",
            },
            new()
            {
                Id = 101,
                PId = 100,
                Name = "val",
                Type = ParameterType.String,
                Alias = "codes[*]",
                MapAlias = "orderNo"
            },
        };
    }

    public static List<Parameter> GetMaps3()
    {
        return new List<Parameter>()
        {
            new()
            {
                Id = 100,
                PId = 0,
                Name = "code",
                Type = ParameterType.String,
                Alias = "code",
                MapAlias = ""
            },
        };
    }

    private static Dictionary<string, JToken> Jobject2Hash(JObject jobject)
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

    private static List<ParameterNode> BuildTreeList(IReadOnlyCollection<Parameter> list, int pid)
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

    private static Dictionary<string, JToken> JArrayToHash(JArray jarr)
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
                res.Add(jToken.Path, jToken);
            }
        }

        return res;
    }

    private static void Add(Dictionary<string, JToken> a, Dictionary<string, JToken> b)
    {
        foreach (var item in b)
        {
            a.Add(item.Key, item.Value);
        }
    }


    private static JObject dfs_object(JToken jtoken, List<ParameterNode> list, Dictionary<string, JToken> hash,
        List<int> idxList)
    {
        var res = new JObject();
        foreach (var item in list)
        {
            string name = item.Name;

            if (item.Type == ParameterType.Object)
            {
                res.Add(name, dfs_object(jtoken, item.Children, hash, idxList));
            }
            else if (item.Type == ParameterType.Array)
            {
                idxList.Add(-1);
                var jarray = dfs_array(jtoken, item, hash, idxList);

                // bool isEmptyArray = jarray.All(el => el.Type == JTokenType.Null);
                // res.Add(item.Name, isEmptyArray ? null : jarray);
                res.Add(item.Name, jarray);

                idxList.RemoveAt(idxList.Count - 1);
            }
            else
            {
                if (string.IsNullOrEmpty(item.MapAlias))
                {
                    if (item.IsRequired)
                        throw new BusinessException(message: $"必填参数{item.Name}（{item.Alias}）必须建立映射");
                    res.Add(name, null);
                    continue;
                }

                //if (item.Alias.Split("[*]").Length != item.MapAlias.Split("[*]").Length) continue;
                JToken val = JValue.CreateNull();

                string path = "";

                if (item.Alias.IndexOf('[') == -1 && item.MapAlias.IndexOf('[') != -1)
                    path = item.MapAlias.Replace("[*]", "[0]");
                else
                {
                    for (int i = 0, j = 0; i < item.MapAlias.Length; i++)
                    {
                        if (item.MapAlias[i] == '[')
                        {
                            i += 2;
                            if (j >= idxList.Count)
                                throw new BusinessException(
                                    message: $"{item.Name}（{item.MapAlias} -> {item.Alias}）是非法映射");
                            path += $"[{idxList[j]}]";
                            j++;
                        }
                        else path += item.MapAlias[i];
                    }
                }

                if (hash.TryGetValue(path, out var value))
                {
                    val = value;
                }

                val = ValidationValue(item, val);
                res.Add(name, val);
            }
        }

        return res;
    }

    private static JArray dfs_array(JToken jobject, ParameterNode arr, Dictionary<string, JToken> hash,
        List<int> idxList)
    {
        var res = new JArray();

        var item = arr.Children[0];
        int len = get_len(jobject, item, idxList.Count);
        for (int idx = 0; idx < len; idx++)
        {
            idxList[^1]++;
            if (item.Type == ParameterType.Object)
            {
                res.Add(dfs_object(jobject, item.Children, hash, idxList));
            }
            else if (item.Type == ParameterType.Array)
            {
                idxList.Add(-1);
                var jarray = dfs_array(jobject, item, hash, idxList);
                res.Add(jarray);
            }
            else
            {
                if (string.IsNullOrEmpty(item.MapAlias))
                {
                    if (item.IsRequired)
                        throw new BusinessException(message: $"必填参数{item.Name}（{item.Alias}）必须建立映射");
                }

                //if (item.Alias.Split("[*]").Length != item.MapAlias.Split("[*]").Length) continue;
                //var key = item.MapAlias.Replace("[*]", $"[{idx}]");

                JToken val;

                string path = "";
                var strs = item.MapAlias.Split("[*]");
                if (strs.Length < 2) path = item.MapAlias;
                var i = 0;
                for (; i < strs.Length - 1; i++)
                {
                    path += $"{strs[i]}[{idxList[i]}]";
                }

                if (i != 0) path += strs[i]; // suffix

                if (hash.ContainsKey(path!))
                {
                    val = hash[path];
                }
                else break;

                val = ValidationValue(item, val);
                res.Add(val);
            }
        }

        return res;
    }

    private static int get_len(JToken jobject, ParameterNode item, int cnt)
    {
        if (string.IsNullOrWhiteSpace(item.MapAlias))
        {
            if (item.Type != ParameterType.Array && item.Type != ParameterType.Object) return 0;
            return get_sub_len(jobject, item, cnt);
        }

        // 数组层级不对等情况
        if (item.Alias.Split("[*]").Length != item.MapAlias.Split("[*]").Length) return 1;

        int index = -1;
        for (int i = 0; i < cnt; i++)
        {
            index = item.MapAlias.IndexOf("[*]", index + 1, StringComparison.Ordinal);
            if (index == -1)
            {
                break;
            }
        }

        var path = item.MapAlias.Substring(0, index + 3);
        // for (int i = 0; i < idxList.Count; i++)
        // {
        //     var idx = path.IndexOf("[*]", StringComparison.Ordinal);
        //     if (idx >= 0)
        //     {
        //         path = path.Remove(idx,3).Insert(idx,$"[{idxList[i]}]");
        //     }
        // }
        return jobject.SelectTokens(path).Count();
    }


    private static int get_sub_len(JToken jobject, ParameterNode node, int cnt)
    {
        int res = 0;
        foreach (var item in node.Children)
        {
            var len = get_len(jobject, item, cnt);
            if (len == 0) continue;
            if (res != 0 && res != len) throw new BusinessException(message: $"组成元素数量不对等，请检查{node.Alias}的组成");
            res = Math.Max(len, res);
        }

        return res;
    }


    private static JToken ValidationValue(ParameterNode node, JToken val)
    {
        if (node.IsRequired && val.ToString() == "")
            throw new Exception(message: $"服务商参数{node.Name}是必填的（系统{node.MapAlias}->服务商{node.Alias}）");
        if (!node.IsRequired && val.Type is JTokenType.Null) return val;


        var equal = false;
        if (node.Type == ParameterType.Boolean)
        {
            equal = bool.TryParse(val.ToString(), out bool v);
            if (equal) val = v;
        }
        else if (node.Type == ParameterType.Float)
        {
            equal = decimal.TryParse(val.ToString(), out decimal v);
            if (equal) val = v;
        }
        else if (node.Type == ParameterType.Integer)
        {
            equal = long.TryParse(val.ToString(), out long v);
            if (equal) val = v;
        }
        else if (node.Type == ParameterType.String)
        {
            equal = val.Type is JTokenType.String;
            if (!equal)
            {
                // 数值转字符串
                if (val.Type is JTokenType.Float or JTokenType.Integer or JTokenType.Boolean)
                {
                    val = val.ToString();
                    equal = true;
                }
            }
        }


        if (!equal)
            throw new Exception(message:
                $"参数{node.Name}（{node.Alias}）期望的类型是{node.Type}，当实际分配的是{val.Type}类型的{val}（{val.Path}）");

        return val;
    }
}