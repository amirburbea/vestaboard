using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Vestaboard.Common;

public interface IBoardClient
{
    Task<bool> PostMessageAsync(BoardCharacter[][] characters, CancellationToken cancellationToken = default);
}

public sealed class BoardClient : IBoardClient, IDisposable
{
    private static readonly MediaTypeHeaderValue _jsonContentType = MediaTypeHeaderValue.Parse("application/json");

    private readonly HttpClient _httpClient;
    private readonly string _readWriteApiKey;

    public BoardClient(HttpMessageHandler messageHandler, IConfiguration configuration)
    {
        this._httpClient = new(messageHandler);
        this._readWriteApiKey = configuration["LOCAL_API_KEY"] ?? string.Empty;
    }

    public void Dispose() => _httpClient.Dispose();

    public async Task<bool> PostMessageAsync(BoardCharacter[][] characters, CancellationToken cancellationToken = default)
    {
        if (characters is not { Length: 6 } || Array.Exists(characters, row => row is not { Length: 22 }))
        {
            throw new ArgumentException("Invalid characters (must array of 6 rows of 22 chars per row).", nameof(characters));
        }
        using HttpRequestMessage request = new(HttpMethod.Post, "http://vestaboard.local:7000/local-api/message")
        {
            Headers = { { "X-Vestaboard-Local-Api-Key", this._readWriteApiKey } },
            Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(characters)) { Headers = { ContentType = BoardClient._jsonContentType } }
        };
        using HttpResponseMessage response = await this._httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created or HttpStatusCode.NoContent;
    }
}