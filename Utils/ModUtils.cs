using GifsChat.Core;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Terraria;

using Color = Microsoft.Xna.Framework.Color;

namespace GifsChat.Utils;

public static class ModUtils
{
    public static async void ExtractAndSendGif(string gifUrl, string sentBy)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var gifStream = await client.GetStreamAsync(gifUrl);

                var gifFramesStreams = await ExtractGifFrames(gifStream);
                GifConverter.EnqueueGifFramesStreams(gifFramesStreams, sentBy, gifUrl);
            }
            catch (Exception e)
            {
                NewText("Failed to get or send Gif!", true);
                NewText($"{e.Message}", true);
            }
        }
    }

    public static async Task<T> DeserializeResults<T>(HttpResponseMessage response)
    {
        string json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            ModUtils.NewText("Failed to deserialize response!", true);
            return default;
        }

        return JsonConvert.DeserializeObject<T>(json);
    }

    // 90% of this was written by Bing AI lol
    /// <summary>
    /// Splits a Gif stream into an array of image streams
    /// </summary>
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

                // We skip adding every second frame if frameskip is on
                if (GifsChatMod.ClientConfig.SkipEverySecondFrame && i % 2 == 1)
                    continue;

                var frameStream = new MemoryStream();
                await image.SaveAsGifAsync(frameStream);
                
                frames.Add(frameStream);
            }
        }
        
        return frames.ToArray();
    }

    /// <summary>
    /// Sends a message to the chat
    /// </summary>
    public static void NewText(string message, bool isException = false) 
        => Main.NewText($"[GifsChat] {message}", isException ? Color.DarkOrange : Color.Yellow);

    public static void RerouteToApiSite()
        => RerouteToSite("https://developers.google.com/tenor/guides/quickstart");

    public static void RerouteToSite(string url)
        => Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
}
