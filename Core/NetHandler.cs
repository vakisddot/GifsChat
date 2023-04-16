using GifsChat.Models.Communicators;
using GifsChat.Utils.Exceptions;
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
                    if (Main.netMode is NetmodeID.Server)
                    {
                        var p = GifsChatMod.Instance.GetPacket();

                        p.Write((byte)2);
                        p.Write(reader.ReadString());
                        p.Write(reader.ReadString());

                        p.Send(ignoreClient: whoAmI);
                    }
                    else
                    {
                        string gifUrl = reader.ReadString();
                        string sentBy = reader.ReadString();

                        if (!GifsChatMod.ClientConfig.GifsEnabled || !GifsChatMod.ServerConfig.GifsEnabled)
                            return;

                        ICommunicator communicator = new TenorCommunicator();
                        communicator.ExtractAndSendGif(gifUrl, sentBy);
                    }
                }
                catch (GifsChatException e)
                {
                    Main.NewText(e.Message, Color.Orange);
                }
                catch { }
                break;

        }
    }
    public static void SendGifURLPacket(string url, string senderPlayerName)
    {
        var packet = GifsChatMod.Instance.GetPacket();

        packet.Write((byte)PacketType.GifURL);
        packet.Write(url);
        packet.Write(senderPlayerName);

        packet.Send();
    }
}
