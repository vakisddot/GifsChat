using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using System;

namespace GifsChat.Core;

public class GifsChatSystem : ModSystem
{
    private static Dictionary<uint, Queue<Stream>> s_awaitingStreams = new();
    private static Dictionary<uint, List<Texture2D>> s_awaitingGifs = new();
    private static Dictionary<uint, (string sentBy, string url)> s_gifDatas = new();

    private const int StreamConvertDelay = 0;
    private int _frameCounter;

    public override void PostUpdateEverything()
    {
        TrySendGif();

        if (_frameCounter >= StreamConvertDelay)
        {
            TryConvertFirstStream();
            _frameCounter = 0;
        }

        _frameCounter++;
    }
    private void TryConvertFirstStream()
    {
        foreach (var awaitingStream in s_awaitingStreams)
        {
            uint hash = awaitingStream.Key;
            var queue = awaitingStream.Value;

            var stream = queue.Dequeue();

            s_awaitingGifs[hash].Add(Texture2D.FromStream(Main.instance.GraphicsDevice, stream));

            stream.Dispose();
        }
    }
    private void TrySendGif()
    {
        foreach (var awaitingGif in s_awaitingGifs.Where(g => g.Value.Any()))
        {
            uint hash = awaitingGif.Key;
            var queue = awaitingGif.Value;

            if (s_awaitingStreams[hash].Any())
                continue;

            try
            {
                //Main.NewText($"<{s_gifSenders[hash]}_{hash}>");
                Main.NewText($"<{s_gifDatas[hash].sentBy}>");
                RemadeChatMonitorHooks.SendTexture(queue.ToArray(), s_gifDatas[hash].url);
            }
            catch { }
            finally
            {
                // Once a gif has been successfully sent, we delete it from our caches
                s_awaitingStreams.Remove(hash);
                s_awaitingGifs.Remove(hash);
                s_gifDatas.Remove(hash);

                //Main.NewText(s_awaitingGifs.Count);
            }
        }
    }
    public static void EnqueueGifFramesStreams(Stream[] streams, string sentBy, string url)
    {
        uint hashCode = (uint)(DateTime.Now.GetHashCode() ^ sentBy.GetHashCode());

        s_awaitingStreams.Add(hashCode, new());
        s_awaitingGifs.Add(hashCode, new());
        s_gifDatas.Add(hashCode, (sentBy, url));

        foreach (var stream in streams)
        {
            s_awaitingStreams[hashCode].Enqueue(stream);
        }
    }
}
