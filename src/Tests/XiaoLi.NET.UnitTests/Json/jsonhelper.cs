using HermaFx;
using XiaoLi.NET.Mvc.Exceptions;
using Newtonsoft.Json.Linq;

namespace XiaoLi.NET.UnitTests.Json;


public static class JsonHelper
{
    /// <summary>
    /// json重构
    /// </summary>
    /// <param name="jtoken"></param>
    /// <param name="treeList"></param>
    /// <param name="mappedJsonType">映射后的json类型</param>
    /// <returns></returns>
    public static JToken Restructure(JToken jtoken, List<ParameterNode> treeList,
        MappedJsonType mappedJsonType = MappedJsonType.Object)
    {
        if (treeList == default || treeList.Count == 0) return jtoken;
        var idxList = new List<int>();

        var hash = Json2Hash(jtoken); // 将json转化成hash
        
        switch (mappedJsonType)
        {
            case MappedJsonType.Object:
                return dfs_object(jtoken, treeList, hash, idxList);
            case MappedJsonType.Array:
                idxList.Add(-1);
                return dfs_array(jtoken, treeList[0], hash, idxList);
            case MappedJsonType.Value:
                var value = hash[treeList[0].MapAlias!];
                return new JValue(value);
            default:
                throw new ArgumentOutOfRangeException(nameof(mappedJsonType), mappedJsonType, null);
        }
    }
    
    public static void RemoveNullProperties(JToken token)
    {
        if (token.Type == JTokenType.Object)
        {
            var obj = (JObject)token;
            var propertiesToRemove = obj.Properties()
                .Where(p => p.Value.Type == JTokenType.Null)
                .ToList();

            foreach (var property in propertiesToRemove)
            {
                property.Remove();
            }

            foreach (var property in obj.Properties())
            {
                RemoveNullProperties(property.Value);
            }
        }
        else if (token.Type == JTokenType.Array)
        {
            var array = (JArray)token;
            for (int i = array.Count - 1; i >= 0; i--)
            {
                RemoveNullProperties(array[i]);
            }
        }
    }

    private static Dictionary<string, JToken> Json2Hash(JToken jtoken)
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

    public static List<ParameterNode> BuildTreeList(IReadOnlyCollection<Parameter> list)
    {
        if (list.All(x => x.MapAlias.IsNullOrWhiteSpace()))
        {
            return default;
        }

        return DoBuild(list, 0);
    }

    private static List<ParameterNode> DoBuild(IReadOnlyCollection<Parameter> list, int pid)
    {
        var res = new List<ParameterNode>();

        foreach (var child in list.Where(x => x.PId == pid))
        {
            var node = new ParameterNode();
            node.Id = child.Id;
            node.PId = child.PId;
            node.Name = child.Name;
            node.Type = child.Type;
            node.Alias = child.Alias;
            node.IsRequired = child.IsRequired;
            if (node.Type is ParameterType.Object or ParameterType.Array)
            {
                node.Children = DoBuild(list, child.Id);
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

                bool isEmptyArray = jarray.All(el => el.Type == JTokenType.Null);
                res.Add(item.Name, isEmptyArray ? null : jarray);

                idxList.RemoveAt(idxList.Count - 1);
            }
            else
            {
                if (string.IsNullOrEmpty(item.MapAlias))
                {
                    // if (item.IsRequired)
                    //     throw new BusinessException(message: $"必填参数{item.Name}（{item.Alias}）必须建立映射");
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

                if (hash.ContainsKey(path!))
                {
                    val = hash[path];
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
                    // if (item.IsRequired)
                    //     throw new BusinessException(message: $"必填参数{item.Name}（{item.Alias}）必须建立映射");
                    continue;
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
        // if (node.IsRequired && val.ToString() == "")
        //     throw new BusinessException(message: $"服务商参数{node.Name}是必填的（系统{node.MapAlias}->服务商{node.Alias}）");
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
            throw new BusinessException(message:
                $"参数{node.Name}（{node.Alias}）期望的类型是{node.Type}，当实际分配的是{val.Type}类型的{val}（{val.Path}）");

        return val;
    }
}

public enum MappedJsonType
{
    Object,
    Array,
    Value
}