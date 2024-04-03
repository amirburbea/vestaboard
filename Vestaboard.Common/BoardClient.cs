using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Vestaboard.Common;

public interface IBoardClient
{
    Task<bool> PostMessageAsync(BoardCharacter[][] characters, CancellationToken cancellationToken = default);
}

public sealed class BoardClient(HttpMessageHandler messageHandler, IConfiguration configuration) : IBoardClient, IDisposable
{
    private static readonly MediaTypeHeaderValue _jsonContentType = MediaTypeHeaderValue.Parse("application/json");

    private readonly HttpClient _httpClient = new(messageHandler);
    private readonly string _localApiKey = configuration["LOCAL_API_KEY"] ?? throw new ArgumentException("Configuration does not contain local api key.", nameof(configuration));

    public void Dispose() => _httpClient.Dispose();

    public async Task<bool> PostMessageAsync(BoardCharacter[][] characters, CancellationToken cancellationToken = default)
    {
        if (characters is not { Length: 6 } || Array.Exists(characters, row => row is not { Length: 22 }))
        {
            throw new ArgumentException("Invalid characters (must array of 6 rows of 22 chars per row).", nameof(characters));
        }
        using HttpRequestMessage request = new(HttpMethod.Post, "http://vestaboard.local:7000/local-api/message")
        {
            Headers = { { "X-Vestaboard-Local-Api-Key", this._localApiKey } },
            Content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(characters)) { Headers = { ContentType = BoardClient._jsonContentType } }
        };
        using HttpResponseMessage response = await this._httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created or HttpStatusCode.NoContent;
    }
}