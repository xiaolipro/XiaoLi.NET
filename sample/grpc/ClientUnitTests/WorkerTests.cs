using Client;
using Grpc.Core;
using Moq;
using Test;

namespace ClientUnitTests;

public class WorkerTests
{
    #region snippet_Test
    [Fact]
    public async Task Greeting_Success_RepositoryCalled()
    {
        // Arrange
        var mockRepository = new Mock<IGreetRepository>();

        var mockCall = CallHelpers.CreateAsyncUnaryCall(new HelloReply { Message = "Test" });
        var mockClient = new Mock<Tester.TesterClient>();
        mockClient
            .Setup(m => m.SayHelloUnaryAsync(
                It.IsAny<HelloRequest>(), null, null, CancellationToken.None))
            .Returns(mockCall);

        var worker = new Worker(mockClient.Object, mockRepository.Object);

        // Act
        await worker.StartAsync(CancellationToken.None);

        // Assert
        mockRepository.Verify(v => v.SaveGreeting("Test"));
    }
    #endregion

    [Fact]
    public async Task Greeting_Error_ExceptionThrown()
    {
        // Arrange
        var mockRepository = new Mock<IGreetRepository>();
        var mockClient = new Mock<Tester.TesterClient>();
        mockClient
            .Setup(m => m.SayHelloUnaryAsync(It.IsAny<HelloRequest>(), null, null, CancellationToken.None))
            .Returns(CallHelpers.CreateAsyncUnaryCall<HelloReply>(StatusCode.InvalidArgument));

        var worker = new Worker(mockClient.Object, mockRepository.Object);

        // Act & Assert
        try
        {
            await worker.StartAsync(CancellationToken.None);
            Assert.True(false, "RpcException should have been thrown.");
        }
        catch (RpcException ex)
        {
            Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        }
    }
}