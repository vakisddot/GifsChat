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

    [Header("GIFs Chat")]
    [Label("Enable Gifs (Server-side)")]
    [DefaultValue(true)]
    public bool GifsEnabled;

    [Header("Performance")]
    [Label("GIF Lifetime (15-120 sec.)")]
    [DefaultValue(60)]
    [Range(15, 120)]
    public int GifLifetime;

    [Label("Delay Between GIFs (5-15 sec.)")]
    [DefaultValue(5)]
    [Range(5, 15)]
    public int GifSendDelay;

    [Header("Filter")]
    [Label("Content Filter")]
    [DefaultValue(0)]
    public ContentFilter ContentFilter;
}