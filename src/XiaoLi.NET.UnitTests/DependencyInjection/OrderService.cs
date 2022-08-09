
using XiaoLi.NET.DependencyInjection.LifecycleInterfaces;

namespace XiaoLi.NET.UnitTests.DependencyInjection;

public class OrderService:IOrderService
{
    
}

public interface IOrderService:ITransient
{
    
}