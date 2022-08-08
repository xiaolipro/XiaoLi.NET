using XiaoLi.NET.DependencyInjection;

namespace XiaoLi.NET.UnitTests.DependencyInjection;

public class OrderService:IOrderService
{
    
}

public interface IOrderService:ITransientDependency
{
    
}