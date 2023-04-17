using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using System;

namespace GifsChat.Core;
public class GifConverter : ModSystem
{
    private static Dictionary<uint, Queue<Stream>> s_awaitingStreams = new();
    private static Dictionary<uint, List<Texture2D>> s_awaitingGifs = new();
    private static Dictionary<uint, (string sentBy, string url)> s_gifDatas = new();

    public override void PostUpdateEverything()
    {
        TrySendFirstGif();
        TryConvertFirstStream();
    }

    /// <summary>
    /// Enqueues an array of Gif frame streams for conversion. Once converted, they will be sent to chat
    /// </summary>
    public static void EnqueueGifFramesStreams(Stream[] streams, string sentBy, string url)
    {
        // We generate a unique hashcode for every new Gif so that if two Gifs are received at the same time, 
        // they will be dealt with separately
        uint hashCode = (uint)(DateTime.Now.GetHashCode() ^ sentBy.GetHashCode());

        s_awaitingStreams.Add(hashCode, new());
        s_awaitingGifs.Add(hashCode, new());
        s_gifDatas.Add(hashCode, (sentBy, url));

        foreach (var stream in streams)
        {
            s_awaitingStreams[hashCode].Enqueue(stream);
        }
    }

    /// <summary>
    /// Converts the first awaiting frame into a Texture2D
    /// </summary>
    private void TryConvertFirstStream()
    {
        if (!s_awaitingStreams.Any())
            return;

        var awaitingStream = s_awaitingStreams.First();

        // This check might be unnecessary
        if (awaitingStream.Value == null || !awaitingStream.Value.Any())
            return;

        uint hash = awaitingStream.Key;
        var queue = awaitingStream.Value;

        var stream = queue.Dequeue();

        s_awaitingGifs[hash].Add(Texture2D.FromStream(Main.instance.GraphicsDevice, stream));

        stream.Dispose();
    }

    /// <summary>
    /// Sends the first fully converted Gif in chat
    /// </summary>
    private void TrySendFirstGif()
    {
        if (!s_awaitingGifs.Any())
            return;
        
        // Get all Gifs that are ready to be sent. If none are found, return
        var readyGifs = s_awaitingGifs.Where(kv => !s_awaitingStreams[kv.Key].Any());
        if (!readyGifs.Any())
            return;

        var awaitingGif = readyGifs.First();

        uint hash = awaitingGif.Key;
        var queue = awaitingGif.Value;

        try
        {
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
