﻿using GifsChat.Models.Json;
using GifsChat.Utils.Exceptions;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GifsChat.Models.Communicators;

public class TenorCommunicator : ICommunicator
{
    // Search parameters
    private const string ClientKey = "gifs_chat_terraria";
    private const string MediaFilter = "tinygif,tinygifpreview";
    public Type ResultType => typeof(TenorResults);
    // Url
    private const string TenorApiUrl = 
        @"https://tenor.googleapis.com/v2/search?q={0}&key={1}&client_key={2}&limit={3}&media_filter={4}&contentfilter={5}";

    public async Task<HttpResponseMessage> GetResponse(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new GifsChatException("Your request was empty!");

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

                sb.AppendLine($"Failed to get OK response from Tenor!");

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    sb.AppendLine($"  Make sure to setup your API key in the mod config.");
                    sb.AppendLine($"  Use '/gif apiKey' to get a key");
                }
                else
                {
                    sb.AppendLine($" Status code {(int)response.StatusCode} ({response.StatusCode}).");
                }

                throw new GifsChatException(sb.ToString());
            }
        }

        return response;
    }
}