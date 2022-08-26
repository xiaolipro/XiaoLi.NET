using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Test;
using XiaoLi.NET.Grpc.IntegrationTests.Infrastructure;
using XiaoLi.NET.Grpc.Interceptors;
using Xunit.Abstractions;

namespace XiaoLi.NET.Grpc.IntegrationTests;

public class GrpcIntegrationTests:IntegrationTestBase
{
        private readonly CallInvoker _callInvoker;
        public GrpcIntegrationTests(IntegrationFixture<Startup> fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _callInvoker = Channel
                .Intercept(new ClientExceptionInterceptor(fixture.LoggerFactory.CreateLogger<ClientExceptionInterceptor>()))
                .Intercept(new ClientLogInterceptor(fixture.LoggerFactory.CreateLogger<ClientLogInterceptor>()));
        }

        [Theory]
        [InlineData("张三", "Hello 张三")]
        [InlineData("李四", "Hello 李四")]
        [InlineData("XX", "Hello XX")]
        public void SayHello_BlockingUnary_Test(string name, string expected)
        {
            // Arrange
            var client = new Tester.TesterClient(_callInvoker);

            // Act
            var response = client.SayHelloUnary(new HelloRequest { Name = name });

            // Assert
            Assert.Equal(expected, response.Message);
        }

        [Theory]
        [InlineData("张三", "Hello 张三")]
        [InlineData("李四", "Hello 李四")]
        public async Task SayHello_AsyncUnary_Test(string name, string expected)
        {
            // Arrange
            var client = new Tester.TesterClient(_callInvoker);

            // Act
            var response = await client.SayHelloUnaryAsync(new HelloRequest { Name = name });

            // Assert
            Assert.Equal(expected, response.Message);
        }

        [Theory]
        [InlineData(new[] { "张三", "李四" }, "Hello 张三, 李四")]
        public async Task SayHello_ClientStreaming_Test(string[] names, string expected)
        {
            // Arrange
            var client = new Tester.TesterClient(_callInvoker);

            HelloReply response;

            // Act
            using var call = client.SayHelloClientStreaming();
            foreach (var name in names)
            {
                await call.RequestStream.WriteAsync(new HelloRequest { Name = name });
            }
            await call.RequestStream.CompleteAsync();

            response = await call;

            // Assert
            Assert.Equal(expected, response.Message);
        }

        [Fact]
        public async Task SayHello_ServerStreaming_Test()
        {
            // Arrange
            var client = new Tester.TesterClient(_callInvoker);

            var cts = new CancellationTokenSource();
            var hasMessages = false;
            var callCancelled = false;

            // Act
            using var call = client.SayHelloServerStreaming(new HelloRequest { Name = "Joe" }, cancellationToken: cts.Token);
            try
            {
                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    hasMessages = true;
                    cts.Cancel();
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                callCancelled = true;
            }

            // Assert
            Assert.True(hasMessages);
            Assert.True(callCancelled);
        }

        [Fact]
        public async Task SayHello_BidirectionStreaming_Test()
        {
            // Arrange
            var client = new Tester.TesterClient(_callInvoker);

            var names = new[] { "James", "Jo", "Lee" };
            var messages = new List<string>();

            // Act
            using var call = client.SayHelloBidirectionalStreaming();
            foreach (var (item,idx) in names.Select((item,idx)=>(item,idx)))
            {
                await call.RequestStream.WriteAsync(new HelloRequest { Name = item });

                Assert.True(await call.ResponseStream.MoveNext());
                messages.Add(call.ResponseStream.Current.Message);
            }

            await call.RequestStream.CompleteAsync();

            // Assert
            Assert.Equal(3, messages.Count);
            Assert.Equal("Hello James", messages[0]);
        }
}