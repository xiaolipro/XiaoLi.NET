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

        [Fact]
        public void GOTO_Test()
        {
            string str = "123";

            switch (str)
            {
                case "1":
                    Console.WriteLine(1);
                    break;
                case "2":
                    Console.WriteLine(2);
                    break;
                default:
                    goto case "1";
            }
        }
    }
}