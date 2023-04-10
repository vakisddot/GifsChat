using GifsChat.Models;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace GifsChat.Configs;

public class ServerConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public override void OnLoaded()
    {
        GifsChatMod.ServerConfig = this;
    }

    [Header("Performance")]
    [Label("GIF Lifetime")]
    [Tooltip("In seconds")]
    [DefaultValue(60)]
    [Range(15, 120)]
    public int GifLifetime;

    [Label("Delay Between GIFs")]
    [DefaultValue(5)]
    [Range(5, 15)]
    public int GifSendDelay;

    [Header("Filter")]
    [Label("Content Filter")]
    [DefaultValue(0)]
    public ContentFilter ContentFilter;
}