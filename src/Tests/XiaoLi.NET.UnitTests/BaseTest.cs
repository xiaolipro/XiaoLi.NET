using System.Collections.Concurrent;
using System.Numerics;
using System.Threading.Channels;
using System.Web;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests
{
    enum Colors
    {
        Red,
        White
    }

    public class BaseTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BaseTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        void sadf()
        {
            var res = maketuple();

            (string response, bool ok) maketuple()
            {
                return ("123", true);
            }

            processres(ref res);
            
            void processres(ref (string response, bool ok) res)
            {
                res.ok = false;
            }

            _testOutputHelper.WriteLine(res.ok.ToString());
        }

        [Fact]
        public void Enum_ToString_Test()
        {
            _testOutputHelper.WriteLine(Colors.White.ToString());
            _testOutputHelper.WriteLine(((int)Colors.White).ToString());
        }

        [Fact]
        public void Test1()
        {
            string str = "sadkhkajsfhjer";
            for (int i = 0; i < str.Length; i++)
            {
                var item = str[i];
                ConsoleOutput.Instance.Write(item.ToString(), OutputLevel.Information);
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
                    _testOutputHelper.WriteLine(1.ToString());
                    break;
                case "2":
                    _testOutputHelper.WriteLine(2.ToString());
                    break;
                default:
                    goto case "1";
            }
        }


        [Fact]
        public void Nullable_Test()
        {
            DateTime? dd = null;
            if (dd != null && dd.Value > DateTime.Now)
            {
                _testOutputHelper.WriteLine(1.ToString());
            }
            else
            {
                _testOutputHelper.WriteLine(2.ToString());
            }
        }

        [Fact]
        public void String_Test()
        {
            string origin = "123";
            var p1 = origin.GetHashCode();
            ProcessRef(ref origin);
            Process(origin);
            var p2 = origin.GetHashCode();
            string str = "123";
            var p3 = str.GetHashCode();
            Assert.Equal("123A", origin);
            Assert.True(p1 == p3);
            // Assert.True(origin.Equals(str));
        }

        private void ProcessRef(ref string origin)
        {
            origin += "A";
        }

        private void Process(string origin)
        {
            origin += "A";
        }

        [Fact]
        void BitOperationsTests()
        {
            _testOutputHelper.WriteLine(BitOperations.RoundUpToPowerOf2(65).ToString());
            _testOutputHelper.WriteLine(BitOperations.LeadingZeroCount(2).ToString());
            _testOutputHelper.WriteLine(BitOperations.TrailingZeroCount(2).ToString());
        }


        [Fact]
        void ConcurrentQueueTest()
        {
            var q = new ConcurrentQueue<string>();
            for (int i = 0; i < 40; i++)
            {
                q.Enqueue("c" + i);
            }

            q.TryDequeue(out var res);
            q.TryPeek(out var res2);
        }
        
        [Fact]
        void ff()
        {
            // var res = Enumerable.Range(11, 62);
            // _testOutputHelper.WriteLine(string.Join(",",res));
            List<byte> list = new List<byte>();
            for (int i = 0; i < int.MaxValue + 1e5; i++)
            {
                list.Add(new byte());
            }
        }

        [Fact]
        void uritest()
        {
            var builder = new UriBuilder("http://example.com");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["foo"] = "bar<>&-baz";
            query["bar"] = "bazinga";
            builder.Query = query.ToString();
            string url = builder.ToString();
            _testOutputHelper.WriteLine(url);
        }


        [Fact]
        void tmp()
        {
            while (true)
            {
                
            }
        }
    }
}