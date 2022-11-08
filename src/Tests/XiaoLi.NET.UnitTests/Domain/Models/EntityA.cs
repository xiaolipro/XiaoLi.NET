using XiaoLi.NET.Domain.SeedWork;

namespace XiaoLi.NET.UnitTests.Domain;

public class EntityA:Entity<int>                                                           
{
    public int A { get; set; }
    public string B { get; set; }

    public EntityA():this(default)
    {
        
    }
    public EntityA(int id) : base(id)
    {
    }
}