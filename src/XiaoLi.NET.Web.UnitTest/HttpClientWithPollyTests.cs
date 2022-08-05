using System.Net;
using XiaoLi.NET.Web.HttpClientWithPolly;

namespace XiaoLi.NET.Web.UnitTest;

public class HttpClientWithPollyTests
{
    [Fact]
    public void Create_HttpClientWithPollyOptions()
    {
        var options = new HttpClientWithPollyOptions();
        // assert
        Assert.Equal(options.HttpResponseMessage.StatusCode, HttpStatusCode.OK);
    }
}