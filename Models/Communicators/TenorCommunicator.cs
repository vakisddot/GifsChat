using GifsChat.Models.Json;
using GifsChat.Utils;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GifsChat.Models.Communicators;

/// <summary>
/// Provides a way to communicate with Tenor
/// </summary>
public class TenorCommunicator : ICommunicator
{
    private const string ProxyUrl =
        @"https://tenor-proxy.onrender.com/search?q={0}&results_limit={1}&content_filter={2}";

    public async Task<string> QueryGifUrl(string query)
    {
        if (string.IsNullOrWhiteSpace(query)
            || !GifsChatMod.ClientConfig.GifsEnabled 
            || !GifsChatMod.ServerConfig.GifsEnabled)
            return null;

        var response = await GetResponse(query);

        if (response == null)
            return null;

        var results = await ModUtils.DeserializeResults<TenorResults>(response);

        // Selects a random Gif URL from the results
        var gifUrl = results?
            .GetRandomResult()
            .GetFormat(FormatType.TinyGif)
            .Url;

        return gifUrl;
    }

    /// <summary>
    /// Sends a GET request to the search engine. Does NOT check if input is valid
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Response message if OK, null otherwise</returns>
    private async Task<HttpResponseMessage> GetResponse(string query)
    {
        int resultsLimit = GifsChatMod.ClientConfig.ResultsLimit;

        string contentFilter = GifsChatMod.ServerConfig.ContentFilter
            .ToString()
            .ToLower();
            
        HttpResponseMessage response;

        using (HttpClient client = new HttpClient())
        {
            string url = string.Format(ProxyUrl, query, resultsLimit, contentFilter);
            response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                StringBuilder sb = new();

                sb.AppendLine($"Failed to get OK response from Tenor!");
                sb.AppendLine($" Status code {(int)response.StatusCode} ({response.StatusCode}).");

                ModUtils.NewText(sb.ToString().TrimEnd(), true);
                return null;
            }
        }

        return response;
    }
}