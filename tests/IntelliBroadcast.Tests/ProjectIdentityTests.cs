using IntelliBroadcast.Identity;
using IntelliBroadcast.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IntelliBroadcast.Tests;

public sealed class ProjectIdentityTests
{
    private static readonly Guid Expected = new("4f0d7c72-9a31-4c68-b526-2e8f5a1d93b7");

    [Fact]
    public void ProjectHash_IsTheStableHardcodedValue()
    {
        Assert.Equal(Expected, ProjectIdentity.FixedProjectHash);
        Assert.Equal(Expected, new ProjectIdentity().ProjectHash);
    }

    [Fact]
    public void ProjectIdentity_IsRegisteredAsOneSingleton()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IProjectIdentity, ProjectIdentity>();
        using var provider = services.BuildServiceProvider();
        Assert.Same(provider.GetRequiredService<IProjectIdentity>(), provider.GetRequiredService<IProjectIdentity>());
        Assert.Equal(Expected, provider.GetRequiredService<IProjectIdentity>().ProjectHash);
    }

    [Fact]
    public async Task OutboundHandler_AlwaysUsesExactProjectHash()
    {
        var capture = new CaptureHandler();
        var handler = new ProjectIdentityHeaderHandler(new ProjectIdentity()) { InnerHandler = capture };
        using var client = new HttpClient(handler);
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/status");
        request.Headers.Add("X-Project-Hash", Guid.NewGuid().ToString());
        await client.SendAsync(request);
        Assert.Equal(Expected.ToString("D"), capture.ProjectHash);
    }

    private sealed class CaptureHandler : HttpMessageHandler
    {
        public string? ProjectHash { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ProjectHash = request.Headers.GetValues("X-Project-Hash").Single();
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
        }
    }
}
