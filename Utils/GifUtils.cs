using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using GifsChat.Core;

namespace GifsChat.Utils;

public static class GifUtils
{
    public static async void ExtractAndSendGif(string gifUrl, string sentBy)
    {
        Stream gifStream = null;

        // Tries to get the Gif stream
        using (HttpClient client = new HttpClient())
        {
            try
            {
                gifStream = await client.GetStreamAsync(gifUrl);
            }
            catch (Exception e)
            {
                ModUtils.NewText("Failed to get Gif from URL!", true);
                ModUtils.NewText($"{e.GetType()}", true);
                ModUtils.NewText($"{e.Message}", true);
                return;
            }
        }

        if (gifStream == null)
        {
            ModUtils.NewText("Null Gif stream! (how did we even get here!?)", true);
            return;
        }

        // Extracts the Gif frames
        int framesLimit = GifsChatMod.ClientConfig.FramesLimit;
        bool skipEverySecondFrame = GifsChatMod.ClientConfig.SkipEverySecondFrame;
        var gifFramesStreams = await ExtractGifFrames(gifStream, framesLimit, skipEverySecondFrame);

        // Sends the Gif
        GifConverter.EnqueueGifFramesStreams(gifFramesStreams, sentBy, gifUrl);
    }

    /// <summary>
    /// Splits a Gif stream into an array of image streams
    /// </summary>
    private static async Task<Stream[]> ExtractGifFrames(
        Stream gifStream,
        int framesLimit,
        bool skipEverySecondFrame)
    {
        var frames = new List<Stream>();

        using (var gif = await Image.LoadAsync<Rgba32>(gifStream))
        {
            int totalFrameCount = gif.Frames.Count;

            if (totalFrameCount < 1)
                return null;

            // This is the canvas that will get drawn over after each iteration
            Image<Rgba32> canvas = null;
            bool isTransparent = false;

            // Will break once every frame has been extracted or the frames limit has been reached
            for (int i = 0; i < totalFrameCount && frames.Count < framesLimit; i++)
            {
                // We skip adding every second frame if frameskip is on
                if (skipEverySecondFrame && i % 2 == 1)
                    continue;

                var currFrame = gif.Frames.CloneFrame(i);
                
                if (i == 0)
                {
                    canvas = currFrame; // We set the canvas
                    isTransparent = canvas.IsTransparent();
                }
                else if (isTransparent)
                {
                    canvas = currFrame;
                }
                else // We draw over the canvas with the new frame
                {
                    canvas.Mutate(c => c.DrawImage(currFrame, 1));
                    currFrame.Dispose();
                }

                var frameStream = new MemoryStream();
                await canvas.SaveAsGifAsync(frameStream);

                frames.Add(frameStream);
            }

            canvas?.Dispose();
        }

        return frames.ToArray();
    }

    private static bool IsTransparent(this Image<Rgba32> image)
    {
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                if (image[x, y].A < 1)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
