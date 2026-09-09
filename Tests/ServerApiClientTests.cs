using System.Net;
using System.Text;
using WinClient.Logic;

namespace WinClient.Tests;

[TestFixture]
public class ServerApiClientTests
{
    private class FakeHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder) 
        : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _responder = 
            responder ?? throw new ArgumentNullException(nameof(responder));

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            return _responder(request, cancellationToken);
        }
    }

    private static HttpClient CreateClient(Func<HttpRequestMessage, CancellationToken, 
        Task<HttpResponseMessage>> responder)
    {
        var handler = new FakeHandler(responder);
        return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };
    }

    [Test]
    public async Task RegisterAsync_ReturnsId_FromJson()
    {
        var client = CreateClient((req, ct) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"id\":\"abc123\"}", Encoding.UTF8, "application/json")
        }));

        using var api = new ServerApiClient(client);
        var returnVal = await api.RegisterAsync(new { username = "u" });
        string id = returnVal[(returnVal.IndexOf(':') + 1)..].Trim('"', '}');
        Assert.That(id, Is.EqualTo("abc123"));
    }

    [Test]
    public async Task RegisterAsync_ReturnsId_FromPlainString()
    {
        var client = CreateClient((req, ct) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("plain-id-789", Encoding.UTF8, "text/plain")
        }));

        using var api = new ServerApiClient(client);
        var id = await api.RegisterAsync(new { username = "u" });
        Assert.That(id, Is.EqualTo("plain-id-789"));
    }

    [Test]
    public async Task RegisterAsync_ReturnsId_FromQuotedString()
    {
        var client = CreateClient((req, ct) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("\"quoted-id\"", Encoding.UTF8, "text/plain")
        }));

        using var api = new ServerApiClient(client);
        var id = await api.RegisterAsync(new { username = "u" });
        Assert.That(id, Is.EqualTo("quoted-id"));
    }

    [Test]
    public void RegisterAsync_Throws_OnServerError()
    {
        var client = CreateClient((req, ct) => 
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("boom", Encoding.UTF8, "text/plain")
        }));

        using var api = new ServerApiClient(client);
        Assert.ThrowsAsync<ServerApiException>(async () => await api.RegisterAsync(new { username = "u" }));
    }

    [Test]
    public async Task LoginAsync_ReturnsTrue_OnSuccess()
    {
        var client = CreateClient((req, ct) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        using var api = new ServerApiClient(client);
        var result = await api.LoginAsync("some-id");
        Assert.That(result, Is.True);
    }

    [Test]
    public void LoginAsync_Throws_OnFailure()
    {
        var client = CreateClient((req, ct) => 
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("nope", Encoding.UTF8, "text/plain")
        }));

        using var api = new ServerApiClient(client);
        Assert.ThrowsAsync<ServerApiException>(async () => await api.LoginAsync("some-id"));
    }
}
