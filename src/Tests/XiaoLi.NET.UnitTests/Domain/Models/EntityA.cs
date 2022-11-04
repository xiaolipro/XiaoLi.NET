using XiaoLi.NET.Domain;

namespace XiaoLi.NET.UnitTests.Domain;

public class EntityA:Entity<int>                                                           
{
    public int A { get; set; }
    public string B { get; set; }
}