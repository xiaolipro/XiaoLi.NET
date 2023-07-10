namespace XiaoLi.NET.UnitTests;
//obj arr
public class ParameterNode:Parameter
{
    public List<ParameterNode> Children { get; set; }

    public ParameterNode()
    {
        Children = new List<ParameterNode>();
    }
}