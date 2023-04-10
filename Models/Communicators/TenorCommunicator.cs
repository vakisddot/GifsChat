using GifsChat.Core;
using GifsChat.Models.Json;
using GifsChat.Utils;
using GifsChat.Utils.Exceptions;
using Microsoft.Xna.Framework;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace GifsChat.Models.Communicators;

public class TenorCommunicator : ICommunicator
{
    // Search parameters
    private const string ClientKey = "gifs_chat_terraria";
    private const string MediaFilter = "tinygif,tinygifpreview";
    // Url
    private const string TenorApiUrl = 
        @"https://tenor.googleapis.com/v2/search?q={0}&key={1}&client_key={2}&limit={3}&media_filter={4}&contentfilter={5}";

    public async void HandleQuery(string query)
    {
        if (!GifsChatMod.ClientConfig.GifsEnabled || !GifsChatMod.ServerConfig.GifsEnabled)
            return;

        var response = await GetResponse(query);

        if (response == null)
            return;

        var results = await ModUtils.DeserializeResults<TenorResults>(response);
        var result = results.GetRandomResult();
        var gifUrl = result.GetFormat(FormatType.TinyGif).Url;

        var gifStream = await ModUtils.GetStreamFromUrl(gifUrl);
        var gifFramesStreams = await ModUtils.ExtractGifFrames(gifStream);

        GifsChatSystem.EnqueueGifFramesStreams(gifFramesStreams);
    }

    public async Task<HttpResponseMessage> GetResponse(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            Main.NewText("[GIFsChat] Empty request!", Color.Orange);
            return null;
        }    

        HttpResponseMessage response;

        using (HttpClient client = new HttpClient())
        {
            string apiKey = GifsChatMod.ClientConfig.TenorApiKey;
            int resultsLimit = GifsChatMod.ClientConfig.ResultsLimit;

            string contentFilter = GifsChatMod.ServerConfig.ContentFilter
                .ToString()
                .ToLower();

            response = await client.GetAsync(string.Format(TenorApiUrl, query, apiKey, ClientKey, resultsLimit, MediaFilter, contentFilter));

            if (!response.IsSuccessStatusCode)
            {
                StringBuilder sb = new();

                sb.AppendLine($"[GIFsChat] Failed to get OK response from Tenor!");

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    sb.AppendLine($"  Make sure to setup your API key in the mod config.");
                    sb.AppendLine($"  Use '/gif apiKey' to get a key");
                }
                else
                {
                    sb.AppendLine($" Status code {(int)response.StatusCode} ({response.StatusCode}).");
                }

                Main.NewText(sb.ToString().TrimEnd(), Color.Orange);
                return null;
            }
        }

        return response;
    }
}