using IntelliBroadcast.Identity;
using System.Net.Http.Json;

namespace IntelliBroadcast.Services;

public sealed class ProjectIdentityHeaderHandler(IProjectIdentity identity) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Remove("X-Project-Hash");
        request.Headers.Add("X-Project-Hash", identity.ProjectHash.ToString("D"));
        return base.SendAsync(request, cancellationToken);
    }
}

public interface IProductApiClient { Task<HttpResponseMessage> GetStatusAsync(CancellationToken cancellationToken = default); }
public sealed class ProductApiClient(HttpClient client) : IProductApiClient
{
    public Task<HttpResponseMessage> GetStatusAsync(CancellationToken cancellationToken = default) => client.GetAsync("status", cancellationToken);
}
