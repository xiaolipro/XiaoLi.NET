namespace XiaoLi.NET.UnitTests.Domain;

public class EntityTests
{
    [Fact]
    public void TransientEntity_NotEquals()
    {
        var a = new EntityA
        {
            A = 0,
            B = "xxx"
        };

        var b = new EntityA()
        {
            A = 0,
            B = "xxx"
        };
        
        Assert.True(a.IsTransient);
        Assert.True(b.IsTransient);
        Assert.False(a.Equals(b));
    }
}