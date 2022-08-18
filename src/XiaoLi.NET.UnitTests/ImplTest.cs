namespace XiaoLi.NET.UnitTests;

public class ImplTest
{
    [Fact]
    public void IsAssignableFrom_Test()
    {
        Assert.True(typeof(IA).IsAssignableFrom(typeof(A)));
        Assert.True(typeof(IB).IsAssignableFrom(typeof(A)));
        Assert.True(typeof(IB).IsAssignableFrom(typeof(IA)));
    }
    
    
    private class A:IA
    {
        
    }
    
    private interface IA:IB
    {
        
    }
    
    private interface IB
    {
        
    }
}