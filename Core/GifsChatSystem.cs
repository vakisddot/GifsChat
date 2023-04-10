using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace GifsChat.Core;

public class GifsChatSystem : ModSystem
{
    public static Queue<Stream> AwaitingStreams = new();
    private static Queue<Texture2D> _awaitingTextures = new();

    private const int StreamConvertDelay = 0;
    private int _frameCounter;

    public override void PostUpdateEverything()
    {
        TrySendImage();

        if (_frameCounter >= StreamConvertDelay)
        {
            TryConvertFirstStream();
            _frameCounter = 0;
        }

        _frameCounter++;
    }
    private void TryConvertFirstStream()
    {
        if (!AwaitingStreams.Any())
            return;

        var stream = AwaitingStreams.Dequeue();

        // TEMP
        //using (var fileStream = File.Create(DebugFilePath + rand.Next(0, 100000) + ".png"))
        //{
        //    stream.Seek(0, SeekOrigin.Begin);
        //    stream.CopyTo(fileStream);
        //}

        _awaitingTextures.Enqueue(Texture2D.FromStream(Main.instance.GraphicsDevice, stream));
        stream.Dispose();
    }
    private void TrySendImage()
    {
        if (!_awaitingTextures.Any() || AwaitingStreams.Any())
            return;

        try
        {
            GifsChatMod.LocalSendImage(_awaitingTextures.ToArray());
        }
        catch { }

        _awaitingTextures.Clear();
    }
    public static void EnqueueGifFramesStreams(Stream[] streams)
    {
        foreach (var stream in streams)
        {
            AwaitingStreams.Enqueue(stream);
        }
    }
}
