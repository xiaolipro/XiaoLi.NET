namespace XiaoLi.NET.UnitTests;

public class Parameter
{
    /// <summary>
    /// 参数Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    public int PId { get; set; }

    ///<summary>
    /// 参数名
    ///</summary>
    public string Name { get; set; }

    ///<summary>
    /// 参数类型
    ///</summary>
    public ParameterType Type { get; set; }

    ///<summary>
    /// 参数别名
    ///</summary>
    public string Alias { get; set; }
    
    ///<summary>
    /// 映射参数别名
    ///</summary>
    public string MapAlias { get; set; }

    public bool IsRequired { get; set; }
}