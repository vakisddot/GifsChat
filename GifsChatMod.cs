using GifsChat.Configs;
using GifsChat.Core;
using GifsChat.Models.Communicators;
using GifsChat.Utils.Exceptions;
using Ionic.Zlib;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GifsChat;

/// <summary>
/// 
/// Made by Vakis / @vakisddot
/// 
/// | Credits:
///   |  Cyrillya - Creator of the Image Chat mod
///   |  Tenor - GIF search engine
/// 
/// | Gifs Chat is open-source under the GPL-3.0 license
/// 
/// </summary>

public class GifsChatMod : Mod
{
    internal static string FolderPath
        => Main.SavePath + Path.DirectorySeparatorChar
        + "Captures" + Path.DirectorySeparatorChar
        + "CachedImages" + Path.DirectorySeparatorChar;

    internal static ServerConfig ServerConfig;
    internal static ClientConfig ClientConfig;
    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        switch (reader.ReadByte())
        {
            case 2:
                try
                {
                    if (Main.netMode is NetmodeID.Server)
                    {
                        var p = GetPacket();

                        p.Write((byte)2);
                        p.Write(reader.ReadString());
                        p.Write(reader.ReadString());

                        p.Send(ignoreClient: whoAmI);
                    }
                    else
                    {
                        string gifUrl = reader.ReadString();
                        string sentBy = reader.ReadString();

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
    public static void LocalSendImage(params Texture2D[] textures)
    {
        RemadeChatMonitorHooks.SendTexture(textures);
    }
}