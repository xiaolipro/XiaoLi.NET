using Moq;
using Server.Services;
using Test;

namespace ServerUnitTests;

public class TesterServiceTests
{
    #region snippet_SayHelloUnaryTest

    [Fact]
    public async Task SayHelloUnaryTest()
    {
        // Arrange
        var mockGreeter = new Mock<IGreeterService>();
        mockGreeter.Setup(
            m => m.Greet(It.IsAny<string>())).Returns((string s) => $"Hello {s}");
        var service = new TesterService(mockGreeter.Object);

        // Act
        var response = await service.SayHelloUnary(
            new HelloRequest { Name = "Joe" }, CustomServerCallContext.Create());

        // Assert
        mockGreeter.Verify(v => v.Greet("Joe"));
        Assert.Equal("Hello Joe", response.Message);
    }

    #endregion


    [Fact]
    public async Task SayHelloServerStreamingTest()
    {
        // Arrange
        var mockGreeter = new Mock<IGreeterService>();
        mockGreeter.Setup(m => m.Greet(It.IsAny<string>())).Returns((string s) => $"Hello {s}");
        var service = new TesterService(mockGreeter.Object);

        var cts = new CancellationTokenSource();
        var callContext = CustomServerCallContext.Create(cancellationToken: cts.Token);
        var responseStream = new CustomServerStreamWriter<HelloReply>(callContext);

        // Act
        using var call =
            service.SayHelloServerStreaming(new HelloRequest { Name = "Joe" }, responseStream, callContext);

        // Assert
        Assert.False(call.IsCompletedSuccessfully, "Method should run until cancelled.");

        cts.Cancel();

        await call;
        responseStream.Complete();

        var allMessages = new List<HelloReply>();
        await foreach (var message in responseStream.ReadAllAsync())
        {
            allMessages.Add(message);
        }

        Assert.True(allMessages.Count >= 1);

        Assert.Equal("Hello Joe 1", allMessages[0].Message);
    }

    [Fact]
    public async Task SayHelloClientStreamingTest()
    {
        // Arrange
        var mockGreeter = new Mock<IGreeterService>();
        mockGreeter.Setup(m => m.Greet(It.IsAny<string>())).Returns((string s) => $"Hello {s}");
        var service = new TesterService(mockGreeter.Object);

        var callContext = CustomServerCallContext.Create();
        var requestStream = new CustomAsyncStreamReader<HelloRequest>(callContext);

        // Act
        using var call = service.SayHelloClientStreaming(requestStream, callContext);

        requestStream.AddMessage(new HelloRequest { Name = "James" });
        requestStream.AddMessage(new HelloRequest { Name = "Jo" });
        requestStream.AddMessage(new HelloRequest { Name = "Lee" });
        requestStream.Complete();

        // Assert
        var response = await call;
        Assert.Equal("Hello James, Jo, Lee", response.Message);
    }

    [Fact]
    public async Task SayHelloBidirectionStreamingTest()
    {
        // Arrange
        var mockGreeter = new Mock<IGreeterService>();
        mockGreeter.Setup(m => m.Greet(It.IsAny<string>())).Returns((string s) => $"Hello {s}");
        var service = new TesterService(mockGreeter.Object);

        var callContext = CustomServerCallContext.Create();
        var requestStream = new CustomAsyncStreamReader<HelloRequest>(callContext);
        var responseStream = new CustomServerStreamWriter<HelloReply>(callContext);

        // Act
        using var call = service.SayHelloBidirectionalStreaming(requestStream, responseStream, callContext);

        // Assert
        requestStream.AddMessage(new HelloRequest { Name = "James" });
        Assert.Equal("Hello James", (await responseStream.ReadNextAsync())!.Message);

        requestStream.AddMessage(new HelloRequest { Name = "Jo" });
        Assert.Equal("Hello Jo", (await responseStream.ReadNextAsync())!.Message);

        requestStream.AddMessage(new HelloRequest { Name = "Lee" });
        Assert.Equal("Hello Lee", (await responseStream.ReadNextAsync())!.Message);

        requestStream.Complete();

        await call;
        responseStream.Complete();

        Assert.Null(await responseStream.ReadNextAsync());
    }
}