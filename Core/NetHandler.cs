using System;
using GifsChat.Utils;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;

namespace GifsChat.Core;

public enum PacketType
{
    GifURL
}

public static class NetHandler
{
    /// <summary>
    /// Handles all sorts of packets ;)
    /// </summary>
    public static void HandlePacket(BinaryReader reader, int whoAmI)
    {
        switch (reader.ReadByte())
        {
            case (byte)PacketType.GifURL:
                try
                {
                    string gifUrl = reader.ReadString();
                    string sentBy = reader.ReadString();

                    // Server
                    if (Main.netMode is NetmodeID.Server)
                    {
                        // Sends the Gif to all clients except the original sender
                        SendGifURLPacket(gifUrl, sentBy, whoAmI);
                    }
                    // Client
                    else
                    {
                        if (string.IsNullOrWhiteSpace(gifUrl)
                            || !GifsChatMod.ClientConfig.GifsEnabled 
                            || !GifsChatMod.ServerConfig.GifsEnabled)
                            return;

                        GifUtils.ExtractAndSendGif(gifUrl, sentBy);
                    }
                }
                catch (Exception e)
                {
                    ModUtils.NewText(e.Message, true);
                }
                break;

        }
    }

    public static void SendGifURLPacket(string url, string senderPlayerName, int whoAmI = -1)
    {
        var packet = GifsChatMod.Instance.GetPacket();

        packet.Write((byte)PacketType.GifURL);
        packet.Write(url);
        packet.Write(senderPlayerName);

        packet.Send(ignoreClient: whoAmI);
    }
}
