using GifsChat.Models.Json;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GifsChat.Models.Json;
public record ResultData
{
    [JsonProperty("id")]
    public string Id { get; init; }

    [JsonProperty("media_formats")]
    public Dictionary<string, MediaFormat> MediaFormats { get; init; }

    [JsonProperty("content_description")]
    public string Description { get; init; }

    [JsonProperty("itemurl")]
    public string ItemUrl { get; init; }

    [JsonProperty("url")]
    public string Url { get; init; }

    public MediaFormat GetFormat(FormatType formatType)
        => MediaFormats[formatType.ToString().ToLower()];

}
