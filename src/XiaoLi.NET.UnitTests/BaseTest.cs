using System.Threading.Channels;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace XiaoLi.NET.UnitTests
{
    public class BaseTest
    {
        [Fact]
        public void Test1()
        {
            string str = "sadkhkajsfhjer";
            for (int i = 0; i < str.Length; i++)
            {
                var item = str[i];
                ConsoleOutput.Instance.Write(item.ToString(),OutputLevel.Information);
                item = 'a';
            }
        }
        
        [Fact]
        public void IEnumerable_Where_Test()
        {
            var items = Enumerable.Range(1, 2);
            var new_items = items.Where(x => x > 3);
            var res = new_items.Count();
            
            Assert.Equal(0, res);
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