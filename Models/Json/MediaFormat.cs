using Newtonsoft.Json;
using System.Collections.Generic;

namespace GifsChat.Models.Json;
public record MediaFormat
{
    [JsonProperty("url")]
    public string Url { get; init; }

    [JsonProperty("duration")]
    public double Duration { get; init; }

    [JsonProperty("preview")]
    public string Preview { get; init; }

    [JsonProperty("dims")]
    public List<int> Dimensions { get; init; }

    [JsonProperty("size")]
    public int Size { get; init; }
}
