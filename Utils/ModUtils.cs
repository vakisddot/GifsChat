using GifsChat.Configs;
using GifsChat.Core;
using GifsChat.Utils.Exceptions;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GifsChat.Utils;

public static class ModUtils
{
    public static async Task<T> DeserializeResults<T>(HttpResponseMessage response)
    {
        string json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new GifsChatException($"Failed to deserialize response!");
        }

        return JsonConvert.DeserializeObject<T>(json);
    }

    public static async Task<Stream> GetStreamFromUrl(string url)
    {
        Stream stream;

        using (HttpClient client = new HttpClient())
        {
            stream = await client.GetStreamAsync(url);
        }

        return stream;
    }

    // 90% of this was written by Bing AI lol
    public static async Task<Stream[]> ExtractGifFrames(Stream gifStream)
    {
        var frames = new List<Stream>();

        using (var gif = await Image.LoadAsync<Rgba32>(gifStream))
        {
            var totalFrameCount = gif.Frames.Count;

            Image<Rgba32> previousFrame = null;

            // Will break once every frame has been extracted or the frames limit has been reached
            for (int i = 0; i < totalFrameCount && frames.Count < GifsChatMod.ClientConfig.FramesLimit; i++)
            {
                var frame = gif.Frames.CloneFrame(i);
                var image = new Image<Rgba32>(frame.Width, frame.Height);
                if (previousFrame != null)
                {
                    image.Mutate(c => c.DrawImage(previousFrame, 1));
                }
                image.Mutate(c => c.DrawImage(frame, 1));
                previousFrame = image.Clone();

                var frameStream = new MemoryStream();
                await image.SaveAsGifAsync(frameStream);

                // We skip adding every second frame if frameskip is on
                if (GifsChatMod.ClientConfig.SkipEverySecondFrame && i % 2 == 1)
                    continue;
                
                frames.Add(frameStream);
            }
        }
        
        return frames.ToArray();
    }

    public static void RerouteToApiSite()
        => Process.Start(new ProcessStartInfo("https://developers.google.com/tenor/guides/quickstart") { UseShellExecute = true });
}
