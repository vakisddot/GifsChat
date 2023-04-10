using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GifsChat.Models.Json;

public record TenorResults
{
    private static Random Rand = new();

    [JsonProperty("results")]
    public List<ResultData> Results { get; init; }
    public ResultData GetRandomResult()
        => Results[Rand.Next(0, Results.Count)];
}