using GifsChat.Models.Json;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GifsChat.Models.Json;
public record ResultData
{
    [JsonProperty("media_formats")]
    public Dictionary<string, MediaFormat> MediaFormats { get; init; }
    
    public MediaFormat GetFormat(FormatType formatType)
        => MediaFormats[formatType.ToString().ToLower()];

}
