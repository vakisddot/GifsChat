using GifsChat.Configs;
using GifsChat.Core;
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

    internal static GifsChatMod Instance;
    internal static List<Color> CacheColors = new();

    internal static Dictionary<string, List<Texture2D>> CacheTextures = new();

    public override void Load()
    {
        Instance = this;
    }
    public override void Unload()
    {
        CacheColors = null;
        CacheTextures = null;
        Instance = null;
    }
    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        switch (reader.ReadByte())
        {
            case 0: // In transit
                if (Main.netMode is NetmodeID.Server)
                {
                    var p = GetPacket();
                    ushort length = reader.ReadUInt16();
                    p.Write((byte)0);
                    p.Write(length);

                    for (int i = 0; i < length; i++)
                    {
                        p.Write(reader.ReadUInt32());
                    }

                    p.Send(ignoreClient: whoAmI);
                }
                else
                {
                    ushort length = reader.ReadUInt16();

                    for (int i = 0; i < length; i++)
                    {
                        CacheColors.Add(new Color
                        {
                            PackedValue = reader.ReadUInt32()
                        });
                    }
                }

                break;

            case 1: // Complete the packet
                if (Main.netMode is NetmodeID.Server)
                {
                    var p = GetPacket();

                    p.Write((byte)1); // 包类型
                    p.Write(reader.ReadByte()); // textures length
                    byte texIndex = reader.ReadByte();
                    p.Write(texIndex);
                    string name = reader.ReadString();
                    p.Write(name);
                    p.Write(reader.ReadUInt16()); // width
                    p.Write(reader.ReadUInt16()); // height

                    if (CacheTextures.ContainsKey(whoAmI + name) && texIndex == 0)
                    {
                        CacheTextures[whoAmI + name].Clear();
                    }

                    p.Send(ignoreClient: whoAmI);
                }
                else
                {
                    byte texturesLength = reader.ReadByte();
                    byte texIndex = reader.ReadByte();
                    string name = reader.ReadString();
                    ushort width = reader.ReadUInt16();
                    ushort height = reader.ReadUInt16();

                    if (!CacheTextures.ContainsKey(whoAmI + name))
                    {
                        CacheTextures.Add(whoAmI + name, new());
                    }

                    if (ClientConfig.SkipEverySecondFrame && texIndex % 2 == 1)
                    {
                        if (texIndex == texturesLength - 1)
                        {
                            Main.NewText(name);
                            RemadeChatMonitorHooks.SendTexture(CacheTextures[whoAmI + name].ToArray(), false);
                            CacheTextures[whoAmI + name].Clear();
                        }

                        return;
                    }

                    var tex = new Texture2D(Main.graphics.GraphicsDevice, width, height);
                    tex.SetData(0, new Rectangle(0, 0, width, height), CacheColors.ToArray(), 0, width * height);

                    CacheTextures[whoAmI + name].Add(tex);

                    if (texIndex == texturesLength - 1)
                    {
                        Main.NewText(name);
                        RemadeChatMonitorHooks.SendTexture(CacheTextures[whoAmI + name].ToArray(), !ClientConfig.SkipEverySecondFrame);
                        CacheTextures[whoAmI + name].Clear();
                    }

                    CacheColors.Clear();
                }

                break;
        }
    }

    public void SendImagePacket(Texture2D[] textures)
    {
        string name = $"<{Main.LocalPlayer.name}>";
        ushort width = (ushort)textures[0].Width;
        ushort height = (ushort)textures[0].Height;
        var colors = new Color[width * height];

        byte texIndex = 0;

        foreach (var tex in textures)
        {
            tex.GetData(0, new Rectangle(0, 0, width, height), colors, 0, width * height);

            int i = 0;

            while (true)
            {
                int end = Math.Min(i + 10000, colors.Length); // 发送[i,end)索引内的所有Color

                var p = GetPacket();
                p.Write((byte)0); // 包类型
                p.Write((ushort)(end - i)); // 发送的Color数量
                for (; i < end; i++)
                {
                    p.Write(colors[i].PackedValue); // Send all colors
                }

                p.Send();

                if (end == colors.Length)
                {
                    break;
                }
            }

            var finishPacket = GetPacket();

            finishPacket.Write((byte)1); // 包类型
            finishPacket.Write((byte)textures.Length);
            finishPacket.Write(texIndex);
            finishPacket.Write(name);
            finishPacket.Write(width);
            finishPacket.Write(height);

            finishPacket.Send();

            texIndex++;
        }
    }
    public static void LocalSendImage(params Texture2D[] textures)
    {
        Main.NewText($"<{Main.LocalPlayer.name}>");

        if (ClientConfig.SkipEverySecondFrame)
        {
            List<Texture2D> filteredTextures = new();

            for (int i = 0; i < textures.Length; i += 2)
            {
                filteredTextures.Add(textures[i]);
            }

            RemadeChatMonitorHooks.SendTexture(filteredTextures.ToArray(), false);
        }
        else
        {
            RemadeChatMonitorHooks.SendTexture(textures);
        }

        if (Main.netMode is NetmodeID.MultiplayerClient)
        {
            Instance.SendImagePacket(textures);
        }
    }
}