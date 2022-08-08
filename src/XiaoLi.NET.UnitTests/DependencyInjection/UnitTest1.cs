
namespace XiaoLi.NET.UnitTests.DependencyInjection
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var type = typeof(OrderService);
            var interfaces = type.GetInterfaces();
        }
    }
}