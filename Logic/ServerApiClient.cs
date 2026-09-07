using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WinClient.Logic;

/// <summary>
/// Simple client for interacting with the local server's /register and /login endpoints.
/// </summary>
public sealed class ServerApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _disposeClient;
    private readonly string _baseAddress = "http://localhost:8080";

    public ServerApiClient(HttpClient? httpClient = null)
    {
        if (httpClient is null)
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _disposeClient = true;
        }
        else
        {
            _httpClient = httpClient;
            _disposeClient = false;
        }
    }

    /// <summary>
    /// Calls POST /register with the provided payload. Expects the server to return the new user's ID
    /// either as a JSON object with an "id" property or as a plain string in the response body.
    /// </summary>
    /// <typeparam name="T">Type of the registration payload.</typeparam>
    /// <param name="payload">Payload to send to /register</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The registered user's ID string.</returns>
    /// <exception cref="ServerApiException">Thrown when the request fails or the response cannot be parsed.</exception>
    public async Task<string> RegisterAsync<T>(T payload, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = new Uri(new Uri(_baseAddress), "/register");
            using var response = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var body = await SafeReadContentAsync(response).ConfigureAwait(false);
                throw new ServerApiException(
                    $"Register request failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(content))
                throw new ServerApiException("Register response was empty.");

            // Try to parse JSON with an "id" property
            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    if (doc.RootElement.TryGetProperty("id", out var idProp) && 
                        idProp.ValueKind == JsonValueKind.String)
                    {
                        return idProp.GetString()!;
                    }
                    // common alternate names
                    if (doc.RootElement.TryGetProperty("userId", out var userIdProp) && 
                        userIdProp.ValueKind == JsonValueKind.String)
                    {
                        return userIdProp.GetString()!;
                    }
                }
            }
            catch (JsonException)
            {
                // Not JSON, fall back to raw string
            }

            // If content is quoted string, trim quotes
            var trimmed = content.Trim();
            if ((trimmed.StartsWith("\"") && trimmed.EndsWith("\"")) || 
                (trimmed.StartsWith("'") && trimmed.EndsWith("'")))
            {
                return trimmed.Substring(1, trimmed.Length - 2);
            }

            return trimmed;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ServerApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServerApiException("An error occurred while registering the user.", ex);
        }
    }

    /// <summary>
    /// Calls GET /login?id={userId}. Returns true when the server responds with a successful status code.
    /// </summary>
    /// <param name="userId">ID returned by the /register endpoint.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if login succeeded (2xx response), otherwise throws.</returns>
    public async Task<bool> LoginAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("userId must be provided", nameof(userId));

        try
        {
            var uriBuilder = new UriBuilder(_baseAddress)
            {
                Path = "/login",
                Query = $"id={Uri.EscapeDataString(userId)}"
            };

            using var response = await _httpClient.GetAsync(uriBuilder.Uri, cancellationToken)
                .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var body = await SafeReadContentAsync(response).ConfigureAwait(false);
                throw new ServerApiException(
                    $"Login request failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ServerApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ServerApiException("An error occurred while logging in the user.", ex);
        }
    }

    private static async Task<string> SafeReadContentAsync(HttpResponseMessage response)
    {
        try
        {
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        catch
        {
            return string.Empty;
        }
    }

    public void Dispose()
    {
        if (_disposeClient)
        {
            _httpClient.Dispose();
        }
    }
}

/// <summary>
/// Exception type used by ServerApiClient to wrap errors.
/// </summary>
public class ServerApiException : Exception
{
    public ServerApiException()
    {
    }

    public ServerApiException(string message)
        : base(message)
    {
    }

    public ServerApiException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
