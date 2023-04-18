using GifsChat.Configs;
using GifsChat.Core;
using System.IO;
using Terraria.ModLoader;

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
    internal static ServerConfig ServerConfig;
    internal static ClientConfig ClientConfig;
    internal static GifsChatMod Instance;

    public override void Load()
    {
        Instance = this;
    }
    public override void HandlePacket(BinaryReader reader, int whoAmI)
        => NetHandler.HandlePacket(reader, whoAmI);
}